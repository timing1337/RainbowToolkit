
using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Classes.Types.Shaders;
using RainbowToolkit.Scimitar.Utils;
using System.Numerics;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class Material : BaseObject {
    public static readonly uint MAGIC = 0x505A2021;
    protected override uint Magic => MAGIC;

    public ulong ShaderTemplateUid;
    public Vector4 DiffuseColor;
    public ulong Flags;
    public ulong BaseMaterialUid;
    public ulong CollisionMaterialUid;
    public Unk5051EF33? Unk1CE51899;
    public Mask? Mask;
    public uint Unk89ECFA7B; // enum
    public float Unk10A240EE;

    public CharacterShaderParams? CharacterShaderParams;
    public CharacterSkinShaderParams? CharacterSkinShaderParams;
    public HairShaderParams? HairShaderParams;

    public UnkC98D2D78? Unk8C12CA3A;
    public bool UnkCBEDD77B;
    public byte UnkF406CE3A, Unk71D2B6CE, Unk165F8539;
    public float UnkFBD5FE26, Unk75620B88;
    public bool Unk6FC50020;
    public float Unk06EC5566;
    public bool Unk876FB3DE, Unk14FE94F1, Unk7D00D22E;
    public Vector4 Unk6A254658;
    public float Unk9A4ED1C9, Unk018DD881;
    public bool UnkBB199A2F;
    public ulong Unk8C7F46A6; // TextureMapSpec handle
    public Vector4 Unk94E3357D;
    public float Unk42B1C353, UnkBC73EB31, Unk08DF1A27, UnkCBFF367D;

    [JsonIgnore] public TextureSelector? DiffuseMap, NormalMap, SpecularMap, Unk99778BBA, ColorMask;
    public UvTransform? UvTransform;
    public uint Unk0FDFD1E4; // enum
    public Vector2 UnkB0E7B2EE;
    public ulong Unk2FF947AF; // TextureMapSpec handle

    [JsonIgnore] public TextureSelector? Unk4A586A5C, Unk4A8878E0, Unk3D5BC489;
    public UvTransform? UvTransform2;
    public uint Unk86D7213E; // enum
    public Vector2 Unk29EE8434;
    public ulong UnkA6F1F769; // TextureMapSpec handle

    public DetailMapDescriptor? DetailMap, DetailMap2;
    public bool Unk0B5D0778;
    public uint Unk93C6C167; // enum
    public float Unk868679E5, UnkA3CCD24B, UnkA838AD4B, Unk8E7B3D08;
    public bool Unk826F1C64;
    public float UnkAF8F5DF7, Unk685D37FF;
    public bool Unk72335D2C;
    public uint Unk547B927D; // enum
    public float UnkDBCD3EDF, Unk0936B06D, UnkAE9AD597;
    public bool Unk2B4204D8;
    [JsonIgnore] public TextureSelector? UnkD5B58247, Unk83DDB0AF;
    public bool UnkB5CB16F9, UnkFE2EC280, Unk026CC00D, Unk838CC486, Unk9942A8CC, UnkCF75731B, Unk16972DDF;
    public string UnkB5C48F6D = "";
    public List<object?> Unk9C9734C4 = new();
    public ulong[] Unk3E2E7B46 = new ulong[4];
    public ulong Unk8E09F505;
    public ulong[] Unk13BA4B45 = new ulong[4]; // TextureMapSpec handles
    public ulong Unk1231A0B9;
    public Vector4[] Unk70D8BC59 = new Vector4[4];
    public uint[] Unk196A8B4C = new uint[16];
    public Vector4 Unk40879C98, UnkC9BAC11E;
    public bool Unk7A61B79F, UnkE36886F9, Unk1AE9624B, Unk23647588;

    [JsonIgnore] public List<object> CustomParams = new();
    public Dictionary<string, object>? ShaderParams;

    public override void Parse(FastLoadReader reader) {
        ShaderTemplateUid = reader.ReadUInt64();
        DiffuseColor = reader.ReadStruct<Vector4>();

        for (var i = 0; i < 5; i++) Flags = reader.ReadBits(Flags, i);
        Flags = reader.ReadBits(Flags, 5, 4, true);
        Flags = reader.ReadBits(Flags, 9, 2, true);
        Flags = reader.ReadBits(Flags, 11);
        Flags = reader.ReadBits(Flags, 12, 8);
        for (var i = 20; i < 31; i++) Flags = reader.ReadBits(Flags, i);
        Flags = reader.ReadBits(Flags, 31, 4, true);
        Flags = reader.ReadBits(Flags, 35);
        Flags = reader.ReadBits(Flags, 36);
        Flags = reader.ReadBits(Flags, 37, 4, true);

        BaseMaterialUid = reader.ReadUInt64();
        CollisionMaterialUid = reader.ReadUInt64();
        Unk1CE51899 = reader.Read<Unk5051EF33>();
        Mask = reader.Read<Mask>();
        Unk89ECFA7B = reader.ReadUInt32();
        Unk10A240EE = reader.ReadSingle();

        switch (reader.ReadNullable()) {
            case CharacterShaderParams p: CharacterShaderParams = p; break;
            case CharacterSkinShaderParams p: CharacterSkinShaderParams = p; break;
            case HairShaderParams p: HairShaderParams = p; break;
        }
        Unk8C12CA3A = reader.ReadNullable() as UnkC98D2D78;

        UnkCBEDD77B = reader.ReadBoolean();
        UnkF406CE3A = reader.ReadByte();
        Unk71D2B6CE = reader.ReadByte();
        Unk165F8539 = reader.ReadByte();
        UnkFBD5FE26 = reader.ReadSingle();
        Unk75620B88 = reader.ReadSingle();
        Unk6FC50020 = reader.ReadBoolean();
        Unk06EC5566 = reader.ReadSingle();
        Unk876FB3DE = reader.ReadBoolean();
        Unk14FE94F1 = reader.ReadBoolean();
        Unk7D00D22E = reader.ReadBoolean();
        Unk6A254658 = reader.ReadStruct<Vector4>();
        Unk9A4ED1C9 = reader.ReadSingle();
        Unk018DD881 = reader.ReadSingle();
        UnkBB199A2F = reader.ReadBoolean();
        Unk8C7F46A6 = reader.ReadUInt64();
        Unk94E3357D = reader.ReadStruct<Vector4>();
        Unk42B1C353 = reader.ReadSingle();
        UnkBC73EB31 = reader.ReadSingle();
        Unk08DF1A27 = reader.ReadSingle();
        UnkCBFF367D = reader.ReadSingle();

        DiffuseMap = reader.Read<TextureSelector>();
        NormalMap = reader.Read<TextureSelector>();
        SpecularMap = reader.Read<TextureSelector>();
        Unk99778BBA = reader.Read<TextureSelector>();
        ColorMask = reader.Read<TextureSelector>();
        UvTransform = reader.Read<UvTransform>();
        Unk0FDFD1E4 = reader.ReadUInt32();
        UnkB0E7B2EE = reader.ReadStruct<Vector2>();
        Unk2FF947AF = reader.ReadUInt64();

        Unk4A586A5C = reader.Read<TextureSelector>();
        Unk4A8878E0 = reader.Read<TextureSelector>();
        Unk3D5BC489 = reader.Read<TextureSelector>();
        UvTransform2 = reader.Read<UvTransform>();
        Unk86D7213E = reader.ReadUInt32();
        Unk29EE8434 = reader.ReadStruct<Vector2>();
        UnkA6F1F769 = reader.ReadUInt64();

        DetailMap = reader.Read<DetailMapDescriptor>();
        DetailMap2 = reader.Read<DetailMapDescriptor>();
        Unk0B5D0778 = reader.ReadBoolean();
        Unk93C6C167 = reader.ReadUInt32();
        Unk868679E5 = reader.ReadSingle();
        UnkA3CCD24B = reader.ReadSingle();
        UnkA838AD4B = reader.ReadSingle();
        Unk8E7B3D08 = reader.ReadSingle();
        Unk826F1C64 = reader.ReadBoolean();
        UnkAF8F5DF7 = reader.ReadSingle();
        Unk685D37FF = reader.ReadSingle();
        Unk72335D2C = reader.ReadBoolean();
        Unk547B927D = reader.ReadUInt32();
        UnkDBCD3EDF = reader.ReadSingle();
        Unk0936B06D = reader.ReadSingle();
        UnkAE9AD597 = reader.ReadSingle();
        Unk2B4204D8 = reader.ReadBoolean();
        UnkD5B58247 = reader.Read<TextureSelector>();
        Unk83DDB0AF = reader.Read<TextureSelector>();
        UnkB5CB16F9 = reader.ReadBoolean();
        UnkFE2EC280 = reader.ReadBoolean();
        Unk026CC00D = reader.ReadBoolean();
        Unk838CC486 = reader.ReadBoolean();
        Unk9942A8CC = reader.ReadBoolean();
        UnkCF75731B = reader.ReadBoolean();
        Unk16972DDF = reader.ReadBoolean();
        UnkB5C48F6D = reader.ReadString();

        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++) Unk9C9734C4.Add(reader.ReadNullable());
        for (var i = 0; i < 4; i++) Unk3E2E7B46[i] = reader.ReadUInt64();
        Unk8E09F505 = reader.ReadUInt64();
        for (var i = 0; i < 4; i++) Unk13BA4B45[i] = reader.ReadUInt64();
        Unk1231A0B9 = reader.ReadUInt64();
        for (var i = 0; i < 4; i++) Unk70D8BC59[i] = reader.ReadStruct<Vector4>();
        for (var i = 0; i < 16; i++) Unk196A8B4C[i] = reader.ReadUInt32();
        Unk40879C98 = reader.ReadStruct<Vector4>();
        UnkC9BAC11E = reader.ReadStruct<Vector4>();
        Unk7A61B79F = reader.ReadBoolean();
        UnkE36886F9 = reader.ReadBoolean();
        Unk1AE9624B = reader.ReadBoolean();
        Unk23647588 = reader.ReadBoolean();

        count = reader.ReadInt32();
        for (var i = 0; i < count; i++) {
            var paramId = reader.ReadUInt32(); // param id
            var type = (reader.ReadUInt64() >> 48) & 0x7F;
            var flags = reader.ReadUInt32(); // flags
            CustomParams.Add(type switch {
                0x0A => (object)reader.ReadSingle(),
                0x0D => reader.ReadStruct<Vector4>(),
                0x1C => (object)reader.ReadUInt64(), // TextureMapSpec uid
                _ => throw new Exception($"Unknown custom param type {type:X2}")
            });
        }
    }
}

