using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class CharacterSkinShaderParams : BaseObject {
    public static readonly uint MAGIC = 0x85A2678A;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var unk1 = reader.ReadUInt64(); // ?
        var unk2 = reader.ReadUInt64(); // ?
        var unk3 = reader.ReadUInt64(); // ?
        var unk4 = reader.ReadUInt64(); // ? 
        var unk5 = reader.ReadUInt64(); // SkinSurfaceScatteringMap
        var unk6 = reader.ReadUInt64(); // ?

        var unk7 = reader.ReadSingle();
        var unk8 = reader.ReadUInt32();
        var unk9 = reader.ReadSingle();
    }
}
