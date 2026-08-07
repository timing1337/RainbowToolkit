using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Container;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Sandbox.Helpers;

public class MaterialHelper {

    public static void ExportMaterialInfo(AssetContainer container, Material material, string path) {
        if (material.DiffuseMap != null) ExportTextureSelector(container, material.DiffuseMap, Path.Join(path, $"diffuse.dds"));
        if (material.SpecularMap != null) ExportTextureSelector(container, material.SpecularMap, Path.Join(path, $"specular.dds"));
        if (material.NormalMap != null) ExportTextureSelector(container, material.NormalMap, Path.Join(path, $"normal.dds"));
        if (material.DetailMap != null && material.DetailMap.TextureBaseUid != 0) ExportDetailMap(container, material.DetailMap, Path.Join(path, $"detail_{material.DetailMap.Scale.X}_{material.DetailMap.Scale.Y}.dds"));
        if (material.DetailMap2 != null && material.DetailMap2.TextureBaseUid != 0) ExportDetailMap(container, material.DetailMap2, Path.Join(path, $"detail_{material.DetailMap2.Scale.X}_{material.DetailMap2.Scale.Y}.dds"));
    }

    public static void ExportDetailMap(AssetContainer container, DetailMapDescriptor descriptor, string path) {
        var textureMapSpec = container.ReadAsset(descriptor.TextureBaseUid).As<TextureMapSpec>()!;
        Console.WriteLine("UID: " + descriptor.TextureBaseUid.ToString("X"));
        Console.WriteLine("Scale: " + descriptor.Scale);
        ExportTextureSpec(container, textureMapSpec, path);
    }

    public static void ExportTextureSpec(AssetContainer container, TextureMapSpec spec, string path) {
        var textureMap = container.ReadAsset(spec.TextureMapUid).As<TextureMap>()!;
        var highestMip = textureMap.Pack1.GetHighestAvailableUid();
        var compiled = ScimitarManager.Instance.FindAssetContainer(highestMip);
        if (compiled == null) {
            throw new Exception("Compiled texture map not found for highest mip level.");
        }
        var compiledTextureMap = compiled.ReadAsset(highestMip).As<CompiledTextureMapObject>()!;

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
}
