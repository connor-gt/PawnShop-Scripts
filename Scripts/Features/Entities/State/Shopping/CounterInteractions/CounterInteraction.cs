using System;
using UnityEngine;

/// <summary>
/// Represents a self-contained interaction that occurs while an <see cref="Entity"/> occupies a
/// <see cref="Counter"/>. Derived classes handle implementing the gameplay that takes place at the counter,
/// such as purchasing items, selling items, negotiating, or other scripted events.
/// </summary>
public abstract class CounterInteraction : IEntityInteractionHandler
{
    /// <summary>
    /// Invoked when this interaction has completed and is ready to return control back to the hosting
    /// <see cref="CounterInteractionState"/>.
    /// </summary>
    public event Action OnFinished;

    /// <summary>
    /// Gets whether this interaction is currently active.
    /// </summary>
    public bool IsActive { get; private set; }

    public string HoverMessage { get; set; } = "";

    /// <summary>
    /// Gets the counter hosting this interaction.
    /// </summary>
    public readonly Counter Counter;

    /// <summary>
    /// Gets the entity participating in this interaction.
    /// </summary>
    public readonly Entity Entity;

    protected readonly DialogueModule dialogueModule;
    protected readonly InteractionModule interactionModule;
    protected readonly InventoryModule inventoryModule;

    /// <summary>
    /// The hover message string that's shown to the player if they are not in the service area when they are
    /// hovering hover this NPC at a counter.
    /// </summary>
    // This is a property instead of a const so derived classes can override it
    protected virtual string GoToServiceAreaMsg => "Go Behind Counter To Complete Transaction";

    /// <summary>
    /// Creates a new counter interaction for the specified entity and counter.
    /// </summary>
    /// <param name="counter">The counter hosting this interaction.</param>
    /// <param name="entity">The entity participating in the interaction.</param>
    public CounterInteraction(Counter counter, Entity entity)
    {
        Counter = counter;
        Entity = entity;
        dialogueModule = entity.GetComponent<DialogueModule>();
        interactionModule = entity.GetComponent<InteractionModule>();
        inventoryModule = entity.GetComponent<InventoryModule>();
    }

    /// <summary>
    /// Begins this interaction and marks it as active.
    /// </summary>
    public virtual void Begin()
    {
        IsActive = true;
    }

    /// <summary>
    /// Updates this interaction once per frame while it is active.
    /// </summary>
    public virtual void LogicUpdate() { }

    /// <summary>
    /// Finishes this interaction, marks it as inactive, and notifies the hosting state that the
    /// interaction has completed by invoking <see cref="OnFinished"/>.
    /// </summary>
    public virtual void Finish()
    {
        IsActive = false;
        OnFinished?.Invoke();
    }

    /// <summary>
    /// Gets the hover prompt currently displayed when the player looks at the entity.
    /// </summary>
    /// <returns>The current hover prompt, or an empty string if no interaction is available.</returns>
    public string GetHoverMessage()
    {
        return HoverMessage;
    }

    /// <summary>
    /// Handles the player interacting with the entity while this interaction is active.
    /// </summary>
    public virtual void InteractWithEntity() { }
}
