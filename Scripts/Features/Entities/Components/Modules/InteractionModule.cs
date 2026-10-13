using UnityEngine;

/// <summary>
/// A module that is responsible for handling player-entity interactions, specifically when the player hovers
/// over the entity and when the player presses the interact keybinding.
/// </summary>
[RequireComponent(typeof(Entity))]
public class InteractionModule : MonoBehaviour, IHoverResponse, IInteractResponse
{
    private Entity entity;
    private Hoverable hoverable;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        hoverable = GetComponent<Hoverable>();

#if UNITY_EDITOR
        // This is here because I often forget to add colliders and get confused why the interactions aren't
        // going through
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"{entity.name} has an interaction module but no collider!");
        }
#endif
    }

    public void OnFocus(IHoverHandler hoverHandler)
    {
        if (entity.Brain is IEntityInteractionHandler entityInteraction)
        {
            hoverHandler.HoverMessage.UpdateHoverText(entityInteraction.GetHoverMessage());
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning($"{name}'s brain is not an IEntityInteractionHandler.");
        }
#endif
    }

    public void OffFocus(IHoverHandler hoverHandler)
    {
        hoverHandler.HoverMessage.ClearHoverText();
    }

    public void Interact()
    {
        if (entity.Brain is IEntityInteractionHandler entityInteraction)
        {
            entityInteraction.InteractWithEntity();
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogWarning($"{name}'s brain is not an IEntityInteractionHandler.");
        }
#endif
    }

    /// <summary>
    /// Build a hover prompt that displays when the player hovers over this <see cref="Entity"/>.
    /// <para>
    /// e.g., "[E]: Talk" is displayed when the input prompt = "Talk".
    /// </para>
    /// </summary>
    /// <param name="prompt">The text display after "[{interactKeybindingString}]: "</param>
    /// <returns>The built prompt as a string.</returns>
    public string BuildPrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return "";
        }

        return InputManager.Instance.GetFormattedBindingDisplay(InputManager.Instance.Actions.Base.Interact, prompt);
    }

    /// <summary>
    /// Requests that this entity's hover prompt be refreshed if the <see cref="Hoverable"/> component is currently
    /// active.
    /// </summary>
    public void RequestRefresh()
    {
        if (GameObjectUtils.FullyActive(hoverable))
        {
            hoverable.RequestRefresh();
        }
    }
}
