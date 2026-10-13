using System;

/// <summary>
/// Contains the text content displayed by systems that display <see cref="Item"/> information, such as
/// <see cref="ItemInfoPopup"/> and <see cref="HeldItemInfoUIController"/>.
/// </summary>
public class ItemInfoContent
{
    /// <summary>
    /// Gets the title text displayed by the popup.
    /// </summary>
    public readonly string Title;

    /// <summary>
    /// Gets the description text displayed by the popup.
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// Gets the individual body text elements displayed by the popup.
    /// </summary>
    public readonly string[] BodyTextElements;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemInfoContent"/> class with the specified title
    /// and description text.
    /// </summary>
    /// <param name="title">The title text to display.</param>
    /// <param name="description">The description text to display.</param>
    public ItemInfoContent(string title, string description)
    {
        Title = title;
        Description = description;
        BodyTextElements = Array.Empty<string>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemInfoContent"/> class with the specified title, 
    /// description, and body text elements.
    /// </summary>
    /// <param name="title">The title text to display.</param>
    /// <param name="description">The description text to display.</param>
    /// <param name="bodyTextElements">The individual body text elements to display.</param>
    public ItemInfoContent(string title, string description, string[] bodyTextElements)
    {
        Title = title;
        Description = description;
        BodyTextElements = bodyTextElements;
    }
}
