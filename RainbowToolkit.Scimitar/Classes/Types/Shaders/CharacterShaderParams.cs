using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class CharacterShaderParams : BaseObject {
    public static readonly uint MAGIC = 0xf2ce7e39;
    protected override uint Magic => MAGIC;

    [JsonIgnore] public TextureSelector PatternTexture;
    public Vector4 PatternTintA;
    public Vector4 PatternTintB;
    public Vector2 PatternUVScale;

    [JsonIgnore] public TextureSelector CamoTexture;
    public Vector4 DyeBaseColor;
    public Vector4 DyeRedColor;
    public Vector4 DyeGreenColor;
    public Vector4 DyeBlueColor;

    public Vector4 FlatTint;
    public Vector3 Unk8;

    public override void Parse(FastLoadReader reader) {
        PatternTexture = reader.Read<TextureSelector>();
        PatternTintA = reader.ReadStruct<Vector4>();
        PatternTintB = reader.ReadStruct<Vector4>();
        PatternUVScale = reader.ReadStruct<Vector2>();

        CamoTexture = reader.Read<TextureSelector>();
        DyeBaseColor = reader.ReadStruct<Vector4>();
        DyeRedColor = reader.ReadStruct<Vector4>();
        DyeGreenColor = reader.ReadStruct<Vector4>();
        DyeBlueColor = reader.ReadStruct<Vector4>();

        FlatTint = reader.ReadStruct<Vector4>();
        Unk8 = reader.ReadStruct<Vector3>();
    }
}
