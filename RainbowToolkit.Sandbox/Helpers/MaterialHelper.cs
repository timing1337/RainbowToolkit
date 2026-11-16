using RainbowToolkit.Scimitar;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Sandbox.Helpers;

public class MaterialHelper {

    private static JsonSerializerOptions Options = new JsonSerializerOptions {
        WriteIndented = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static CompiledTextureMapObject? FindCompiledTextureMap(AssetDirectory assets, ulong uid) {
        return assets.Find(uid)?.As<CompiledTextureMapObject>();
    }

    public static CompiledTextureMapObject? GetHighestAvailableMip(AssetDirectory assets, TextureMap textureMap) {
        var pack1 = textureMap.Pack1;
        CompiledTextureMapObject? mip = null;
        if (pack1.FutureResUid != 0 && (mip = FindCompiledTextureMap(assets, pack1.FutureResUid)) != null) {
            return mip;
        }

        if(pack1.UltraResUid != 0 && (mip = FindCompiledTextureMap(assets, pack1.UltraResUid)) != null) {
            return mip;
        }

        if(pack1.HighResUid != 0 && (mip = FindCompiledTextureMap(assets, pack1.HighResUid)) != null) {
            return mip;
        }

        if(pack1.MediumResUid != 0 && (mip = FindCompiledTextureMap(assets, pack1.MediumResUid)) != null) {
            return mip;
        }

        if(pack1.LowResUid != 0 && (mip = FindCompiledTextureMap(assets, pack1.LowResUid)) != null) {
            return mip;
        }

        return mip;
    }

    public static void ExportMaterialInfo(AssetDirectory assets, Material material, string path) {
        if (material.DiffuseMap != null && material.DiffuseMap.TextureBaseUid != 0) ExportTextureSelector(assets, material.DiffuseMap, Path.Join(path, $"diffuse.dds"));
        if (material.SpecularMap != null && material.SpecularMap.TextureBaseUid != 0) ExportTextureSelector(assets, material.SpecularMap, Path.Join(path, $"specular.dds"));
        if (material.NormalMap != null && material.NormalMap.TextureBaseUid != 0) ExportTextureSelector(assets, material.NormalMap, Path.Join(path, $"normal.dds"));
        if(material.ColorMask != null && material.ColorMask.TextureBaseUid != 0) ExportTextureSelector(assets, material.ColorMask, Path.Join(path, $"colormask.dds"));
        if (material.DetailMap != null && material.DetailMap.TextureBaseUid != 0) {
            ExportDetailMap(assets, material.DetailMap, Path.Join(path, $"detail_1.dds"));
        }

        if (material.DetailMap2 != null && material.DetailMap2.TextureBaseUid != 0) {
            ExportDetailMap(assets, material.DetailMap2, Path.Join(path, $"detail_2.dds"));
        }

        if(material.CharacterSkinShaderParams != null && material.CharacterSkinShaderParams.SkinSurfaceScatteringMapUid != 0) {
            ExportTextureSpecFromUid(assets, material.CharacterSkinShaderParams.SkinSurfaceScatteringMapUid, Path.Join(path, $"skin_surface_scattering.dds"));
        }

        if(material.CharacterShaderParams != null) {
            var characterShaderParams = material.CharacterShaderParams;
            if(characterShaderParams.PatternTexture != null && characterShaderParams.PatternTexture.TextureBaseUid != 0) {
                ExportTextureSelector(assets, characterShaderParams.PatternTexture, Path.Join(path, $"pattern.dds"));
            }

            if (characterShaderParams.CamoTexture != null && characterShaderParams.CamoTexture.TextureBaseUid != 0) {
                ExportTextureSelector(assets, characterShaderParams.CamoTexture, Path.Join(path, $"camo.dds"));
            }
        }

        if (material.CustomParams.Count > 0) {
            var names = GetShaderParamNames(assets, material.ShaderTemplateUid);
            var named = names.Count == material.CustomParams.Count;
            material.ShaderParams = new();
            for (var i = 0; i < material.CustomParams.Count; i++) {
                var name = named ? names[i] : $"Param{i}";
                var value = material.CustomParams[i];
                if (value is ulong textureUid && textureUid != 0) {
                    ExportTextureSpecFromUid(assets, textureUid, Path.Join(path, $"{name}.dds"));
                }
                material.ShaderParams[name] = value;
            }
        }

        File.WriteAllText(Path.Join(path, $"material_config.json"), JsonSerializer.Serialize(material, Options));
    }

    // Shader templates embed "#define <Name> UM_CustomParam..." in the same order as Material.CustomParams.
    private static List<string> GetShaderParamNames(AssetDirectory assets, ulong templateUid) {
        var source = assets.Find(templateUid)?.As<ShaderTemplate>()?.Code?.Source ?? "";
        return Regex.Matches(source, @"#define (\w+) \w*CustomParam").Select(m => m.Groups[1].Value).ToList();
    }

    public static void ExportDetailMap(AssetDirectory assets, DetailMapDescriptor descriptor, string path) {
        var textureMapSpec = assets.Get<TextureMapSpec>(descriptor.TextureBaseUid);
        ExportTextureSpec(assets, textureMapSpec, path);
    }

    public static void ExportTextureSpec(AssetDirectory assets, TextureMapSpec spec, string path) {
        var textureMap = assets.Get<TextureMap>(spec.TextureMapUid);
        var compiledTextureMap = GetHighestAvailableMip(assets, textureMap);

        if(compiledTextureMap == null) {
            throw new Exception($"No compiled texture map found for {spec.TextureMapUid}");
        }

        var dds = ImageHelper.ExportHeader(compiledTextureMap.CompiledTextureMap);
        var buffer = compiledTextureMap.CompiledTextureMap.Data.ImageBuffer;

        using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
        fileStream.Write(dds);
        fileStream.Write(buffer);
    }

    public static void ExportTextureSelector(AssetDirectory assets, TextureSelector selector, string path) {
        var textureMapSpec = assets.Get<TextureMapSpec>(selector.TextureBaseUid);
        ExportTextureSpec(assets, textureMapSpec, path);
    }

    public static void ExportTextureSpecFromUid(AssetDirectory assets, ulong textureBaseUid, string path) {
        var textureMapSpec = assets.Get<TextureMapSpec>(textureBaseUid);
        ExportTextureSpec(assets, textureMapSpec, path);
    }
}
