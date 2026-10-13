/// <summary>
/// Defines an object that represents a shopper and tracks the shelf item they are currently interacting with 
/// and/or caching.
/// </summary>
public interface IShopper
{
    /// <summary>
    /// Gets or sets this shopper's current <see cref="ShelfItem"/>.
    /// </summary>
    ShelfItem CurrentShelfItem { get; set; }
}
