using Sirenix.OdinInspector;
using UnityEngine;

public class EnterTriggerTask : PlayerTask
{
    [Title("Detector")]
    [SerializeField]
    private TriggerDetector triggerDetector;

    private void OnEnable()
    {
        triggerDetector.OnEntered += OnPlayerEntered;
    }

    private void OnDisable()
    {
        triggerDetector.OnEntered -= OnPlayerEntered;
    }

    private void OnPlayerEntered(Collider col)
    {
        TryComplete();
    }
}
