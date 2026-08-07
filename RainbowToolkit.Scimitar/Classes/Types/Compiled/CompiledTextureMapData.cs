namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledTextureMapData : BaseObject {
    public static readonly uint MAGIC = 0xc30c4b3d;
    protected override uint Magic => MAGIC;

    public int Width;
    public int Height;
    public int Depth;
    public int NbMipMaps;
    public int FirstMip;
    public int LastMip;
    public int MipOffset;

    public byte[] ImageBuffer = [];

    public override void Parse(FastLoadReader reader) {
        var imageLength = reader.ReadUInt32();
        reader.ReadUInt32();
        ImageBuffer = reader.ReadBytes((int)imageLength - 4);
        Width = reader.ReadInt32();
        Height = reader.ReadInt32();
        Depth = reader.ReadInt32();
        var unk1 = reader.ReadUInt32();
        FirstMip = reader.ReadInt32();
        LastMip = reader.ReadInt32();
        NbMipMaps = reader.ReadInt32();
        MipOffset = reader.ReadInt32();
    }
}
