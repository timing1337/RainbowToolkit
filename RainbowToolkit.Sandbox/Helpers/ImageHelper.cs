using DirectXTex;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Sandbox.Helpers;


public static class ImageHelper {
    public static byte[] GenerateDDSHeader(CompiledTextureMap textureMap, string path) {
        var textureData = textureMap.Data;

        var width = textureData.Width >> textureData.FirstMip;
        var height = textureData.Height >> textureData.FirstMip;
        var mipCount = textureData.NbMipMaps;

        var format = textureMap.PixelFormat switch {
            PixelFormat.PixelFormat_RGBA8888 => DirectXTexUtility.DXGIFormat.R8G8B8A8UNORM,
            PixelFormat.PixelFormat_RGBA8888Signed => DirectXTexUtility.DXGIFormat.R8G8B8A8SNORM,
            PixelFormat.PixelFormat_BC1 => DirectXTexUtility.DXGIFormat.BC1UNORM,
            PixelFormat.PixelFormat_BC1A => DirectXTexUtility.DXGIFormat.BC1UNORM,
            PixelFormat.PixelFormat_BC2 => DirectXTexUtility.DXGIFormat.BC2UNORM,
            PixelFormat.PixelFormat_BC3 => DirectXTexUtility.DXGIFormat.BC3UNORM,
            PixelFormat.PixelFormat_BC4 => DirectXTexUtility.DXGIFormat.BC4UNORM,
            PixelFormat.PixelFormat_BC5 => DirectXTexUtility.DXGIFormat.BC5UNORM,
            PixelFormat.PixelFormat_BC6 => DirectXTexUtility.DXGIFormat.BC6HUF16,
            PixelFormat.PixelFormat_BC7 => DirectXTexUtility.DXGIFormat.BC7UNORM,
            PixelFormat.PixelFormat_A8 => DirectXTexUtility.DXGIFormat.A8UNORM,
            PixelFormat.PixelFormat_I8 => DirectXTexUtility.DXGIFormat.R8UNORM,
            PixelFormat.PixelFormat_I16 => DirectXTexUtility.DXGIFormat.R16UNORM,
            PixelFormat.PixelFormat_A8I8 => DirectXTexUtility.DXGIFormat.R8G8UNORM,
            PixelFormat.PixelFormat_R32F => DirectXTexUtility.DXGIFormat.R32FLOAT,
            PixelFormat.PixelFormat_RGBA32F => DirectXTexUtility.DXGIFormat.R32G32B32A32FLOAT,
            PixelFormat.PixelFormat_RGBA16F => DirectXTexUtility.DXGIFormat.R16G16B16A16FLOAT,
            _ => throw new NotSupportedException($"Unsupported pixel format: {textureMap.PixelFormat}"),
        };

        var metadata = DirectXTexUtility.GenerateMetaData(width, height, mipCount, format, false);
        DirectXTexUtility.GenerateDDSHeader(metadata, DirectXTexUtility.DDSFlags.NONE, out var header, out var dx10Header);
        return DirectXTexUtility.EncodeDDSHeader(header, dx10Header);
    }
}
