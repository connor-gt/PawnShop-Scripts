using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    Master,
    Music,
    SFX,
    UI
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Title("Master Mixer")]
    [SerializeField]
    private AudioMixer masterMixer;

    /// <summary>
    /// Gets the minimum possible volume <see cref="AudioType"/>'s can be set to.
    /// </summary>
    [field: Title("Volume Settings")]
    [field: Tooltip("The minimum possible volume for each audio type.")]
    [field: SerializeField]
    public float MinVolume { get; private set; } = 0f;

    /// <summary>
    /// Gets the default volume for <see cref="AudioType"/>'s (the volume that the game loads with the very
    /// first time the game is started).
    /// </summary>
    [field: Tooltip("The default volume for each audio type.")]
    [field: SerializeField]
    public float DefaultVolume { get; private set; } = 100f;

    /// <summary>
    /// Gets the maximum possible volume <see cref="AudioType"/>'s can be set to.
    /// </summary>
    [field: Tooltip("The maximum possible volume for each audio type.")]
    [field: SerializeField]
    public float MaxVolume { get; private set; } = 200f;

    /// <summary>
    /// Near-zero value that is used to remap the volume range. This cannot be 0 since the logarithmic scale
    /// used for audio volume control requires a non-zero value (if exactly 0, volume goes back to full).
    /// </summary>
    private const float MIN_REMAP_VALUE = 0.0001f;

    /// <summary>
    /// Gets the maximum value that the volume can be remapped to.
    /// </summary>
    private float MaxRemapValue => MaxVolume / DefaultVolume;

    /// <summary>
    /// A dictionary that maps each <see cref="AudioType"/> to its corresponding PlayerPrefs key.
    /// </summary>
    private static readonly Dictionary<AudioType, string> VolumeKeys =
        Enum.GetValues(typeof(AudioType))
            .Cast<AudioType>()
            .ToDictionary(audioType => audioType, audioType => $"Volume_{audioType}");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolumesFromPlayerPrefs();
    }
    
    private void LoadVolumesFromPlayerPrefs()
    {
        foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
        {
            float volume = PlayerPrefs.GetFloat(GetPlayerPrefsVolumeKey(audioType), DefaultVolume);
            SetVolume(audioType, volume);
        }
    }

    /// <summary>
    /// Sets the volume for a specific audio type.
    /// </summary>
    /// <param name="audioType">The type of audio for which to set the volume.</param>
    /// <param name="volume">The volume to set.</param>
    public void SetVolume(AudioType audioType, float volume)
    {
        // Save the volume to PlayerPrefs directly instead of the remapped value so that volume settings don't
        // need to worry about remapping it back to a slider scale
        volume = Mathf.Clamp(volume, MinVolume, MaxVolume);
        PlayerPrefs.SetFloat(GetPlayerPrefsVolumeKey(audioType), volume);

        volume = MathUtils.Remap(volume, MinVolume, MaxVolume, MIN_REMAP_VALUE, MaxRemapValue);
        masterMixer.SetFloat(GetPlayerPrefsVolumeKey(audioType), Mathf.Log10(volume) * 20f);
    }

    /// <summary>
    /// Gets the volume for a specific audio type.
    /// </summary>
    /// <param name="audioType">The type of audio for which to get the volume.</param>
    /// <returns>The volume for the specified audio type.</returns>
    public float GetVolume(AudioType audioType)
    {
        return PlayerPrefs.GetFloat(GetPlayerPrefsVolumeKey(audioType), DefaultVolume);
    }

    /// <summary>
    /// Gets the PlayerPrefs key for the volume of a specific audio type (e.g. Volume_Master, Volume_SFX, etc.).
    /// </summary>
    /// <param name="audioType">The type of audio for which to get the volume key.</param>
    /// <returns>The PlayerPrefs key as a string for the volume of the specified audio type.</returns>
    public string GetPlayerPrefsVolumeKey(AudioType audioType)
    {
        return VolumeKeys[audioType];
    }
}
