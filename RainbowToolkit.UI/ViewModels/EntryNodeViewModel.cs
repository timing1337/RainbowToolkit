using System.Collections.Generic;
using RainbowToolkit.Scimitar;

namespace RainbowToolkit.UI.ViewModels;

/// <summary>
/// A fat file inside an archive. Its assets are read and decompressed the first time the
/// node is expanded — a single fat file can hold hundreds of thousands of them.
/// </summary>
public sealed class EntryNodeViewModel : LazyNodeViewModel
{
    private readonly ScimitarFile? _archive;
    private readonly FatFile? _fatFile;

    /// <summary>Runtime constructor — carries what it needs to read its own assets on demand.</summary>
    public EntryNodeViewModel(ScimitarFile archive, FatFile fatFile)
    {
        _archive = archive;
        _fatFile = fatFile;
        Uid = fatFile.Uid;
        Name = $"0x{fatFile.Uid:X16}";
    }

    /// <summary>Design-time constructor (no backing archive; expanding does nothing).</summary>
    public EntryNodeViewModel(string name)
    {
        Name = name;
    }

    public ulong Uid { get; }

    protected override bool CanLoad => _archive is not null && _fatFile is not null;

    protected override string EmptyMessage => "(no assets)";

    protected override List<NodeViewModelBase> LoadChildren()
    {
        var archive = _archive!; // CanLoad gates this

        // The archive's reader is shared across all of its entries, so serialize access.
        lock (archive)
        {
            var container = archive.GetAssetContainer(_fatFile!);
            var header = container.DeserializeHeader();
            var assets = new List<NodeViewModelBase>(header.Count);
            foreach (var (uid, entry) in header)
            {
                assets.Add(new AssetNodeViewModel(uid, entry.Size));
            }

            return assets;
        }
    }
}
