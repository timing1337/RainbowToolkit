using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class MeshPrimitive {

    public uint VertexOffset;
    public uint VertexCount;
    public uint IndexCount;
    public uint IndexOffset;
    public uint MaterialId;


    public static MeshPrimitive Read(BinaryReader reader) {
        var unk = reader.ReadUInt32();
        var unk1 = reader.ReadUInt32();
        var vertexCount = reader.ReadUInt32();
        var indexOffset = reader.ReadUInt32();
        var indexCount = reader.ReadUInt32();
        var materialId = reader.ReadUInt32();
        var unk5 = reader.ReadUInt32();
        var unk6 = reader.ReadUInt32();
        var unk7 = reader.ReadUInt32();

        return new MeshPrimitive() {
            VertexCount = vertexCount,
            IndexCount = indexCount,
            IndexOffset = indexOffset,
            MaterialId = materialId
        };
    }
}
