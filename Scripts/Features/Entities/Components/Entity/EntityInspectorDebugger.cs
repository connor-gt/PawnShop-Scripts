#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This <see cref="Entity"/> debugger script can be attached to game objects and show live values that the
/// <see cref="Entity"/> currently has (including state, brain values, movement values, etc.).
/// </summary>
public class EntityInspectorDebugger : MonoBehaviour
{
    [field: Title("State")]
    [field: ReadOnly]
    [field: SerializeField]
    public string ActiveState { get; private set; } = "None";

    [field: Title("Movement")]
    [field: ReadOnly]
    [field: SerializeField]
    public Vector3 Destination { get; private set; } = Vector3.zero;

    private Entity entity;
    private NavigationModule navigationModule;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        navigationModule = GetComponent<NavigationModule>();
    }

    private void Update()
    {
        ActiveState = entity.StateMachine.ActiveState.ToString();

        if (navigationModule != null)
        {
            Destination = navigationModule.GetDestination();
        }
    }
}
#endif
