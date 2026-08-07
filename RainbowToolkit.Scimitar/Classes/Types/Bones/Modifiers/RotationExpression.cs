using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class RotationExpression : BaseObject {
    public static readonly uint MAGIC = 0x2B992BB7;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        var unk0 = reader.ReadUInt32();
        var unk1 = reader.ReadUInt32();
        var unk2 = reader.ReadUInt32();
        var unk3 = reader.ReadUInt32();
        var unk4 = reader.ReadByte();
    }
}
