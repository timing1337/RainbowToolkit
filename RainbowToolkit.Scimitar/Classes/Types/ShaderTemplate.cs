namespace RainbowToolkit.Scimitar.Classes.Types;

public class ShaderTemplate : BaseObject {
    public static readonly uint MAGIC = 0xD01611C7;
    protected override uint Magic => MAGIC;

    public uint Unk8355091A; // enum
    public ulong Flags;
    public uint UnkD5F3455B; // enum
    public ShaderCode? Code;
    public uint Unk62C40670;

    public override void Parse(FastLoadReader reader) {
        Unk8355091A = reader.ReadUInt32();
        for (var i = 0; i < 8; i++) Flags = reader.ReadBits(Flags, i);
        Flags = reader.ReadBits(Flags, 8, 2, true);
        for (var i = 10; i < 32; i++) Flags = reader.ReadBits(Flags, i);
        Flags = reader.ReadBits(Flags, 32, 4, true);
        for (var i = 36; i < 49; i++) Flags = reader.ReadBits(Flags, i);
        UnkD5F3455B = reader.ReadUInt32();
        Code = reader.Read<ShaderCode>();
        Unk62C40670 = reader.ReadUInt32();
    }
}
