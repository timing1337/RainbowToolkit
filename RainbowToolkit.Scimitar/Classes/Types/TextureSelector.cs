namespace RainbowToolkit.Scimitar.Classes.Types;

public class TextureSelector : BaseObject {
    public static readonly uint MAGIC = 0x7E34C538;
    protected override uint Magic => MAGIC;

    public ulong TextureBaseUid;
    public uint TextureSpecificationMethod; // enum
    public uint MapType; // enum

    public override void Parse(FastLoadReader reader) {
        TextureSpecificationMethod = reader.ReadUInt32();
        MapType = reader.ReadUInt32();
        TextureBaseUid = reader.ReadUInt64();
    }
}
