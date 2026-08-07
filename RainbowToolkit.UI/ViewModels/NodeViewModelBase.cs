using CommunityToolkit.Mvvm.ComponentModel;

namespace RainbowToolkit.UI.ViewModels;

public abstract partial class NodeViewModelBase : ViewModelBase
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    partial void OnIsExpandedChanged(bool value)
    {
        if (value)
        {
            OnExpanded();
        }
    }

    /// <summary>
    /// Called when the node is expanded. Override to lazy-load children the first time.
    /// </summary>
    protected virtual void OnExpanded()
    {
    }
}
