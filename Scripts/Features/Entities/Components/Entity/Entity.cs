using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Tooltip("The brain asset that defines this entity's behavior.")]
    [Required]
    [SerializeField]
    private BrainSO BrainSO;

    /// <summary>
    /// Gets the runtime brain instance created from the assigned <see cref="BrainSO"/> in the inspector.
    /// </summary>
    public Brain Brain { get; private set; }

    /// <summary>
    /// Gets the state machine used by this entity.
    /// </summary>
    public StateMachine StateMachine { get; private set; } = new();

    private void Start()
    {
        // StateMachine and Brain are created in Start to give time to other systems to register in Awake if
        // states need references
        StateMachine.Initialize(null);
        Brain = BrainSO.CreateBrain(this);
        Brain.StartThinking();
    }
    
    private void Update()
    {
        Brain.LogicUpdate();
    }

    private void FixedUpdate()
    {
        Brain.PhysicsUpdate();
    }

#if UNITY_EDITOR
    private void Reset()
    {
        // Automatically add a navigation module. Since I expect most entities to use navigation, it's nicer
        // to have it added when this component is added. The module also automatically adds related components
        // when it is added to a game object in a similar fashion. Unneeded components can be just be removed
        gameObject.AddComponentsIfMissing(
            typeof(NavigationModule)
        );
    }
#endif
}
