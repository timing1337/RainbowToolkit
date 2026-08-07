using Cast.NET;
using Cast.NET.Nodes;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Xml.Linq;

namespace RainbowToolkit.Sandbox.Helpers;

public static class MeshHelper {
    public static ModelNode ExportLod(Mesh mesh, CompiledMeshObject meshObj, uint lodIndex, Dictionary<ulong, Material> materialOverrides) {
        var meshData = meshObj.Mesh.Data;
        if (meshData.Revision == 0) {
            throw new Exception("Unsupported revision.");
        }

        var modelNode = new ModelNode();
        var lodInfo = meshData.Lods[lodIndex];
        var meshesNode = ReadMeshInfoCast(mesh, meshData, lodInfo);
        foreach (var meshNode in meshesNode) {
            modelNode.AddNode(meshNode);
        }

        RemapMaterial(modelNode, mesh, lodInfo, materialOverrides);

        return modelNode;
    }

    public static void PopulateSkeleton(ModelNode node, Mesh mesh, List<Skeleton> skeletons) {
        var allBones = skeletons.SelectMany(s => s.Bones).ToList();
        var skeletonNode = node.AddNode<SkeletonNode>();

        for (int i = 0; i < mesh.MeshBones.Length; i++) {
            var meshBone = mesh.MeshBones[i];
            var bone = allBones.FirstOrDefault(b => b.BoneId == meshBone.BoneId);

            if (bone == null) {
                throw new Exception($"Bone with ID {meshBone.BoneId:X} not found in any skeleton.");
            }

            var boneNode = skeletonNode.AddNode<BoneNode>();
            boneNode.Name = $"bone_{meshBone.BoneId:X}";
            Matrix4x4 childGlobal;
            Matrix4x4.Invert(meshBone.Transform, out childGlobal);

            // HACK
            // Okay for some reasons not ALL bones in skeleton are in mesh bone
            // This fucks up hierachy. What we do is going up until we find a bone that is in mesh bone
            // if it doesn't exist then we just set it to null and make it a root bone

            MeshBone? parentMeshBone = null;
            if (bone.ParentBone != null) {
                var currentParentBone = bone.ParentBone;

                while (currentParentBone != null &&
                       !mesh.MeshBones.Any(b => b.BoneId == currentParentBone.BoneId)) {
                    currentParentBone = currentParentBone.ParentBone;
                }

                if (currentParentBone != null) {
                    parentMeshBone = mesh.MeshBones.First(b => b.BoneId == currentParentBone.BoneId);
                }
            }

            if (parentMeshBone != null) {
                var local = childGlobal * parentMeshBone.Transform;
                Matrix4x4.Decompose(local, out var s, out var r, out var t);
                boneNode.Scale = s;
                boneNode.LocalRotation = r;
                boneNode.LocalPosition = t;
                boneNode.ParentIndex = mesh.MeshBones.IndexOf(parentMeshBone);
            } else {
                Matrix4x4.Decompose(childGlobal, out var s, out var r, out var t);
                boneNode.Scale = s;
                boneNode.LocalRotation = r;
                boneNode.LocalPosition = t;
                boneNode.ParentIndex = -1;
            }
        }
    }

    private static void RemapMaterial(ModelNode node, Mesh mesh, MeshLod lod, Dictionary<ulong, Material> materialOverrides) {
        for (int i = 0; i < lod.Primitives.Length; i++) {
            var primitive = lod.Primitives[i];
            var meshNode = node.Meshes[i];
            var material = mesh.MaterialUids[primitive.MaterialId];

            if (materialOverrides.ContainsKey(material)) {
                material = materialOverrides[material].Uid;
            }

            var materialNode = new MaterialNode($"material_{material:X}", "pbr");
            node.AddNode(materialNode);
            meshNode.Material = materialNode;
        }
    }

    private static MeshNode[] ReadMeshInfoCast(Mesh mesh, MeshData data, MeshLod lod) {
        if (data.Revision == 0) {
            throw new Exception("Unsupported revision 0");
        }

        if (data.VertexFormat <= 28) {
            return ReadMeshInfoPre28(data, lod);
        } else {
            return ReadMeshInfoPost28(data, lod);
        }
    }

