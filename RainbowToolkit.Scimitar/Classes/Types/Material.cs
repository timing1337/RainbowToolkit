
using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Utils;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class Material : BaseObject {
    public static readonly uint MAGIC = 0x9BFBCAA8;
    protected override uint Magic => MAGIC;

    public TextureSelector? DiffuseMap;
    public TextureSelector? NormalMap;
    public TextureSelector? SpecularMap;

    public DetailMapDescriptor? DetailMap;
    public DetailMapDescriptor? DetailMap2;

    // Unknown sections: count * size
    // Unknown object skips: uid + magic + actual size
    public override void Parse(FastLoadReader reader) {
        var shaderTemplateUid = reader.ReadUInt64();

        reader.Advance(16);
        reader.Advance(5 * 1);
        reader.Advance(2 * 4);
        reader.Advance(13 * 1);
        reader.Advance(4);
        reader.Advance(2 * 1);
        reader.Advance(4);

        var ref1Obj = reader.ReadUInt64();
        var collisionMaterialUid = reader.ReadUInt64();

        reader.Advance(8 + 4 + 16); // 0x4C347258
        reader.Advance(8 + 4 + 1); // 0xF1984C39

        reader.Advance(4 * 2);

        var characterShaderParams = reader.ReadNullable();
        var unkObj2 = reader.ReadNullable();

        reader.Advance(4 * 1);
        reader.Advance(2 * 4);
        reader.Advance(1);
        reader.Advance(4);
        reader.Advance(1);
        reader.Advance(2 * 1);
        reader.Advance(16);
        reader.Advance(2 * 4);
        reader.Advance(1);

        var ref2Obj = reader.ReadUInt64();
        reader.Advance(16);
        reader.Advance(4 * 4);

        DiffuseMap = reader.Read<TextureSelector>(); // Diffuse
        NormalMap = reader.Read<TextureSelector>(); // Normal
        SpecularMap = reader.Read<TextureSelector>(); // Specular
        var unk2 = reader.Read<TextureSelector>();
        var unk3 = reader.Read<TextureSelector>();
        var unk4 = reader.Read<UvTransform>();
        reader.Advance(12);
        var referenceUid = reader.ReadUInt64();
        var unk5 = reader.Read<TextureSelector>();
        var unk6 = reader.Read<TextureSelector>();
        var unk7 = reader.Read<TextureSelector>();
        var transform = reader.Read<UvTransform>();
        reader.Advance(12);
        var referenceUid2 = reader.ReadUInt64();
        DetailMap = reader.Read<DetailMapDescriptor>();
        DetailMap2 = reader.Read<DetailMapDescriptor>();
    }
}
