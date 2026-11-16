using Cast.NET;
using RainbowToolkit.Scimitar;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;

namespace RainbowToolkit.Sandbox.Helpers;

public static class BuildTableHelper {
    private static Dictionary<ulong, Material> RemapMaterialOverrideBuildTable(AssetDirectory assets, BuildTable buildTable) {
        var materialOverrides = new Dictionary<ulong, Material>();
        foreach (var column in buildTable.Rows) {
            foreach (var dynamicProp in column.DynamicProperties) {
                if (dynamicProp.FieldType != OverrideDefinition.MAGIC) {
                    continue;
                }

                if (dynamicProp.Value is not OverrideDefinition def || def.NewMaterial == 0) {
                    continue;
                }

                materialOverrides[def.MaterialToReplace] = assets.Get<Material>(def.NewMaterial);
            }
        }
        return materialOverrides;
    }

    private static void ExportChildBuildTable(AssetDirectory assets, BuildTable buildTable, string path) {
        var row = buildTable.Rows.FirstOrDefault();
        if (row == null) {
            return;
        }

        List<Skeleton> skel = new();
        List<Mesh> meshes = new();

        foreach (var dynamicProp in row.DynamicProperties) {
            if (dynamicProp.ValueUid == 0) {
                continue;
            }

            if (dynamicProp.ValueUid == 0) {
                continue;
            }

            var asset = assets.Find(dynamicProp.ValueUid)
                ?? throw new Exception($"Could not find asset with UID {dynamicProp.ValueUid:X} in any container.");

            var assetObj = asset.Data;
            if (assetObj is Skeleton skeleton) {
                skel.Add(skeleton);
            } else if (assetObj is Mesh mesh) {
                meshes.Add(mesh);
            }
        }
        var materialsPath = Path.Combine(path, "materials");
        if (!Directory.Exists(materialsPath)) {
            Directory.CreateDirectory(materialsPath);
        }

        foreach (var meshobj in meshes) {
            // Export mesh first
            var compiledMeshObj = assets.Get<CompiledMeshObject>(meshobj.CompiledMeshObjectUid);
            var modelNode = MeshHelper.ExportLod(meshobj, compiledMeshObj, 0, new());
            MeshHelper.PopulateSkeleton(modelNode, meshobj, skel);

            foreach (var materialUid in meshobj.MaterialUids) {
                var material = assets.Get<Material>(materialUid);
                var materialPath = Path.Combine(materialsPath, $"material_{material.Uid:X}");
                Directory.CreateDirectory(materialPath);
                MaterialHelper.ExportMaterialInfo(assets, material, materialPath);
            }

            var root = new CastNode(CastNodeIdentifier.Root);
            root.AddNode(modelNode);
            CastWriter.Save(Path.Combine(path, $"mesh_{meshobj.Uid:X}.cast"), root);
        }
    }

    private static void ExportMeshAndSkeleton(
        AssetDirectory assets, BuildTable buildTable,
        string path, Dictionary<ulong, Material> materialOverrides) {
        var row = buildTable.Rows.FirstOrDefault();
        if (row == null) {
            throw new Exception("Mesh build table has no rows.");
        }

        List<Skeleton> skel = new();
        List<Mesh> meshes = new();
        foreach (var dynamicProp in row.DynamicProperties) {
            if (dynamicProp.ValueUid == 0) {
                continue;
            }

            var asset = assets.Find(dynamicProp.ValueUid)
                ?? throw new Exception($"Could not find asset with UID {dynamicProp.ValueUid:X} in any container.");

            var assetObj = asset.Data;
            if (assetObj is Skeleton skeleton) {
                skel.Add(skeleton);
            } else if (assetObj is Mesh mesh) {
                meshes.Add(mesh);
            }else if(assetObj is BuildTable childBuildTable) {
                ExportChildBuildTable(assets, childBuildTable, path);
            }
        }

        var materialsPath = Path.Combine(path, "materials");
        if (!Directory.Exists(materialsPath)) {
            Directory.CreateDirectory(materialsPath);
        }

        foreach (var meshobj in meshes) {
            // Export mesh first
            var compiledMeshObj = assets.Get<CompiledMeshObject>(meshobj.CompiledMeshObjectUid);
            var modelNode = MeshHelper.ExportLod(meshobj, compiledMeshObj, 0, materialOverrides);
            MeshHelper.PopulateSkeleton(modelNode, meshobj, skel);

            foreach (var materialUid in meshobj.MaterialUids) {
                var material = materialOverrides.GetValueOrDefault(materialUid) ?? assets.Get<Material>(materialUid);

                var materialPath = Path.Combine(materialsPath, $"material_{material.Uid:X}");
                Directory.CreateDirectory(materialPath);
                MaterialHelper.ExportMaterialInfo(assets, material, materialPath);
            }
            
            var root = new CastNode(CastNodeIdentifier.Root);
            root.AddNode(modelNode);
            CastWriter.Save(Path.Combine(path, $"mesh_{meshobj.Uid:X}.cast"), root);
        }
    }

    public static void ExportCharacterBuildTable(AssetDirectory assets, BuildTable buildTable, string path) {
        path = Path.Join(path, $"buildtable_{buildTable.Uid:X}");
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
        var row = buildTable.Rows.FirstOrDefault();
        if (row == null) {
            throw new Exception("Character build table has no rows.");
        }

        var meshTable = row.DynamicProperties.FirstOrDefault(prop => prop.PropertyId == 4 || prop.PropertyId == 17);
        var materialOverrideTable = row.DynamicProperties.FirstOrDefault(prop => prop.PropertyId == 5 || prop.PropertyId == 18);

        if (meshTable == null) {
            throw new Exception("Character build table is missing mesh table property.");
        }

        if (materialOverrideTable == null) {
            throw new Exception("Character build table is missing material override table property.");
        }

        var materialOverrideBuildTable = assets.Get<BuildTable>(materialOverrideTable.ValueUid);
        var meshBuildTable = assets.Get<BuildTable>(meshTable.ValueUid);

        var materialOverrides = RemapMaterialOverrideBuildTable(assets, materialOverrideBuildTable);
        ExportMeshAndSkeleton(assets, meshBuildTable, path, materialOverrides);
    }
}
