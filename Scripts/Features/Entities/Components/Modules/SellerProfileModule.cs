using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A module that acts as a container for a <see cref="SellerProfileSO"/> and a <see cref="SellerInventorySO"/>
/// which influences how this entity negotiates with the player when it is selling items to the player.
/// </summary>
public class SellerProfileModule : MonoBehaviour
{
    /// <summary>
    /// Gets the <see cref="SellerProfileSO"/> for this entity.
    /// </summary>
    [field: Title("Seller Behavior Data")]
    [field: Required]
    [field: SerializeField]
    public SellerProfileSO Profile { get; private set; }

    /// <summary>
    /// Gets this seller's starting inventory (the items that they want to sell to the player).
    /// </summary>
    [field: Title("Starting Inventory")]
    [field: Tooltip("The items that this NPC wants to sell to the player.")]
    [field: Required]
    [field: SerializeField]
    public SellerInventorySO Inventory { get; private set; }

    /// <summary>
    /// Set this module's <see cref="SellerProfileSO"/> to a new one.
    /// </summary>
    /// <param name="newProfile">The new profile to use.</param>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="newProfile"/> is null.</exception>
    public void SetNewProfile(SellerProfileSO newProfile)
    {
        if (newProfile == null)
        {
            throw new ArgumentNullException(nameof(newProfile));
        }

        Profile = newProfile;
    }
}

