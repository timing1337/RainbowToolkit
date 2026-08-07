using Cast.NET.Nodes;
using RainbowToolkit.Scimitar.Classes;
using System.Numerics;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class Mesh : BaseObject {
    public static readonly uint MAGIC = 0xF5C0AFD3;
    protected override uint Magic => MAGIC;

    public uint Category;
    public MeshBone[] MeshBones = [];
    public ulong MultiLodSetupUid;
    public ulong CompiledMeshObjectUid;
    public ulong[] MaterialUids = [];

    public override void Parse(FastLoadReader reader) {
        Category = reader.ReadUInt32();
        var meshBoneCount = reader.ReadUInt32();
        MeshBones = new MeshBone[meshBoneCount];
        for (int i = 0; i < meshBoneCount; i++) {
            MeshBones[i] = reader.Read<MeshBone>();
        }

        MultiLodSetupUid = reader.ReadUInt64();
        var unk0 = reader.ReadByte();
        CompiledMeshObjectUid = reader.ReadUInt64();

        var materialCount = reader.ReadUInt32();
        MaterialUids = new ulong[materialCount];
        for (int j = 0; j < materialCount; j++) {
            MaterialUids[j] = reader.ReadUInt64();
        }
    }
}
