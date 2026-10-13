#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Debug an entity's behavior by knowing when it enters/exits states.
/// </summary>
public class EntityStateDebugger : MonoBehaviour
{
    [Tooltip("Optional logger reference. If none is provided, a generic logger is added to this game object.")]
    [SerializeField]
    private Logger logger;

    private Entity entity;
    private State currentState;

    private void Awake()
    {
        entity = GetComponent<Entity>();

        if (logger == null)
        {
            logger = gameObject.AddComponent<Logger>();
        }
    }

    private void Update()
    {
        if (currentState == null)
        {
            currentState = entity.StateMachine.ActiveState;
            logger.Log($"{entity.gameObject.name} {currentState} Enter()", entity.gameObject);
        }
        else if (currentState != entity.StateMachine.ActiveState)
        {
            logger.Log($"{entity.gameObject.name} {currentState} Exit()", entity.gameObject);
            currentState = entity.StateMachine.ActiveState;
            logger.Log($"{entity.gameObject.name} {currentState} Enter()", entity.gameObject);
        }
    }
}
#endif
