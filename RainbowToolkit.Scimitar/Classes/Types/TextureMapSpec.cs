namespace RainbowToolkit.Scimitar.Classes.Types;

public class TextureMapSpec : BaseObject {
    public static readonly uint MAGIC = 0x7C7D57AE;
    protected override uint Magic => MAGIC;

    public ulong TextureMapUid;
    public uint MapType;

    public override void Parse(FastLoadReader reader) {
        MapType = reader.ReadUInt32();
        var unk = reader.ReadByte();
        var unk1 = reader.ReadByte();
        var unk2 = reader.ReadByte();

        TextureMapUid = reader.ReadUInt64();
    }
}
