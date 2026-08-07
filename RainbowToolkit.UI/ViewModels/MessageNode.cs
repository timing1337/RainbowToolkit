namespace RainbowToolkit.UI.ViewModels;

/// <summary>
/// A non-data status row shown inside the tree — e.g. "Loading…", "(no assets)",
/// or an error when a fat file can't be read as an asset container.
/// </summary>
public sealed class MessageNode : NodeViewModelBase
{
    public MessageNode(string text)
    {
        Name = text;
    }
}
