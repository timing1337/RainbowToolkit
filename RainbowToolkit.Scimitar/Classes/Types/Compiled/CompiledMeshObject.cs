using Cast.NET.Nodes;

namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledMeshObject : BaseObject {
    public static readonly uint MAGIC = 0xABEB2DFB;
    protected override uint Magic => MAGIC;

    public CompiledMesh Mesh;

    public override void Parse(FastLoadReader reader) {
        Mesh = reader.Read<CompiledMesh>();
    }
}
