using UnityEngine;

/// <summary>
/// Provides functionality for managing shopper-specific interactions with shelf items.
/// </summary>
public class ShopperService
{
    private readonly Entity entity; // Used for owning coroutines
    private readonly IShopper shopper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopperService"/> class.
    /// </summary>
    /// <param name="entity">The <see cref="Entity"/> used to schedule delayed operations.</param>
    /// <param name="shopper">The shopper whose shelf item interactions can be managed by this service.</param>
    public ShopperService(Entity entity, IShopper shopper)
    {
        this.entity = entity;
        this.shopper = shopper;
    }

    /// <summary>
    /// Schedules the current shelf item's interaction point to be released after the specified delay.
    /// <para>
    /// Once released, the shopper's current shelf item is cleared if it is still the item that was originally
    /// cached.
    /// </para>
    /// </summary>
    /// <param name="delay">The amount of time, in seconds, to wait before releasing the interaction point.</param>
    public void ScheduleShelfItemRelease(float delay)
    {
        // Cache the current shelf item so the correct interaction point is released even if the shopper's
        // current shelf item changes before the delay expires
        ShelfItem shelfItemToRelease = shopper.CurrentShelfItem;

        entity.Invoke(() =>
        {
            if (shelfItemToRelease != null)
            {
                shelfItemToRelease.InteractionPoint.Release();

                // Only clear the current shelf item if it has not been replaced since the release was scheduled
                if (shopper.CurrentShelfItem == shelfItemToRelease)
                {
                    shopper.CurrentShelfItem = null;
                }
            }

        }, delay);
    }

    /// <summary>
    /// Attempt to release the shopper's current shelf item.
    /// <para>
    /// This released the shelf item's interaction point and sets the <see cref="IShopper.CurrentShelfItem"/>
    /// to null.
    /// </para>
    /// </summary>
    /// <returns>True if the current shelf item exists and it was released, false if otherwise.</returns>
    public bool TryReleaseCurrentShelfItem()
    {
        if (shopper.CurrentShelfItem != null)
        {
            shopper.CurrentShelfItem.InteractionPoint.Release();
            shopper.CurrentShelfItem = null;
            return true;
        }

        return false;
    }
}
