using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A counter where NPCs can go to perform interactions (such as purchase/sell items).
/// </summary>
public class Counter : MonoBehaviour
{
    [field: Title("Register")]
    [field: SerializeField]
    public CounterRegister Register { get; private set; }

    [Title("Queue Point Mapping")]
    [SerializeField]
    private InteractionMapping<InteractionPoint> queueMapping;

    [Title("Front Detection")]
    [Tooltip("The maximum distance from the service point for an entity to be considered at the front of the " 
        + "queue.")]
    [SerializeField]
    [Min(0f)]
    private float servicePointTolerance = 0.25f;

    /// <summary>
    /// Gets the list of queue points associated with this counter.
    /// </summary>
    private List<InteractionPoint> QueuePoints => queueMapping.Interactions;

    /// <summary>
    /// The <see cref="InteractionPoint"/> that NPCs should go to when they are ready to interact with the
    /// counter (e.g., purchase/sell items).
    /// </summary>
    private InteractionPoint servicePoint;

    [Title("Placed Item Locations")]
    [Tooltip("The locations that items placed on the counter will be instantiated.")]
    [SerializeField]
    private ItemCounterLocation[] itemCounterLocations;

    /// <summary>
    /// Gets the current number of items that are sitting on this counter.
    /// </summary>
    public int ItemCount
    {
        get
        {
            int count = 0;

            for (int i = 0; i < itemCounterLocations.Length; i++)
            {
                if (itemCounterLocations[i].IsOccupied)
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <summary>
    /// Occurs when the player's presence within the counter's service area changes.
    /// </summary>
    public event Action OnPlayerInServiceAreaChanged;

    [Title("Optional Service Area")]
    [Tooltip("Optional trigger used to determine whether the player is standing behind this counter. When "
        + "assigned, the player must be within this trigger for IsPlayerBehindCounter to be true. Leave this "
        + "field unassigned if the counter should not require the player to be behind it.")]
    [SerializeField]
    private TriggerDetector playerInServiceAreaDetector;

    /// <summary>
    /// Gets whether or not the player is currently standing behind the counter.
    /// <para>
    /// If no <see cref="playerInServiceAreaDetector"/> is assigned in the inspector, this will always be true.
    /// </para>
    /// </summary>
    public bool IsPlayerInServiceArea { get; private set; }

    private void Start()
    {
        servicePoint = queueMapping.Point;

        if (playerInServiceAreaDetector != null)
        {
            // Since counter objects are persistent in the environment, I'm using lambda event subscriptions since
            // they are a bit tidier and it's reasonably expected for counter objects to always be active in while
            // the pawn shop scene is loaded in
            playerInServiceAreaDetector.OnEntered += _ => 
            {
                IsPlayerInServiceArea = true;
                OnPlayerInServiceAreaChanged?.Invoke();
            };

            playerInServiceAreaDetector.OnExited += _ =>
            {
                IsPlayerInServiceArea = false;
                OnPlayerInServiceAreaChanged?.Invoke();
            };
        }
        else
        {
            // No detector means this counter does not enforce a behind-counter requirement, so just treat the
            // player as always being behind so systems using this property can still function
            IsPlayerInServiceArea = true;
        }
    }

    /// <summary>
    /// Attempts to assign the specified <see cref="Entity"/> to the first available queue point.
    /// </summary>
    /// <param name="entity">The NPC attempting to join the queue.</param>
    /// <param name="assignedPoint">The queue point successfully reserved for the NPC, or null if no points
    /// were available.</param>
    /// <returns>True if the NPC was assigned to a queue point, false if otherwise.</returns>
    public bool TryJoinQueue(Entity entity, out InteractionPoint assignedPoint)
    {
        for (int i = 0; i < QueuePoints.Count; i++)
        {
            if (QueuePoints[i].TryReserve(entity))
            {
                assignedPoint = QueuePoints[i];
                return true;
            }
        }

        assignedPoint = null;
        return false;
    }

    /// <summary>
    /// Attempts to move the specified <see cref="Entity"/> forward in the queue.
    /// </summary>
    /// <param name="entity">The NPC attempting to advance.</param>
    /// <param name="nextPoint">The next interaction point the NPC successfully reserved, or null if no
    /// advancement was possible.</param>
    /// <returns>True if the NPC advanced to a new point, false if otherwise.</returns>
    public bool TryAdvanceQueue(Entity entity, out InteractionPoint nextPoint)
    {
        int queuePosition = GetQueuePosition(entity, out InteractionPoint currentAssignedPoint);
        
        // If the entity is at the front of the queue, attempt to move to the service point
        if (queuePosition == 0 && servicePoint.TryReserve(entity))
        {
            currentAssignedPoint.Release();
            nextPoint = servicePoint;
            return true;
        }
        // Otherwise attempt to move forward one queue slot
        else if (queuePosition > 0 && QueuePoints[queuePosition - 1].TryReserve(entity))
        {
            currentAssignedPoint.Release();
            nextPoint = QueuePoints[queuePosition - 1];
            return true;
        }

        nextPoint = null;
        return false;
    }

    /// <summary>
    /// Determines the NPC's current position within the queue.
    /// </summary>
    /// <param name="entity">The NPC whose queue position is being checked.</param>
    /// <param name="currentAssignedPoint">The queue point currently occupied by the NPC, or null if the NPC
    /// is not currently in the queue.</param>
    /// <returns>The zero-based index of the NPC's position in the queue or -1 if the NPC is not currently
    /// assigned to any queue point.</returns>
    public int GetQueuePosition(Entity entity, out InteractionPoint currentAssignedPoint)
    {
        for (int i = 0; i < QueuePoints.Count; i++)
        {
            if (QueuePoints[i].Occupant == entity)
            {
                currentAssignedPoint = QueuePoints[i];
                return i;
            }
        }

        currentAssignedPoint = null;
        return -1;
    }

    /// <summary>
    /// Checks whether the specified <see cref="Entity"/> is currently occupying the service point and has
    /// physically reached it.
    /// </summary>
    /// <param name="entity">The NPC being checked.</param>
    /// <returns>True if the NPC is occupying the service point and is within the tolerance distance set in
    /// the inspector, false if otherwise.</returns>
    public bool IsAtFront(Entity entity)
    {
        if (servicePoint.Occupant != entity)
        {
            return false;
        }

        return entity.transform.WithinDistance(servicePoint.transform, servicePointTolerance);
    }

    /// <summary>
    /// Place a list of <see cref="Item"/>s at this counter.
    /// <para>
    /// <b>Note:</b> this method does NOT replace any existing <see cref="Item"/> world prefabs on this counter,
    /// so if the number of <see cref="Item"/>s to place exceeds the current available amount of
    /// <see cref="ItemCounterLocation"/>s, those <see cref="Item"/>s will not be placed. Any <see cref="Item"/>
    /// that's placed on this counter is placed WITH any existing <see cref="Item"/>s.
    /// </para>
    /// </summary>
    /// <param name="itemsToPlaceOnCounter">The <see cref="Item"/>s to place.</param>
    public int PlaceItems(IReadOnlyList<Item> itemsToPlaceOnCounter)
    {
        int numPlacedItems = 0;

        // Regular for-loop is used since interface collections generate garbage with foreach loops
        for (int i = 0; i < itemsToPlaceOnCounter.Count; i++)
        {
            foreach (var counterLocation in itemCounterLocations)
            {
                if (counterLocation.TryPlaceItem(itemsToPlaceOnCounter[i]))
                {
                    numPlacedItems++;
                    break;
                }
            }
        }

        return numPlacedItems;
    }

    /// <summary>
    /// "Lock" or "unlock" the current <see cref="Item"/>s on this counter by disabling or enabling their
    /// <see cref="Holdable"/> component. This prevents the player from being able to pick up the
    /// <see cref="Item"/>s that are currently on the counter.
    /// </summary>
    /// <param name="shouldLock">Whether or not the <see cref="Holdable"/> should be disabled.</param>
    public void LockItemsFromPlayer(bool shouldLock)
    {
        foreach (var counterLocation in itemCounterLocations)
        {
            ItemInstance itemInstance = counterLocation.CurrentItemInstance;
            if (itemInstance != null && itemInstance.TryGetComponent(out Holdable holdable))
            {
                holdable.enabled = !shouldLock;
            }
        }
    }

    /// <summary>
    /// Get the current <see cref="ItemInstance"/>s on this counter.
    /// <para>
    /// <b>Note:</b> Since this returns a snapshot list, it is best to use the list information immediately
    /// and not store it to use for later since it may become stale by then.
    /// </para>
    /// </summary>
    /// <returns>A snapshot list of the current <see cref="ItemInstance"/>s on this counter.</returns>
    public List<ItemInstance> GetItemInstances()
    {
        List<ItemInstance> itemsOnCounter = new();

        foreach (var counterLocation in itemCounterLocations)
        {
            if (counterLocation.IsOccupied)
            {
                itemsOnCounter.Add(counterLocation.CurrentItemInstance);
            }
        }

        return itemsOnCounter;
    }

    /// <summary>
    /// Get the current <see cref="Item"/>s on this counter.
    /// <para>
    /// <b>Note:</b> Since this returns a snapshot list, it is best to use the list information immediately
    /// and not store it to use for later since it may become stale by then.
    /// </para>
    /// </summary>
    /// <returns>A snapshot list of the current <see cref="Item"/>s on this counter.</returns>
    public List<Item> GetItems()
    {
        List<Item> itemsOnCounter = new();

        foreach (var counterLocation in itemCounterLocations)
        {
            if (counterLocation.IsOccupied)
            {
                itemsOnCounter.Add(counterLocation.CurrentItemInstance.Item);
            }
        }

        return itemsOnCounter;
    }

    /// <summary>
    /// Retrieve the <see cref="Item"/>s that are currently on this counter. Retrieving the current
    /// <see cref="Item"/>s clears this counter by removing all instantiated game objects from their respective
    /// <see cref="ItemCounterLocation"/>s.
    /// </summary>
    /// <returns>A list of <see cref="Item"/>s (the list is empty if there are no <see cref="Item"/>s on this
    /// counter).</returns>
    public List<Item> RetrieveItems()
    {
        List<Item> items = new();

        foreach (var counterLocation in itemCounterLocations)
        {
            Item item = counterLocation.RemoveCurrentItem();
            if (item != null)
            {
                items.Add(item);
            }
        }

        return items;
    }
}
