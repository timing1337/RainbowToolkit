using Cast.NET;
using RainbowToolkit.Scimitar.Assets;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Container;
using System.Security.Cryptography;

namespace RainbowToolkit.Sandbox.Helpers;

public static class BuildTableHelper {
    private static Dictionary<ulong, Material> RemapMaterialOverrideBuildTable(AssetContainer container, BuildTable buildTable) {
        var materialOverrides = new Dictionary<ulong, Material>();
        foreach (var column in buildTable.Rows) {
            foreach (var dynamicProp in column.DynamicProperties) {
                if (dynamicProp.FieldType != OverrideDefinition.MAGIC) {
                    continue;
                }

                if (dynamicProp.Value is not OverrideDefinition def || def.NewMaterial == 0) {
                    continue;
                }

                materialOverrides[def.MaterialToReplace] = container.ReadAsset(def.NewMaterial).As<Material>()!;
            }
        }
        return materialOverrides;
    }
    private static void ExportMeshAndSkeleton(
        AssetContainer container, BuildTable buildTable,
        string path, Dictionary<ulong, Material> materialOverrides) {
        var row = buildTable.Rows.FirstOrDefault();
        if (row == null) {
            throw new Exception("Mesh build table has no rows.");
        }

        List<Skeleton> skel = new();
        Mesh? meshobj = null;
        foreach (var dynamicProp in row.DynamicProperties) {
            Asset? asset = null;

            if (container.ContainsAsset(dynamicProp.ValueUid)) {
                asset = container.ReadAsset(dynamicProp.ValueUid);
            } else if (ScimitarManager.Instance.FindAssetContainer(dynamicProp.ValueUid) is AssetContainer foundContainer) {
                asset = foundContainer.ReadAsset(dynamicProp.ValueUid);
            }

            if (asset == null) {
                throw new Exception($"Could not find asset with UID {dynamicProp.ValueUid} in any container.");
            }

            var assetObj = asset.Data;
            if (assetObj is Skeleton skeleton) {
                skel.Add(skeleton);
            } else if (assetObj is Mesh mesh) {
                meshobj = mesh;
            }
        }

        if (meshobj == null) {
            throw new Exception("Mesh build table is missing a mesh object.");
        }

        // Export mesh first
        var compiledMeshObjAsset = ScimitarManager.Instance.FindAssetContainer(meshobj.CompiledMeshObjectUid)?.ReadAsset(meshobj.CompiledMeshObjectUid);
        if (compiledMeshObjAsset == null) {
            throw new Exception($"Could not find compiled mesh object with UID {meshobj.CompiledMeshObjectUid} in any container.");
        }

        var compiledMeshObj = compiledMeshObjAsset.As<CompiledMeshObject>()!;
        var modelNode = MeshHelper.ExportLod(meshobj, compiledMeshObj, 0, materialOverrides);
        MeshHelper.PopulateSkeleton(modelNode, meshobj, skel);

        var root = new CastNode(CastNodeIdentifier.Root);
        root.AddNode(modelNode);
        CastWriter.Save(Path.Combine(path, "mesh.cast"), root);
    }

    public static void ExportCharacterBuildTable(AssetContainer container, BuildTable buildTable, string path) {
        var row = buildTable.Rows.FirstOrDefault();
        if (row == null) {
            throw new Exception("Character build table has no rows.");
        }

        var meshTable = row.DynamicProperties.FirstOrDefault(prop => prop.PropertyId == 4);
        var materialOverrideTable = row.DynamicProperties.FirstOrDefault(prop => prop.PropertyId == 5);

        if (meshTable == null) {
            throw new Exception("Character build table is missing mesh table property.");
        }

        if (materialOverrideTable == null) {
            throw new Exception("Character build table is missing material override table property.");
        }

        var materialOverrideBuildTable = container.ReadAsset(materialOverrideTable.ValueUid).As<BuildTable>();
        var meshBuildTable = container.ReadAsset(meshTable.ValueUid).As<BuildTable>();

        var materialOverrides = RemapMaterialOverrideBuildTable(container, materialOverrideBuildTable);
        ExportMeshAndSkeleton(container, meshBuildTable, path, materialOverrides);
    }
}
