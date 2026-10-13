using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class EntranceDoorHandler : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private Door door;

    [SerializeField]
    private TriggerDetector triggerDetector;

    private readonly List<NavMeshAgent> agentsInTrigger = new();

    private void OnEnable()
    {
        triggerDetector.OnEntered += OnEntered;
        triggerDetector.OnExited += OnExited;
    }

    private void OnDisable()
    {
        triggerDetector.OnEntered -= OnEntered;
        triggerDetector.OnExited -= OnExited;
    }

    // Update is used for constant polling to handle edge cases where the door is transitioning while agents
    // enter or exit the trigger. Without polling, an event could occur while the door is already transitioning
    // and the resulting open/close request would be ignored by Door, potentially leaving the door in the wrong
    // state once the transition finishes (and then the agent could walk straight through it)
    private void Update()
    {
        if (!door.IsOpen && agentsInTrigger.Count > 0)
        {
            door.Open();
        }
    }

    private void OnEntered(Collider col)
    {
        if (col.TryGetComponent(out NavMeshAgent agent) && !agentsInTrigger.Contains(agent))
        {
            agentsInTrigger.Add(agent);
        }
    }

    private void OnExited(Collider col)
    {
        if (col.TryGetComponent(out NavMeshAgent agent) && agentsInTrigger.Remove(agent))
        {
            // This is not in Update() so that the when the player opens the door, it doesn't automatically
            // close itself if there are no agents currently in the trigger
            if (door.IsOpen && agentsInTrigger.Count == 0)
            {
                door.Close();
            }
        }
    }
}
