
using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Utils;
using System.Drawing;
using System.Numerics;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class Material : BaseObject {
    public static readonly uint MAGIC = 0x9BFBCAA8;
    protected override uint Magic => MAGIC;

    public TextureSelector? DiffuseMap;
    public TextureSelector? NormalMap;
    public TextureSelector? SpecularMap;

    public DetailMapDescriptor? DetailMap;
    public DetailMapDescriptor? DetailMap2;

    public ulong ShaderTemplateUid;
    public Vector4 DiffuseColor;

    // Unknown sections: count * size
    // Unknown object skips: uid + magic + actual size
    public override void Parse(FastLoadReader reader) {
        ShaderTemplateUid = reader.ReadUInt64();

        DiffuseColor = reader.ReadStruct<Vector4>();
        reader.Advance(5 * 1);
        reader.Advance(2 * 4);
        reader.Advance(13 * 1);
        reader.Advance(4);
        reader.Advance(2 * 1);
        reader.Advance(4);

        var unkObjUid = reader.ReadUInt64();
        var collisionMaterialUid = reader.ReadUInt64();

        reader.Advance(8 + 4 + 16); // Some object here...
        reader.Advance(8 + 4 + 1);

        reader.Advance(4 * 2);

        var characterShaderParams = reader.ReadNullable();
        var unkObj0 = reader.ReadNullable();

        reader.Advance(4 * 1);
        reader.Advance(2 * 4);
        reader.Advance(1);
        reader.Advance(4);
        reader.Advance(1);
        reader.Advance(2 * 1);
        var unk0 = reader.ReadStruct<Vector4>();
        reader.Advance(2 * 4);
        reader.Advance(1);
        var ref2Obj = reader.ReadUInt64();
        var unk1 = reader.ReadStruct<Vector4>();
        reader.Advance(4 * 4);

        DiffuseMap = reader.Read<TextureSelector>(); // Diffuse
        NormalMap = reader.Read<TextureSelector>(); // Normal
        SpecularMap = reader.Read<TextureSelector>(); // Specular
        var unk2 = reader.Read<TextureSelector>();
        var unk3 = reader.Read<TextureSelector>();
        var unk4 = reader.Read<UvTransform>();
        var unk5 = reader.ReadUInt32();
        var unk6 = reader.ReadStruct<Vector2>();
        var referenceUid = reader.ReadUInt64();


        var unk7 = reader.Read<TextureSelector>();
        var unk8 = reader.Read<TextureSelector>();
        var unk9 = reader.Read<TextureSelector>();
        var unk10 = reader.Read<UvTransform>();
        var unk11 = reader.ReadUInt32();
        var unk12 = reader.ReadStruct<Vector2>();

        var referenceUid2 = reader.ReadUInt64();
        DetailMap = reader.Read<DetailMapDescriptor>();
        DetailMap2 = reader.Read<DetailMapDescriptor>();

        reader.Advance(1);
        reader.ReadUInt32();
        reader.ReadSingle();
        reader.ReadUInt32();
        reader.ReadSingle();
        reader.ReadSingle();
        reader.Advance(1);
        reader.ReadSingle();
        reader.ReadSingle();
        reader.Advance(1);
        reader.ReadSingle();
        reader.ReadSingle();
        reader.ReadSingle();
        reader.ReadSingle();
        reader.Advance(1);

        var unk13 = reader.Read<TextureSelector>();
        var unk14 = reader.Read<TextureSelector>();

        reader.Advance(1);
        reader.Advance(1);
        reader.Advance(1);
        reader.Advance(1);
        reader.Advance(1);
        reader.Advance(1);
        reader.Advance(1);

        var unk15 = reader.ReadUInt32();
        Console.WriteLine("unk15: " + unk15);
    }
}
