/// <summary>
/// Provides the ability for external systems with additional context knowledge to set an object's hover message.
/// <para>
/// For example, a <see cref="Counter"/> displaying a sellable item's name + price. The counter has more
/// contextual knowledge of where that item is than the item itself does.
/// </para>
/// </summary>
public interface IHoverMessageUser
{
    /// <summary>
    /// Gets or sets the current hover message for this <see cref="IHoverMessageUser"/>.
    /// </summary>
    string HoverMessage { get; set; }
}
