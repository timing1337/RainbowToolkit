namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledMesh : BaseObject {
    public static readonly uint MAGIC = 0xfc9e1595;
    protected override uint Magic => MAGIC;

    public MeshData Data;
    public uint PlatformVersion;
    public uint SdkVersion;

    public override void Parse(FastLoadReader reader) {
        var compiledMeshLength = reader.ReadUInt32();
        var compiledMesh = reader.ReadBytes((int)compiledMeshLength);
        Data = MeshData.Read(compiledMesh);
        PlatformVersion = reader.ReadUInt32();
        SdkVersion = reader.ReadUInt32();
    }
}
