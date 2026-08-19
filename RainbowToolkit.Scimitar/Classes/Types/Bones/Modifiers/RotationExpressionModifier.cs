using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class RotationExpressionModifier : BoneModifier {
    public new static readonly uint MAGIC = 0xf465cc0f;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        base.Parse(reader);
        var unk3 = reader.ReadUInt32();
        var unk4 = reader.ReadNullable();
        var unk5 = reader.ReadUInt32();
        var unk6 = reader.ReadUInt32();
        var unk7 = reader.ReadNullable();
        var unk8 = reader.ReadUInt32();
        var unk9 = reader.ReadUInt32();

        for (int i = 0; i < unk9; i++) {
            reader.ReadNullable();
        }
        var unk10 = reader.ReadUInt32();
        for (int i = 0; i < unk10; i++) {
            reader.ReadNullable();
        }
    }
}
