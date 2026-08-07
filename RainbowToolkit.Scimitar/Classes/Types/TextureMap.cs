using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Enums;

namespace RainbowToolkit.Scimitar.Classes.Types;
public class TextureMap : BaseObject {
    public static readonly uint MAGIC = 0x3C7E34FD;
    protected override uint Magic => MAGIC;

    public uint MapType;
    public PixelFormat PixelFormat;
    public TextureFormat TextureFormat;

    public uint Width;
    public uint Height;
    public uint Depth;
    public uint NumberMipsMap;

    public TexturePack Pack1;
    public TexturePack Pack2;

    public override void Parse(FastLoadReader reader) {
        MapType = reader.ReadUInt32();
        PixelFormat = (PixelFormat)reader.ReadUInt32();
        TextureFormat = (TextureFormat)reader.ReadUInt32();
        var gammaSetting = reader.ReadUInt32();
        var unk0 = reader.ReadByte();
        var unk1 = reader.ReadByte();
        var category = reader.ReadUInt32(); // Enum

        reader.ReadByte();
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadUInt32();
        reader.ReadUInt32();
        reader.ReadUInt32();
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadUInt32();

        var userCategoryData = reader.Read<UserResourceCategoryData>();
        Width = reader.ReadUInt32();
        Height = reader.ReadUInt32();
        Depth = reader.ReadUInt32();
        NumberMipsMap = reader.ReadUInt32();

        var unkCount = reader.ReadUInt32();
        reader.BaseStream.Seek(4 * unkCount, SeekOrigin.Current);

        Pack1 = TexturePack.Read(reader);
        Pack2 = TexturePack.Read(reader);
    }
}
