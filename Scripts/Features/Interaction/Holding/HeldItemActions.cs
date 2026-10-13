using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Collects and manages the <see cref="IHoldAction"/>s available for the item currently being held by the player.
/// </summary>
public class HeldItemActions : MonoBehaviour
{
    /// <summary>
    /// Gets whether this component is currently subscribed to the input actions of its held item.
    /// </summary>
    public bool ActionsEnabled { get; private set; }

    private IHoldAction[] heldItemActions;

    private void Awake()
    {
        heldItemActions = GetComponents<IHoldAction>()
            .OrderBy(a => a.HoldActionInput.DisplayOrder)
            .ToArray();
    }

    // Safety net in case this component or game object is somehow destroyed without the caller disabling actions
    private void OnDestroy()
    {
        DisableActions();
    }

    /// <summary>
    /// Enables all hold actions attached to this object by subscribing them to their associated input actions.
    /// <para>
    /// If the actions are already enabled, this method does nothing.
    /// </para>
    /// </summary>
    public void EnableActions()
    {
        if (ActionsEnabled)
        {
            return;
        }

        for (int i = 0; i < heldItemActions.Length; i++)
        {
            heldItemActions[i].InputBinding.Enable();
        }

        ActionsEnabled = true;
    }

    /// <summary>
    /// Disables all hold actions attached to this object by unsubscribing them from their associated input actions.
    /// <para>
    /// If the actions are already disabled, this method does nothing.
    /// </para>
    /// </summary>
    public void DisableActions()
    {
        if (!ActionsEnabled)
        {
            return;
        }

        for (int i = 0; i < heldItemActions.Length; i++)
        {
            heldItemActions[i].InputBinding.Disable();
        }

        ActionsEnabled = false;
    }

    /// <summary>
    /// Gets the <see cref="IHoldAction"/>s that are associated with this held item.
    /// </summary>
    /// <returns>An array of <see cref="IHoldAction"/>.</returns>
    public IHoldAction[] GetHoldActions()
    {
        return heldItemActions;
    }
}
