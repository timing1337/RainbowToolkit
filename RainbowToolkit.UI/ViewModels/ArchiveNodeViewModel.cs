using System.Collections.Generic;
using RainbowToolkit.Scimitar;

namespace RainbowToolkit.UI.ViewModels;

/// <summary>
/// A root node in the directory tree — one opened .forge archive. The archive is indexed as
/// soon as it is opened, but its fat files only become rows when the node is first expanded:
/// a big archive holds hundreds of thousands of them, and the rows cost several times more
/// memory than the index they are built from.
/// </summary>
public sealed class ArchiveNodeViewModel : LazyNodeViewModel
{
    private readonly ScimitarFile? _archive;

    /// <summary>Runtime constructor — holds the opened archive so its entries can read from it.</summary>
    public ArchiveNodeViewModel(ScimitarFile archive, string name)
    {
        _archive = archive;
        Name = name;
    }

    /// <summary>Design-time constructor (no backing archive; expanding does nothing).</summary>
    public ArchiveNodeViewModel(string name)
    {
        Name = name;
    }

    protected override bool CanLoad => _archive is not null;

    protected override string EmptyMessage => "(no entries)";

    protected override List<NodeViewModelBase> LoadChildren()
    {
        var archive = _archive!; // CanLoad gates this
        var entries = new List<NodeViewModelBase>(archive.Files.Count);
        foreach (var fatFile in archive.Files.Values)
        {
            entries.Add(new EntryNodeViewModel(archive, fatFile));
        }

        return entries;
    }
}
