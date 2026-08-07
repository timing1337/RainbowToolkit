using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class MeshLod {
    public MeshPrimitive[] Primitives = [];
    public MeshLod(uint numPrimitives) {
        Primitives = new MeshPrimitive[numPrimitives];
    }
}

public class MeshData {

    public uint Revision;
    public uint VertexFormat;
    public uint VertexCount;
    public MeshLod[] Lods = [];
    public MeshSkinMap[] SkinMaps = [];
    public uint meshType;
    public byte[] VertexBuffer = [];
    public byte[] IndexBuffer = [];


    public static MeshData Read(byte[] buffer) {
        var reader = new BinaryReader(new MemoryStream(buffer));

        var version = reader.ReadUInt32();
        var revision = reader.ReadUInt32();
        var vertexFormat = reader.ReadUInt32();
        var vertexBufferLength = reader.ReadUInt32();
        var indexBufferLength = reader.ReadUInt32();
        var vertexMapLength = reader.ReadUInt32();
        var unkBufferLength = reader.ReadUInt32();
        var unk2BufferLength = reader.ReadUInt32();
        var unk3BufferLength = reader.ReadUInt32();
        var unk4BufferLength = reader.ReadUInt32();

        var unk0 = reader.ReadUInt32();
        var meshType = reader.ReadUInt32();
        var numLods = reader.ReadUInt32();
        var unk1 = reader.ReadUInt32();
        var numSubmeshes = reader.ReadUInt32();

        var vertCount = vertexBufferLength / vertexFormat;

        reader.BaseStream.Seek(76, SeekOrigin.Begin);

        var vertexBuffer = reader.ReadBytes((int)vertexBufferLength);
        var indexBuffer = reader.ReadBytes((int)indexBufferLength);
        var vertexMap = reader.ReadBytes((int)vertexMapLength);
        var unkBuffer = reader.ReadBytes((int)unkBufferLength);
        var unk2Buffer = reader.ReadBytes((int)unk2BufferLength);
        var unk3Buffer = reader.ReadBytes((int)unk3BufferLength);
        var unk4Buffer = reader.ReadBytes((int)unk4BufferLength);

        var meshData = new MeshData {
            Revision = revision,
            VertexFormat = vertexFormat,
            VertexCount = vertCount,
            Lods = new MeshLod[numLods],
            SkinMaps = new MeshSkinMap[numSubmeshes],
            meshType = meshType,
            VertexBuffer = vertexBuffer,
            IndexBuffer = indexBuffer
        };

        for (int i = 0; i < numLods; i++) {
            var vertexOffset = 0;
            var lod = new MeshLod(numSubmeshes);
            for (int j = 0; j < numSubmeshes; j++) {
                var meshPrimitive = MeshPrimitive.Read(reader);
                lod.Primitives[j] = meshPrimitive;
                meshPrimitive.VertexOffset = (uint)vertexOffset;
                vertexOffset += (int)meshPrimitive.VertexCount;
            }
            meshData.Lods[i] = lod;
        }

        for (int i = 0; i < numSubmeshes; i++) {
            reader.BaseStream.Seek(32, SeekOrigin.Current);
        }

        for (int i = 0; i < numSubmeshes; i++) {
            var skinMap = MeshSkinMap.Read(reader);
            meshData.SkinMaps[i] = skinMap;
        }

        return meshData;
    }
}