public class Unk5051EF33 : BaseObject {
    public static readonly uint MAGIC = 0x5051EF33;
    protected override uint Magic => MAGIC;

    public ushort Flags;
    
    public override void Parse(FastLoadReader reader) {
        for (var i = 0; i < 16; i++) Flags = (ushort)reader.ReadBits(Flags, i);
    }
}

public class UnkC98D2D78 : BaseObject {
    public static readonly uint MAGIC = 0xC98D2D78;
    protected override uint Magic => MAGIC;

    public bool UnkF1C23DAE;
    public float UnkF4227C41, Unk2903180E;
    public bool Unk88064BF8;
    public uint UnkC1266747; // enum
    public bool UnkBF73E16E;
    public float Unk87B88539;
    public bool UnkCE6D1CA4, Unk219817C2;
    public float Unk09748C79;
    public Vector4 UnkFF1666F6, UnkE53C47F0;

    public override void Parse(FastLoadReader reader) {
        UnkF1C23DAE = reader.ReadBoolean();
        UnkF4227C41 = reader.ReadSingle();
        Unk2903180E = reader.ReadSingle();
        Unk88064BF8 = reader.ReadBoolean();
        UnkC1266747 = reader.ReadUInt32();
        UnkBF73E16E = reader.ReadBoolean();
        Unk87B88539 = reader.ReadSingle();
        UnkCE6D1CA4 = reader.ReadBoolean();
        Unk219817C2 = reader.ReadBoolean();
        Unk09748C79 = reader.ReadSingle();
        UnkFF1666F6 = reader.ReadStruct<Vector4>();
        UnkE53C47F0 = reader.ReadStruct<Vector4>();
    }
}

