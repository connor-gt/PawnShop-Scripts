using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Controller for handling player interactions with hoverable objects in the world. This component is
/// responsible for detecting hoverable objects within range and invoking their focus callbacks when the
/// player looks at them.
/// </summary>
[RequireComponent(typeof(Interactor))]
public class HoverableHandler : MonoBehaviour, IHoverHandler, IPlayerInventoryState, IHoverMessageHandler
{
    /// <summary>
    /// Occurs when the displayed hover text should be updated.
    /// </summary>
    // Sends updates to HoverTextUI.cs, as that script handles displaying text to the gui
    public static event Action<string> OnUpdateHoverText;

    public event Action<Hoverable> OnFocus;
    public event Action<Hoverable> OffFocus;

    [Title("References")]
    [Tooltip("This is used to access the player's hold state and determine how hover responses should "
        + "react to being hovered over.")]
    [SerializeField]
    private HeldItemHandler heldItemHandler;

    /// <summary>
    /// Gets the player's inventory and hold state used by hover responses.
    /// </summary>
    public IPlayerInventoryState PlayerInventoryState => this;

    /// <summary>
    /// Gets the handler used by hover responses to update or clear the displayed hover message.
    /// </summary>
    public IHoverMessageHandler HoverMessage => this;

    /// <summary>
    /// Gets the current <see cref="HeldItemHandler.HoldState"/> of the player.
    /// </summary>
    public HoldState HoldState => heldItemHandler.HoldState;

    /// <summary>
    /// Gets the player's inventory.
    /// </summary>
    public Inventory Inventory => heldItemHandler.Inventory;

    /// <summary>
    /// Gets the <see cref="Hoverable"/> game object the player is currently hovering over.
    /// </summary>
    public Hoverable CurrentHoverable { get; private set; } = null;

    private Interactor interactor;

    /// <summary>
    /// Gets the maximum distance at which hover detection can occur.
    /// </summary>
    public float InteractRange => interactor.InteractRange;

    private Transform InteractorSource => interactor.InteractorSource;
    private LayerMask LayerMask => interactor.LayerMask;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
    }

    // Makes sure to clear the current hoverable when this component is disabled, so that the hoverable doesn't
    // think it's still being hovered over when the player is no longer able to interact with it. This also
    // makes sure hover text is cleared (example use case: player going to bed and hiding hover text)
    private void OnDisable()
    {
        if (CurrentHoverable != null)
        {
            ClearCurrentHoverable();
        }
    }

    private void FixedUpdate()
    {
        if (PhysicsUtils.ObjectInRange(InteractorSource, InteractRange, LayerMask, out Hoverable hoverable))
        {
            UpdateCurrentHoverable(hoverable);
        }
        else if (CurrentHoverable != null)
        {
            ClearCurrentHoverable();
        }
    }

    /// <summary>
    /// Updates the currently hovered object and triggers the appropriate focus callbacks.
    /// </summary>
    /// <param name="hoverable">The <see cref="Hoverable"/> instance currently detected by the raycast.</param>
    private void UpdateCurrentHoverable(Hoverable hoverable)
    {
        if (CurrentHoverable == null)
        {
            SetCurrentHoverable(hoverable);
        }
        else if (CurrentHoverable.gameObject.GetEntityId() != hoverable.gameObject.GetEntityId())
        {
            ClearCurrentHoverable();
            SetCurrentHoverable(hoverable);
        }
    }

    /// <summary>
    /// Assigns the specified <see cref="Hoverable"/> as the current hover target and triggers its initial
    /// focus callbacks.
    /// </summary>
    /// <param name="hoverable">The <see cref="Hoverable"/> instance that has just been detected.</param>
    private void SetCurrentHoverable(Hoverable hoverable)
    {
        CurrentHoverable = hoverable;
        CurrentHoverable.OnRequestRefresh += Refresh;

        CurrentHoverable.OnFocus(this);
        OnFocus?.Invoke(CurrentHoverable);
    }

    /// <summary>
    /// Clears the current hover target and triggers its unfocus callbacks.
    /// </summary>
    private void ClearCurrentHoverable()
    {
        CurrentHoverable.OnRequestRefresh -= Refresh;
        CurrentHoverable.OffFocus(this);

        OffFocus?.Invoke(CurrentHoverable);
        CurrentHoverable = null;
    }

    /// <summary>
    /// Updates the displayed hover text. "" or null can be used to clear the text alongside the
    /// <see cref="ClearHoverText"/> method.
    /// </summary>
    /// <param name="text">The text to display.</param>
    public void UpdateHoverText(string text)
    {
        OnUpdateHoverText?.Invoke(text);
    }

    /// <summary>
    /// Clears the currently displayed hover text.
    /// </summary>
    public void ClearHoverText()
    {
        OnUpdateHoverText?.Invoke(null);
    }

    /// <summary>
    /// Refreshes the current hover target by re-triggering its hover response callbacks.
    /// </summary>
    public void Refresh()
    {
        if (CurrentHoverable != null)
        {
            CurrentHoverable.OnFocus(this);
            OnFocus?.Invoke(CurrentHoverable);
        }
    }
}
