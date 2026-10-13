using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Base class for interaction controllers that defines common properties such as the source transform and
/// interaction range.
/// </summary>
public class Interactor : MonoBehaviour
{
    /// <summary>
    /// Gets the source Transform of the interactions (where the raycasts are sent out from).
    /// </summary>
    [field: Title("Interaction Source")]
    [field: SerializeField]
    public Transform InteractorSource { get; private set; }

    /// <summary>
    /// Gets or sets the interaction range (<see cref="IInteractResponse"/> objects within this range to
    /// <see cref="InteractorSource"/> will be interactable).
    /// </summary>
    [field: Title("Range")]
    [field: SerializeField]
    public float InteractRange { get; set; } = 3f;

    /// <summary>
    /// Gets the layers the <see cref="Interactor"/> can interact with.
    /// </summary>
    [field: Title("Layer")]
    [field: Tooltip("Objects can only be interacted with if they have the specified layers.")]
    [field: SerializeField]
    public LayerMask LayerMask { get; private set; }
}
