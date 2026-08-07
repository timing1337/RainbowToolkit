using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Collections;

namespace RainbowToolkit.UI.ViewModels;

/// <summary>
/// A node whose children are expensive to build — hundreds of thousands of rows, often read
/// off disk — so they are built on a worker thread the first time the node is expanded and
/// added in a single batch. Until then the node carries one placeholder row, which both
/// gives it an expander chevron and shows what the load is doing.
/// </summary>
public abstract class LazyNodeViewModel : NodeViewModelBase
{
    private AvaloniaList<NodeViewModelBase>? _children;
    private bool _loaded;

    /// <summary>
    /// Allocated on first access, i.e. only for rows the TreeView actually realizes.
    /// AvaloniaList so a whole batch of rows arrives in one change notification.
    /// </summary>
    public AvaloniaList<NodeViewModelBase> Children => _children ??= CreateChildren();

    /// <summary>False for design-time nodes: they have no source to read and are filled in by hand.</summary>
    protected abstract bool CanLoad { get; }

    /// <summary>Shown when the node turns out to hold nothing.</summary>
    protected virtual string EmptyMessage => "(empty)";

    /// <summary>
    /// Builds this node's children. Called once, on a worker thread, and only when
    /// <see cref="CanLoad"/> is true.
    /// </summary>
    protected abstract List<NodeViewModelBase> LoadChildren();

    protected override void OnExpanded()
    {
        if (_loaded || !CanLoad)
        {
            return;
        }

        _loaded = true;
        _ = LoadChildrenAsync();
    }

    private AvaloniaList<NodeViewModelBase> CreateChildren() =>
        CanLoad
            ? new AvaloniaList<NodeViewModelBase> { new MessageNode("Loading…") }
            : new AvaloniaList<NodeViewModelBase>();

    /// <summary>Fire-and-forget from <see cref="OnExpanded"/>, so it never throws.</summary>
    private async Task LoadChildrenAsync()
    {
        try
        {
            var built = await Task.Run(LoadChildren);

            // Back on the UI thread: swap the placeholder for the real rows in one batch.
            Children.Clear();
            if (built.Count == 0)
            {
                Children.Add(new MessageNode(EmptyMessage));
            }
            else
            {
                Children.AddRange(built);
            }
        }
        catch (Exception ex)
        {
            Children.Clear();
            Children.Add(new MessageNode($"(unreadable: {ex.Message})"));
        }
    }
}
