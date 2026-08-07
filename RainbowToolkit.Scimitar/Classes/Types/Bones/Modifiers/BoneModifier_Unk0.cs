using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class BoneModifier_Unk0 : BaseObject {
    public static readonly uint MAGIC = 0;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        var unk0 = reader.ReadNullable();
        var unk1 = reader.ReadByte();
        var unk2 = reader.ReadByte();
    }
}
