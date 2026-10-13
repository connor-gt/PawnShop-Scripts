/// <summary>
/// Defines the contract for handling player-initiated interactions with an entity.
/// <para>
/// Specifically, this interface allows both brains and individual states to provide context‑specific
/// interaction behavior:
/// <list type="bullet">
/// <item>General dialogue options while the NPC is browsing.</item>
/// <item>State-specifc interactions such as <see cref="CheckoutState"/> displaying "[E]: Complete Transaction"
/// and calling <see cref="InteractWithEntity"/> completes that sale transaction.</item>
/// </list>
/// </para>
/// </summary>
public interface IEntityInteractionHandler
{
    /// <summary>
    /// Returns the hover message displayed to the player when they look at/hover over the entity.
    /// </summary>
    /// <returns>A string representing the hover prompt shown to the player.</returns>
    string GetHoverMessage();

    /// <summary>
    /// Executes the interaction logic when the player presses the interact keybinding.
    /// </summary>
    void InteractWithEntity();
}
