using System;
using UnityEngine;

[RequireComponent(typeof(ItemInstance))]
public class Holdable : MonoBehaviour, IPickupable
{
    /// <summary>
    /// The event that is triggered when the holdable object is picked up by the player. The object is then
    /// immediately disabled and destroyed after a short delay.
    /// </summary>
    public event Action<Holdable> OnPickedUp;

    private ItemInstance itemInstance;

    /// <summary>
    /// Gets the <see cref="Item"/> that this <see cref="Holdable"/> represents.
    /// </summary>
    public Item Item => itemInstance.Item;

    /// <summary>
    /// Gets the <see cref="ItemDataSO"/> that this <see cref="Holdable"/> represents.
    /// </summary>
    public ItemDataSO Data => Item.Data;

    /// <summary>
    /// The delay before the holdable object is destroyed after being picked up. This is necessary to allow
    /// time for any hover responses to execute before the object is destroyed.
    /// </summary>
    private const float DESTROY_DELAY = 0.25f;

    private void Awake()
    {
        itemInstance = GetComponent<ItemInstance>();
    }
    
    /// <summary>
    /// Called when the player interacts with this <see cref="Holdable"/>. 
    /// </summary>
    /// <param name="context">The hold context representing the player's item-handling system. This context
    /// decides whether the pickup is valid and performs any necessary inventory or visual updates.</param>
    public void Pickup(IPickupHandler context)
    {
        if (enabled)
        {
            context.RequestPickup(this);
        }
    }

    /// <summary>
    /// Consumes the world instance of this <see cref="Holdable"/> by triggering the <see cref="OnPickedUp"/>
    /// event, disabling the game object immediately, and then destroying the object after a delay.
    /// </summary>
    /// <returns>Returns this <see cref="Holdable"/> instance for convenience.</returns>
    public Holdable ConsumeWorldInstance()
    {
        OnPickedUp?.Invoke(this);

        // Object destruction is delayed so that if it is a hoverable object, the OffFocus() method will
        // execute before the object is destroyed
        gameObject.SetActive(false);
        Destroy(gameObject, DESTROY_DELAY);

        return this;
    }
}