    private static MeshNode[] ReadMeshInfoPre28(MeshData data, MeshLod lod) {
        var vertexReader = new BinaryReader(new MemoryStream(data.VertexBuffer));
        var indexReader = new BinaryReader(new MemoryStream(data.IndexBuffer));
        var primitives = lod.Primitives;
        var nodes = new MeshNode[primitives.Length];

        for (int p = 0; p < primitives.Length; p++) {
            var primitive = primitives[p];
            var meshNode = new MeshNode();

            meshNode.UVLayerCount = 1;

            meshNode.AddArray<Vector3>("vp", (int)primitive.VertexCount);
            meshNode.AddArray<Vector3>("vn", (int)primitive.VertexCount);
            meshNode.AddArray<Vector2>("u0", (int)primitive.VertexCount);
            meshNode.AddArray<ushort>("f");

            if (data.VertexFormat == 28) {
                meshNode.ColorLayerCount = 1;
                meshNode.AddArray<uint>("c0", (int)primitive.VertexCount);
            }

            nodes[p] = meshNode;
        }

        // Positions
        foreach (var meshNode in nodes) {
            var vp = meshNode.GetArrayProperty<Vector3>("vp");
            for (int i = 0; i < vp.Values.Capacity; i++) {
                vp.Add(vertexReader.ReadUInt64AsPos());
            }
        }

        // Normals
        foreach (var meshNode in nodes) {
            var vn = meshNode.GetArrayProperty<Vector3>("vn");
            for (int i = 0; i < vn.Values.Capacity; i++) {
                vn.Add(vertexReader.ReadUInt32AsVec());
            }
        }

        // Colors
        if (data.VertexFormat == 28) {
            foreach (var meshNode in nodes) {
                var c0 = meshNode.GetArrayProperty<uint>("c0");
                for (int i = 0; i < c0.Values.Capacity; i++) {
                    c0.Add(vertexReader.ReadUInt32());
                }
            }
        }

        // TexCoords
        foreach (var meshNode in nodes) {
            var u0 = meshNode.GetArrayProperty<Vector2>("u0");
            for (int i = 0; i < u0.Values.Capacity; i++) {
                var uv = vertexReader.ReadUInt32AsUv();
                u0.Add(uv);
            }
        }

        for (int i = 0; i < primitives.Length; i++) {
            var primitive = primitives[i];
            var meshNode = nodes[i];
            var faceBuffer = meshNode.GetProperty("f") as CastArrayProperty<ushort>;
            var offset = primitive.IndexOffset * 64 * 3 * 2;
            indexReader.BaseStream.Seek(offset, SeekOrigin.Begin);

            for (int j = 0; j < primitive.IndexCount * 64; j++) {
                var a = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                var b = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                var c = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                if (a == b || b == c || a == c) {
                    continue;
                }
                faceBuffer.Add(a);
                faceBuffer.Add(b);
                faceBuffer.Add(c);
            }
        }
        return nodes;
    }

