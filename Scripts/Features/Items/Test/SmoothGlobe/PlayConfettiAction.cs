using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayConfettiAction : MonoBehaviour, IHoldAction
{
    [field: Title("Input Action")]
    [field: SerializeField]
    public HoldActionInputSO HoldActionInput { get; private set; }

    public TemporaryInputBinding InputBinding { get; private set; }

    [Title("Confetti")]
    [SerializeField]
    private ParticleSystem confetti;

    private void Awake()
    {
        InputBinding = new TemporaryInputBinding(HoldActionInput.InputReference.action, PlayConfetti);
    }

    public void PlayConfetti(InputAction.CallbackContext ctx)
    {
        confetti.Play();
    }
}
