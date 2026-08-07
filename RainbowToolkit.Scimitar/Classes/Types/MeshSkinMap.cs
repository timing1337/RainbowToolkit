using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class MeshSkinMap {
    public bool HasSkin;
    public byte BoneCount;
    public byte SubmeshIndex;
    public ushort VertexCount;
    public byte[] BoneMap = [];

    public static MeshSkinMap Read(BinaryReader reader) {
        var hasSkin = reader.ReadBoolean();
        var unk1 = reader.ReadByte();
        var boneCount = reader.ReadByte();
        var submeshIdx = reader.ReadByte();
        var unk4 = reader.ReadByte();
        var unk5 = reader.ReadByte();
        var vertexCount = reader.ReadUInt16();
        var len = reader.ReadByte();
        var boneMap = reader.ReadBytes(len);
        reader.BaseStream.Seek(255 - len, SeekOrigin.Current);
        var unk9 = reader.ReadUInt32();

        return new MeshSkinMap {
            HasSkin = hasSkin,
            BoneCount = boneCount,
            SubmeshIndex = submeshIdx,
            BoneMap = boneMap,
            VertexCount = vertexCount,
        };
    }
}