    private static MeshNode[] ReadMeshInfoPost28(MeshData data, MeshLod lod) {
        var vertexReader = new BinaryReader(new MemoryStream(data.VertexBuffer));
        var indexReader = new BinaryReader(new MemoryStream(data.IndexBuffer));
        var primitives = lod.Primitives;
        var nodes = new MeshNode[primitives.Length];

        for (int p = 0; p < primitives.Length; p++) {
            var primitive = primitives[p];
            var meshNode = new MeshNode();
            meshNode.UVLayerCount = 1;
            meshNode.MaximumWeightInfluence = 4;

            meshNode.AddArray<Vector3>("vp", (int)primitive.VertexCount);
            meshNode.AddArray<Vector3>("vt", (int)primitive.VertexCount);
            meshNode.AddArray<Vector3>("vn", (int)primitive.VertexCount);
            meshNode.AddArray<Vector2>("u0", (int)primitive.VertexCount);
            meshNode.AddArray<ushort>("f");

            if (data.VertexFormat == 36) {
                meshNode.AddArray<byte>("wb", (int)primitive.VertexCount * 4);
                meshNode.AddArray<float>("wv", (int)primitive.VertexCount * 4);
            }

            nodes[p] = meshNode;
        }

        // Positions
        foreach (var meshNode in nodes) {
            var vp = meshNode.GetArrayProperty<Vector3>("vp");
            for (int i = 0; i < vp.Values.Capacity; i++) {
                vp.Add(vertexReader.ReadStruct<Vector3>());
            }
        }

        // Normals
        foreach (var meshNode in nodes) {
            var vn = meshNode.GetArrayProperty<Vector3>("vn");
            for (int i = 0; i < vn.Values.Capacity; i++) {
                vn.Add(vertexReader.ReadUInt32AsVec());
            }
        }

        // Tangents
        foreach (var meshNode in nodes) {
            var vt = meshNode.GetArrayProperty<Vector3>("vt");
            for (int i = 0; i < vt.Values.Capacity; i++) {
                vt.Add(vertexReader.ReadUInt32AsVec());
            }
        }

        // Binormals
        foreach (var primitive in primitives) {
            vertexReader.BaseStream.Seek(primitive.VertexCount * 4, SeekOrigin.Current);
        }

        // Uvs
        foreach (var meshNode in nodes) {
            var u0 = meshNode.GetArrayProperty<Vector2>("u0");
            for (int i = 0; i < u0.Values.Capacity; i++) {
                var uv = vertexReader.ReadUInt32AsUv();
                u0.Add(uv);
            }
        }

        // Need more inputs from parsing other models
        // This could well be colors vertex if the model itself doesnt have any bones
        // Maybe we need to use the flag on the original mesh data?
        if (data.VertexFormat == 36) {
            for (int i = 0; i < primitives.Length; i++) {
                var primitive = primitives[i];
                var meshNode = nodes[i];
                var boneMap = data.SkinMaps[i];
                var wb = meshNode.GetArrayProperty<byte>("wb");
                for (int j = 0; j < primitive.VertexCount; j++) {
                    var bone1 = vertexReader.ReadByte();
                    var bone2 = vertexReader.ReadByte();
                    var bone3 = vertexReader.ReadByte();
                    var bone4 = vertexReader.ReadByte();
                    wb.Add(bone1);
                    wb.Add(bone2);
                    wb.Add(bone3);
                    wb.Add(bone4);
                }
            }

            for (int i = 0; i < primitives.Length; i++) {
                var primitive = primitives[i];
                var meshNode = nodes[i];
                var wv = meshNode.GetArrayProperty<float>("wv");
                for (int j = 0; j < primitive.VertexCount; j++) {
                    var w1 = vertexReader.ReadByte();
                    var w2 = vertexReader.ReadByte();
                    var w3 = vertexReader.ReadByte();
                    var w4 = vertexReader.ReadByte();
                    var sum = w1 + w2 + w3 + w4;
                    wv.Add((float)w1 / sum);
                    wv.Add((float)w2 / sum);
                    wv.Add((float)w3 / sum);
                    wv.Add((float)w4 / sum);
                }
            }
        }

        for (int i = 0; i < primitives.Length; i++) {
            var primitive = primitives[i];
            var meshNode = nodes[i];
            var faceBuffer = meshNode.GetProperty("f") as CastArrayProperty<ushort>;
            var offset = primitive.IndexOffset * 64 * 3 * 2;
            indexReader.BaseStream.Seek(offset, SeekOrigin.Begin);

            for (int j = 0; j < primitive.IndexCount * 64; j++) {
                var a = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                var b = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                var c = (ushort)(indexReader.ReadUInt16() - primitive.VertexOffset);
                if (a == b || b == c || a == c) {
                    continue;
                }
                faceBuffer.Add(a);
                faceBuffer.Add(b);
                faceBuffer.Add(c);
            }
        }
        return nodes;
    }
}
