namespace RainbowToolkit.UI.ViewModels;

/// <summary>
/// A leaf node in the directory tree — one asset stored inside a fat file.
/// </summary>
public sealed class AssetNodeViewModel : NodeViewModelBase
{
    public AssetNodeViewModel(ulong uid, int size)
    {
        Uid = uid;
        Size = size;
        Name = $"0x{uid:X16}";
    }

    public ulong Uid { get; }

    /// <summary>Uncompressed size of the asset's payload in the container body, in bytes.</summary>
    public int Size { get; }
}
