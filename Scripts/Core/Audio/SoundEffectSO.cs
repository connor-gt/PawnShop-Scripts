using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Data container for sound effects. Use <see cref="SoundEffectPlayer"/> to play the sound effects.
/// </summary>
[CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Scriptable Objects/New Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    /// <summary>
    /// Gets this sound effect's audio clips.
    /// </summary>
    [field: Title("Clips")]
    [field: SerializeField]
    public AudioClip[] Clips { get; private set; }
    
    /// <summary>
    /// Gets this sound effect's volume. Volume.x acts as the minimum volume and Volume.y acts as the maximum.
    /// </summary>
    [field: Title("Volume")]
    [field: Tooltip("Will play the volume at a random level between x and y.")]
    [field: SerializeField]
    public Vector2 Volume { get; private set; } = new(1f, 1f);

    /// <summary>
    /// Gets this sound effect's pitch. Pitch.x acts as the minimum pitch and Pitch.y acts as the maximum.
    /// </summary>
    [field: Title("Volume")]
    [field: Tooltip("Will set the pitch randomly between the values of x and y when the sound effect is played.")]
    [field: SerializeField]
    public Vector2 Pitch { get; private set; } = new(0.9f, 1.1f);

    public enum Order
    {
        Random,
        In_Order,
        Reverse
    }
    
    /// <summary>
    /// Gets the play order of the audio clips.
    /// </summary>
    [field: Title("Play Order")]
    [field: SerializeField]
    public Order PlayOrder { get; private set; } = Order.Random;

    /// <summary>
    /// Getes this sound effect's mixer (this may be null).
    /// </summary>
    [field: Title("Optional Mixer Reference")]
    [field: Tooltip("If this is set, the sound effect that's played will go through the referenced mixer.")]
    [field: SerializeField]
    public AudioMixerGroup Mixer { get; private set; } = null;
}
