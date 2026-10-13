using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specific <see cref="CounterInteraction"/> that's used for entities purchasing items.
/// </summary>
public class CheckoutInteraction : CounterInteraction
{
    public CheckoutInteraction(Counter counter, Entity entity) : base(counter, entity) { }

    public override void Begin()
    {
        base.Begin();
        Counter.OnPlayerInServiceAreaChanged += UpdateHoverMessage;

        IReadOnlyList<Item> itemsInInventory = inventoryModule.Empty();
        Counter.PlaceItems(itemsInInventory);

        // Prevent the player from taking items while the customer is waiting to complete the transaction
        Counter.LockItemsFromPlayer(true);

        List<ItemInstance> itemInstances = Counter.GetItemInstances();
        for (int i = 0; i < itemInstances.Count; i++)
        {
            if (itemInstances[i].TryGetComponent(out IHoverMessageUser hoverMessageUser))
            {
                hoverMessageUser.HoverMessage = $"{itemInstances[i].Data.Name}. ";
                hoverMessageUser.HoverMessage += $"Listed for ${itemInstances[i].Item.Pricing.ListingPrice}";
            }
        }

        UpdateHoverMessage();
    }

    /// <summary>
    /// Updates the hover prompt based on whether the player is currently behind the counter.
    /// </summary>
    private void UpdateHoverMessage()
    {
        if (Counter.IsPlayerInServiceArea)
        {
            HoverMessage = interactionModule.BuildPrompt("Complete Transaction");
        }
        else
        {
            HoverMessage = GoToServiceAreaMsg;
        }

        // Refresh the hover prompt in case the player was already hovering over this NPC before the
        // interaction became available and the hoverMessage prompt was built
        interactionModule.RequestRefresh();
    }

    public override void InteractWithEntity()
    {
        base.InteractWithEntity();

        if (Counter.IsPlayerInServiceArea)
        {
            Finish();
        }
    }

    public override void Finish()
    {
        Counter.OnPlayerInServiceAreaChanged -= UpdateHoverMessage;
        base.Finish();

        List<Item> itemsOnCounter = Counter.RetrieveItems();
        int totalSaleValue = ItemServices.GetTotalListingPrice(itemsOnCounter);
        Counter.Register.AddCash(totalSaleValue);

        inventoryModule.Add(itemsOnCounter);
    }
}
