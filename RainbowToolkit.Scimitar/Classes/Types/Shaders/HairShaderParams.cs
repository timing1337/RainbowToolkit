using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class HairShaderParams : BaseObject {
    public static readonly uint MAGIC = 0xF6348091;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var unk0 = reader.ReadSingle();
        var unk1 = reader.ReadSingle();
        var unk2 = reader.ReadSingle();
        var unk3 = reader.ReadSingle();
        var unk4 = reader.ReadSingle();
        var unk5 = reader.ReadSingle();
        var unk6 = reader.ReadSingle();
        var unk7 = reader.ReadSingle();
        var unk8 = reader.ReadSingle();
    }
}
