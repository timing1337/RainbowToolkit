using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class ConstraintBoneModifier : BoneModifier {
    public new static readonly uint MAGIC = 0;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        base.Parse(reader);
        var unk1 = reader.ReadUInt32();
        for (int i = 0; i < unk1; i++) {
            reader.ReadNullable();
        }
        var unk2 = reader.ReadByte();
    }
}
