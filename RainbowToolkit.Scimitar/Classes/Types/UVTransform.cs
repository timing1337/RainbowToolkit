using System.Numerics;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class UvTransform : BaseObject {
    public static readonly uint MAGIC = 0x9848465B;
    protected override uint Magic => MAGIC;

    public Vector2 Scale;
    public float[] Unk = new float[8]; // FA3D9988..2E27877B (translation/speed/rotation, unverified)
    public uint Unk7F7AA349; // enum
    public byte UnkD83F6D85, Unk27385DEF, Unk3B486695;
    public byte Flags; // 4 bitfield bools 7E8E6C14..AB8D84D7

    public override void Parse(FastLoadReader reader) {
        Scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
        var translationU = reader.ReadSingle();
        var translationV = reader.ReadSingle();
        var translationSpeedU = reader.ReadSingle();
        var translationSpeedV = reader.ReadSingle();
        var rotation = reader.ReadSingle();
        var rotationSpeed = reader.ReadSingle();
        var unk9 = reader.ReadSingle();
        var unk10 = reader.ReadSingle();
        Unk7F7AA349 = reader.ReadUInt32();
        UnkD83F6D85 = reader.ReadByte();
        Unk27385DEF = reader.ReadByte();
        Unk3B486695 = reader.ReadByte();
        for (var i = 0; i < 4; i++) Flags = (byte)reader.ReadBits(Flags, i);
    }
}
