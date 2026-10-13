using UnityEngine;

/// <summary>
/// A module that is responsible for updating the movement-related animation parameters of an NPC entity based
/// on the current state of its movement.
/// </summary>
[RequireComponent(typeof(Animator)), RequireComponent(typeof(NavigationModule))]
public class MovementAnimationModule : MonoBehaviour
{
    private Animator animator;

    /// <summary>
    /// The animator's speed as set in the inspector.
    /// </summary>
    private float defaultSpeed;

    [Tooltip("The speed at which the animation transitions are dampened. Higher values will result in smoother"
        + " transitions, while lower values will make them more responsive and transition quicker.")]
    [SerializeField]
    [Min(0f)]
    private float dampenSpeed = 0.1f;

    /// <summary>
    /// A cached hash for the "speed" parameter in the <see cref="animator"/>. This improves performance by
    /// avoiding the need to compute the hash from the string every frame update.
    /// </summary>
    private static readonly int SPEED_HASH = Animator.StringToHash("speed");

    private NavigationModule navigationModule;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultSpeed = animator.speed;

        navigationModule = GetComponent<NavigationModule>();
    }

    private void Update()
    {
        float speed = navigationModule.Agent.velocity.magnitude;
        animator.SetFloat(SPEED_HASH, speed, dampenSpeed, Time.deltaTime);
    }

    /// <summary>
    /// Set the animator's speed (1 is normal playback speed).
    /// </summary>
    /// <param name="speed">The new speed of the animator</param>
    public void SetAnimatorSpeed(float speed)
    {
        if (speed < 0f)
        {
            speed = 0f;
        }

        animator.speed = speed;
    }

    /// <summary>
    /// Reset the animator's speed to its default value as set in the inspector.
    /// </summary>
    public void ResetAnimatorSpeed()
    {
        animator.speed = defaultSpeed;
    }
}
