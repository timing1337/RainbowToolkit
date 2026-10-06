using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class CharacterSkinShaderParams : BaseObject {
    public static readonly uint MAGIC = 0xF6348091;
    protected override uint Magic => MAGIC;

    [JsonIgnore]
    public ulong SkinSurfaceScatteringMapUid;

    public float Unk0;
    public uint Unk1;
    public float Unk2;

    public override void Parse(FastLoadReader reader) {
        var unk1 = reader.ReadUInt64(); // ?
        var unk2 = reader.ReadUInt64(); // ?
        var unk3 = reader.ReadUInt64(); // ?
        var unk4 = reader.ReadUInt64(); // ? 
        SkinSurfaceScatteringMapUid = reader.ReadUInt64(); // SkinSurfaceScatteringMap
        var unk6 = reader.ReadUInt64(); // ?

        Unk0 = reader.ReadSingle();
        Unk1 = reader.ReadUInt32();
        Unk2 = reader.ReadSingle();
    }
}
