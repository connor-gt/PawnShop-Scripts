using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A module that is responsible for handling footstep sound effects of an NPC entity. This module handles
/// that by listening for animation events called <see cref="OnFootstepEvent"/>.
/// </summary>
// Animator is required so animation events are guaranteed to reach this module, but this module doesn't
// actually need any reference to an Animator component
[RequireComponent(typeof(Animator))]
public class FootstepPlayerModule : MonoBehaviour
{
    [Title("Sound Effect")]
    [Tooltip("The sound effect to play when this entity takes a footstep. Note that the footstep sound effect "
        + "event should be set up with the appropriate frames in the .fbx animation file's animation events "
        + "with the function name 'OnFootstepEvent'")]
    [SerializeField]
    private SoundEffectSO footstepSoundEffect;

    /// <summary>
    /// Gets the <see cref="AudioSource"/>" component that is used to play footstep sound effects.
    /// </summary>
    [field: Title("Source")]
    [field: SerializeField]
    public AudioSource AudioSource { get; private set; }

    private SoundEffectPlayer soundEffectPlayer;

    private void Awake()
    {
        soundEffectPlayer = new SoundEffectPlayer(footstepSoundEffect);
    }

    /// <summary>
    /// Play a footstep sound effect. This is intended to be called by an animation event to play a synced
    /// footstep sound. Add animation events named <c>OnFootstepEvent</c> in the .fbx animation timeline when
    /// a sound effect should be played.
    /// </summary>
    public void OnFootstepEvent()
    {
        soundEffectPlayer.Play(AudioSource);
    }
}
