using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A module that acts as a container for a <see cref="BuyerProfileSO"/> which influences how this entity
/// decides to purchase items.
/// </summary>
public class BuyerProfileModule : MonoBehaviour
{
    /// <summary>
    /// Gets the <see cref="BuyerProfileSO"/> for this entity.
    /// </summary>
    [field: Title("Buyer Behavior Data")]
    [field: Required]
    [field: SerializeField]
    public BuyerProfileSO Profile { get; private set; }

    /// <summary>
    /// Set this module's <see cref="BuyerProfileSO"/> to a new one.
    /// </summary>
    /// <param name="newProfile">The new profile to use.</param>
    /// <exception cref="ArgumentNullException">Thrown if <see cref="newProfile"/> is null.</exception>
    public void SetNewProfile(BuyerProfileSO newProfile)
    {
        if (newProfile == null)
        {
            throw new ArgumentNullException(nameof(newProfile));
        }

        Profile = newProfile;
    }
}
