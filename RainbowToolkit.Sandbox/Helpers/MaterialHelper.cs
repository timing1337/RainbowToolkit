using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Container;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Sandbox.Helpers;

public class MaterialHelper {

    private static JsonSerializerOptions Options = new JsonSerializerOptions {
        WriteIndented = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static CompiledTextureMapObject? FindCompiledTextureMap(ulong uid) {
        var container = ScimitarManager.Instance.FindAssetContainer(uid);
        if (container != null) {
            return container.ReadAsset(uid).As<CompiledTextureMapObject>()!;
        }

        return null;
    }

    public static CompiledTextureMapObject? GetHighestAvailableMip(TextureMap textureMap) {
        var pack1 = textureMap.Pack1;
        CompiledTextureMapObject? mip = null;
        if (pack1.FutureResUid != 0 && (mip = FindCompiledTextureMap(pack1.FutureResUid)) != null) {
            return mip;
        }

        if(pack1.UltraResUid != 0 && (mip = FindCompiledTextureMap(pack1.UltraResUid)) != null) {
            return mip;
        }

        if(pack1.HighResUid != 0 && (mip = FindCompiledTextureMap(pack1.HighResUid)) != null) {
            return mip;
        }

        if(pack1.MediumResUid != 0 && (mip = FindCompiledTextureMap(pack1.MediumResUid)) != null) {
            return mip;
        }

        if(pack1.LowResUid != 0 && (mip = FindCompiledTextureMap(pack1.LowResUid)) != null) {
            return mip;
        }

        return mip;
    }

    public static void ExportMaterialInfo(AssetContainer container, Material material, string path) {
        if (material.DiffuseMap != null && material.DiffuseMap.TextureBaseUid != 0) ExportTextureSelector(container, material.DiffuseMap, Path.Join(path, $"diffuse.dds"));
        if (material.SpecularMap != null && material.SpecularMap.TextureBaseUid != 0) ExportTextureSelector(container, material.SpecularMap, Path.Join(path, $"specular.dds"));
        if (material.NormalMap != null && material.NormalMap.TextureBaseUid != 0) ExportTextureSelector(container, material.NormalMap, Path.Join(path, $"normal.dds"));

        if (material.DetailMap != null && material.DetailMap.TextureBaseUid != 0) {
            ExportDetailMap(container, material.DetailMap, Path.Join(path, $"detail_1.dds"));
        }

        if (material.DetailMap2 != null && material.DetailMap2.TextureBaseUid != 0) {
            ExportDetailMap(container, material.DetailMap2, Path.Join(path, $"detail_2.dds"));
        }

        if(material.CharacterSkinShaderParams != null && material.CharacterSkinShaderParams.SkinSurfaceScatteringMapUid != 0) {
            ExportTextureSpecFromUid(container, material.CharacterSkinShaderParams.SkinSurfaceScatteringMapUid, Path.Join(path, $"skin_surface_scattering.dds"));
        }

        File.WriteAllText(Path.Join(path, $"material_config.json"), JsonSerializer.Serialize(material, Options));
    }

    public static void ExportDetailMap(AssetContainer container, DetailMapDescriptor descriptor, string path) {
        var textureMapSpec = container.ReadAsset(descriptor.TextureBaseUid).As<TextureMapSpec>()!;
        ExportTextureSpec(container, textureMapSpec, path);
    }

    public static void ExportTextureSpec(AssetContainer container, TextureMapSpec spec, string path) {
        var textureMap = container.ReadAsset(spec.TextureMapUid).As<TextureMap>()!;
        var compiledTextureMap = GetHighestAvailableMip(textureMap);

        if(compiledTextureMap == null) {
            throw new Exception($"No compiled texture map found for {spec.TextureMapUid}");
        }

        var dds = ImageHelper.ExportHeader(compiledTextureMap.CompiledTextureMap);
        var buffer = compiledTextureMap.CompiledTextureMap.Data.ImageBuffer;

        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        fileStream.Write(dds);
        fileStream.Write(buffer);
    }

    public static void ExportTextureSelector(AssetContainer container, TextureSelector selector, string path) {
        var textureMapSpec = container.ReadAsset(selector.TextureBaseUid).As<TextureMapSpec>()!;
        ExportTextureSpec(container, textureMapSpec, path);
    }

    public static void ExportTextureSpecFromUid(AssetContainer container, ulong textureBaseUid, string path) {
        var textureMapSpec = container.ReadAsset(textureBaseUid).As<TextureMapSpec>()!;
        ExportTextureSpec(container, textureMapSpec, path);
    }
}
