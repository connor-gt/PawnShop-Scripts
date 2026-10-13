/// <summary>
/// Brains that inherit from this interface can be told to exit the shop (even if they aren't necessarily
/// done on their own accord).
/// </summary>
public interface IExitRequester
{
    /// <summary>
    /// Tell this shopper to finish what they are doing and leave the shop.
    /// </summary>
    void WrapUpAndLeave();
}
