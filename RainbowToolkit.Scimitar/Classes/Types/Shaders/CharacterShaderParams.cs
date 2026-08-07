using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class CharacterShaderParams : BaseObject {
    public static readonly uint MAGIC = 0xf2ce7e39;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var patternTexture = reader.Read<TextureSelector>();
        var patternTintA = reader.ReadStruct<Vector4>();
        var patternTintB = reader.ReadStruct<Vector4>();
        var patternUVScale = reader.ReadStruct<Vector2>();

        var dyeMaskTexture = reader.Read<TextureSelector>();
        var dyeBaseColor = reader.ReadStruct<Vector4>();
        var dyeRedColor = reader.ReadStruct<Vector4>();
        var dyeGreenColor = reader.ReadStruct<Vector4>();
        var dyeBlueColor = reader.ReadStruct<Vector4>();

        var flatTint = reader.ReadStruct<Vector4>();
        var unk8 = reader.ReadStruct<Vector3>();
    }
}
