using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;
public class BoneModifier_0x9666EE78 : BaseObject {
    public static readonly uint MAGIC = 0x655CB45B;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        var unk0 = reader.ReadNullable();
        var unk1 = reader.ReadUInt32();
    }
}

