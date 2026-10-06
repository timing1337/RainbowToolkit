using RainbowToolkit.Scimitar.Utils;
using System.Numerics;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class MeshBone : BaseObject {
    public static readonly uint MAGIC = 0x3761F6E4;
    protected override uint Magic => MAGIC;

    public uint BoneId;
    public Matrix4x4 Transform;

    public override void Parse(FastLoadReader reader) {
        BoneId = reader.ReadUInt32();
        Transform = reader.ReadStruct<Matrix4x4>();
    }
}
