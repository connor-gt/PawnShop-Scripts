using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Class that is used to play sound effects using <see cref="SoundEffectSO"/> parameters.
/// </summary>
public class SoundEffectPlayer
{
    private SoundEffectSO _soundEffect;

    /// <summary>
    /// Gets or sets <see cref="SoundEffectSO"/>. If a new sound effect is set, this <see cref="SoundEffectPlayer"/>'s
    /// volume, pitch, and mixer are set to the new sound effect's values.
    /// </summary>
    public SoundEffectSO SoundEffect
    {
        get => _soundEffect;
        set
        {
            _soundEffect = value;
            Volume = value.Volume;
            Pitch = value.Pitch;
            Mixer = value.Mixer;
            playIndex = 0;
        }
    }

    /// <summary>
    /// Gets or sets this sound effect player's volume.
    /// </summary>
    public Vector2 Volume;

    /// <summary>
    /// Gets or sets this sound effect player's pitch.
    /// </summary>
    public Vector2 Pitch;

    /// <summary>
    /// Gets or sets this sound effect player's mixer.
    /// </summary>
    public AudioMixerGroup Mixer { get; set; }

    /// <summary>
    /// The created game object's name if a game object needs to be created for an audio source to be added to it.
    /// </summary>
    private const string SOURCE_OBJ_NAME = "Sound Source";

    private int playIndex = 0; // Keeps track of the play order

    /// <summary>
    /// Inject a sound effect scriptable object into this player. The sound effect can be changed at anytime by
    /// setting <see cref="SoundEffect"/> to a different <see cref="SoundEffectSO"/> at any point.
    /// </summary>
    /// <param name="soundEffect"></param>
    public SoundEffectPlayer(SoundEffectSO soundEffect)
    {
        SoundEffect = soundEffect; // Uses SoundEffect setter logic
    }

    public AudioSource Play(AudioSource audioSource = null)
    {
        if (SoundEffect.Clips.Length == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"{SoundEffect.name} has no audio clips.");
#endif
            return null;
        }

        if (audioSource == null)
        {
            audioSource = CreateAudioSource(Vector3.zero, Mixer);
        }

        audioSource.clip = GetAudioClip();
        audioSource.volume = Random.Range(Volume.x, Volume.y);
        audioSource.pitch = Random.Range(Pitch.x, Pitch.y);
        audioSource.Play();

        if (audioSource.name == SOURCE_OBJ_NAME)
        {
            Object.Destroy(audioSource.gameObject, audioSource.clip.length / audioSource.pitch);
        }

        return audioSource;
    }

    public AudioSource CreateAudioSource(Vector3 position, AudioMixerGroup mixer = null)
    {
        GameObject obj = new GameObject(SOURCE_OBJ_NAME, typeof(AudioSource));
        AudioSource audioSource = obj.GetComponent<AudioSource>();
        audioSource.transform.position = position;

        if (mixer != null)
        {
            audioSource.outputAudioMixerGroup = mixer;
        }

        return audioSource;
    }

    /// <summary>
    /// Get an audio clip based on the current <see cref="SoundEffectSO.Order"/> value.
    /// </summary>
    /// <returns>The audio clip that is next based on <see cref="SoundEffectSO.Order"/>.</returns>
    private AudioClip GetAudioClip()
    {
        AudioClip clip = SoundEffect.Clips[playIndex >= SoundEffect.Clips.Length ? 0 : playIndex];

        switch (SoundEffect.PlayOrder)
        {
            case SoundEffectSO.Order.Random:
                playIndex = Random.Range(0, SoundEffect.Clips.Length);
                break;
            case SoundEffectSO.Order.In_Order:
                playIndex = (playIndex + 1) % SoundEffect.Clips.Length;
                break;
            case SoundEffectSO.Order.Reverse:
                playIndex = (playIndex + SoundEffect.Clips.Length - 1) % SoundEffect.Clips.Length;
                break;
        }

        return clip;
    }
}
