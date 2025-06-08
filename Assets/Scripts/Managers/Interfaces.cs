/// <summary>
/// Interface defining an interactable object.
/// Requires an interaction text and an Interact method implementation.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Text shown to the player indicating the interaction action.
    /// </summary>
    public string InteractionText { get; }

    /// <summary>
    /// Method to be called when interaction occurs.
    /// </summary>
    public void Interact();
}
