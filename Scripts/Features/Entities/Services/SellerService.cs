using UnityEngine;

/// <summary>
/// Provides functionality for NPCs who enter the pawn shop to sell items.
/// </summary>
public class SellerService
{
    private readonly InventoryModule inventoryModule;
    private readonly SellerInventorySO sellerInventoryConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="SellerService"/> class.
    /// </summary>
    /// <param name="inventoryModule">The <see cref="InventoryModule"/> that stores the seller's items.</param>
    /// <param name="sellerInventoryConfig">The <see cref="SellerInventorySO"/> that determines which items
    /// should be generated for the seller.</param>
    public SellerService(InventoryModule inventoryModule, SellerInventorySO sellerInventoryConfig)
    {
        this.inventoryModule = inventoryModule;
        this.sellerInventoryConfig = sellerInventoryConfig;
    }

    /// <summary>
    /// Generates the seller's inventory using the configured <see cref="SellerInventorySO"/>.
    /// <para>
    /// Any items currently in the inventory are removed before the newly generated items are added.
    /// </para>
    /// </summary>
    public void GenerateInventory()
    {
        if (inventoryModule.Count > 0)
        {
            inventoryModule.Empty();
        }

        inventoryModule.Add(sellerInventoryConfig.GetItems());
    }
}
