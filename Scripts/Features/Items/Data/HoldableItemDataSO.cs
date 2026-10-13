using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Scriptable object that holds data for holdable items that the player can grab and hold. This is used as a
/// field in <see cref="ItemDataSO"/> for items that are holdable.
/// </summary>
[CreateAssetMenu(fileName = "NewHoldableItemData", menuName = "Scriptable Objects/Item Data/New Holdable Item Data")]
public class HoldableItemDataSO : ScriptableObject
{
    /// <summary>
    /// Gets the held prefab of this item (the prefab version the player actually holds).
    /// </summary>
    [field: Title("Held Prefab")]
    [field: Tooltip("The prefab that is instantiated when the player holds this item.")]
    [field: SerializeField]
    public GameObject HeldPrefab { get; private set; }

    [field: Title("Preview Prefabs")]
    [field: Tooltip("The prefab that is instantiated when showing a preview of this item.")]
    [field: SerializeField]
    public GameObject ValidPreviewPrefab { get; private set; }

    [field: Tooltip("The prefab that is instantiated when showing an invalid preview of this item.")]
    [field: SerializeField]
    public GameObject InvalidPreviewPrefab { get; private set; }

    [field: Title("Inventory Icon")]
    [field: SerializeField]
    public Sprite InventoryIcon { get; private set; }

    [field: Title("Held Settings")]
    [field: Tooltip("If enabled, set the item's held prefab's transform.position and transform.rotation to the"
        + " below values.")]
    [field: SerializeField]
    public bool OverridePosition { get; private set; }

    /// <summary>
    /// Gets the offset that can be used for instantiating the held item.
    /// </summary>
    [field: SerializeField]
    public Vector3 Offset { get; private set; }

    /// <summary>
    /// Gets the rotation that can be used for instantiating the held item.
    /// </summary>
    [field: SerializeField]
    public Vector3 Rotation { get; private set; }
}