public class Unk092D606E : BaseObject {
    public static readonly uint MAGIC = 0x092D606E;
    protected override uint Magic => MAGIC;

    public ulong MaterialUid; // 36E04A07
    public ulong Unk7481E6B2, Unk50FCADE0; // 01031DF9 / 3C3DEFF7 handles
    public float Unk2DD10816;
    public Vector4 Unk4A72EABC, Unk3A83B61B, Unk5D0C4A68, Unk75A9C5CA;
    public ulong Unk152C49A1; // 4A27C837 handle
    public float UnkA099388E, Unk39920A14;
    public bool Unk57D6B76C;

    public override void Parse(FastLoadReader reader) {
        MaterialUid = reader.ReadUInt64();
        Unk7481E6B2 = reader.ReadUInt64();
        Unk50FCADE0 = reader.ReadUInt64();
        Unk2DD10816 = reader.ReadSingle();
        Unk4A72EABC = reader.ReadStruct<Vector4>();
        Unk3A83B61B = reader.ReadStruct<Vector4>();
        Unk5D0C4A68 = reader.ReadStruct<Vector4>();
        Unk75A9C5CA = reader.ReadStruct<Vector4>();
        Unk152C49A1 = reader.ReadUInt64();
        UnkA099388E = reader.ReadSingle();
        Unk39920A14 = reader.ReadSingle();
        Unk57D6B76C = reader.ReadBoolean();
    }
}
