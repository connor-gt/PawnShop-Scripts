using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controller for handling player interactions with interactable objects in the world. This component is
/// responsible for detecting interactable objects within range and invoking their interaction responses when
/// the player interacts with them.
/// </summary>
[RequireComponent(typeof(Interactor))]
public class InteractableHandler : MonoBehaviour
{
    public event Action<Interactable> OnInteract;

    /// <summary>
    /// Gets the <see cref="Interactable"/> game object the player is currently looking at.
    /// </summary>
    public Interactable CurrentInteractable { get; private set; } = null;

    private Interactor interactor;

    /// <summary>
    /// Gets the maximum distance at which interacting with <see cref="Interactable"/>s occurs.
    /// </summary>
    public float InteractRange => interactor.InteractRange;

    private Transform InteractorSource => interactor.InteractorSource;
    private LayerMask LayerMask => interactor.LayerMask;

    private PlayerActions inputActions;

    private void Awake()
    {
        interactor = GetComponent<Interactor>();
        inputActions = InputManager.Instance.Actions;
    }

    private void OnEnable()
    {
        inputActions.Base.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        inputActions.Base.Interact.performed -= OnInteractPerformed;
    }

    private void FixedUpdate()
    {
        if (PhysicsUtils.ObjectInRange(InteractorSource, InteractRange, LayerMask, out Interactable interactable))
        {
            UpdateCurrentInteractable(interactable);
        }
        else if (CurrentInteractable != null)
        {
            CurrentInteractable = null;
        }
    }

    /// <summary>
    /// Updates the currently detected interactable object. If the new interactable is different from the
    /// previously stored one, the reference is replaced.
    /// </summary>
    /// <param name="interactable">The <see cref="Interactable"/> instance detected by the most recent 
    /// raycast.</param>
    private void UpdateCurrentInteractable(Interactable interactable)
    {
        if (CurrentInteractable == null
            || CurrentInteractable.gameObject.GetEntityId() != interactable.gameObject.GetEntityId())
        {
            CurrentInteractable = interactable;
        }
    }

    /// <summary>
    /// Handle when the player presses the interact keybinding by checking if <see cref="CurrentInteractable"/>
    /// is not null and calling <see cref="Interactable.Interact()"/>.
    /// </summary>
    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (CurrentInteractable != null)
        {
            CurrentInteractable.Interact();
            OnInteract?.Invoke(CurrentInteractable);
        }
    }
}
