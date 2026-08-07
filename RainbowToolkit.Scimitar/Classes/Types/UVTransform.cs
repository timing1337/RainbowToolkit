namespace RainbowToolkit.Scimitar.Classes.Types;

public class UvTransform : BaseObject {
    public static readonly uint MAGIC = 0xFE77F7CC;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var unk0 = reader.ReadUInt32();
        var scaleU = reader.ReadSingle();
        var scaleV = reader.ReadSingle();
        var translationU = reader.ReadSingle();
        var translationV = reader.ReadSingle();
        var translationSpeedU = reader.ReadSingle();
        var translationSpeedV = reader.ReadSingle();
        var rotation = reader.ReadSingle();
        var rotationSpeed = reader.ReadSingle();
        var unk9 = reader.ReadSingle();
        var unk10 = reader.ReadSingle();

        var unk11 = reader.ReadByte();
        var unk12 = reader.ReadByte();
        var unk13 = reader.ReadByte();

        // Fucking flags or whatever here i dont really care this shit is stupid
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadByte();
        reader.ReadByte();
    }
}
