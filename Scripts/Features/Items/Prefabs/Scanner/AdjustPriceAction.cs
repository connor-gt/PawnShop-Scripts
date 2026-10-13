using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Scanner))]
public class AdjustPriceAction : MonoBehaviour, IHoldAction
{
    [field: Title("Input Action")]
    [field: SerializeField]
    public HoldActionInputSO HoldActionInput { get; private set; }

    public TemporaryInputBinding InputBinding { get; private set; }
    
    [Title("Adjust Price By")]
    [SerializeField]
    private int amount = 1;

    private Scanner scanner;
    
    /// <summary>
    /// The coroutine that repeatedly adjusts the price while the input remains held.
    /// </summary>
    private Coroutine adjustmentRoutine = null;

    /// <summary>
    /// Controls the initial delay and repeat interval for continuous price adjustment.
    /// </summary>
    private HoldRepeat holdRepeat;

    /// <summary>
    /// The amount of time the input must be held before continuous price adjustment begins.
    /// </summary>
    private const float HOLD_INITIAL_DELAY = 0.5f;

    /// <summary>
    /// The amount of time between repeated price adjustments while the input remains held.
    /// </summary>
    private const float HOLD_REPEAT_DELAY = 0.04f;

    private void Awake()
    {
        scanner = GetComponent<Scanner>();

        InputBinding = new TemporaryInputBinding(HoldActionInput.InputReference.action, AdjustPrice);
        holdRepeat = new HoldRepeat(HOLD_INITIAL_DELAY, HOLD_REPEAT_DELAY);
    }

    /// <summary>
    /// Adjusts the listing price of the currently targeted item and begins continuously adjusting the price
    /// if the associated input is being held.
    /// </summary>
    /// <param name="ctx">The input callback context for the price adjustment action.</param>
    public void AdjustPrice(InputAction.CallbackContext ctx)
    {
        scanner.TryAdjustItemPrice(amount);

        if (adjustmentRoutine == null && InputBinding.IsInputBeingHeld(HoldActionInput.InputReference.action))
        {
            adjustmentRoutine = StartCoroutine(AdjustWhileHeldRoutine());
        }
    }

    /// <summary>
    /// Continuously adjusts the item's listing price while the action's input remains held.
    /// </summary>
    private IEnumerator AdjustWhileHeldRoutine()
    {
        while (InputBinding.IsEnabled)
        {
            if (holdRepeat.ShouldTick(InputBinding.IsInputBeingHeld(HoldActionInput.InputReference.action)))
            {
                AdjustPrice(default);
            }

            yield return null;
        }

        adjustmentRoutine = null;
    }
}
