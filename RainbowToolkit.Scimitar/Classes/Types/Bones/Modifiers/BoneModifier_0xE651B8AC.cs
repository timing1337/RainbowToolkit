using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class BoneModifier_0x08D774B2 : BoneModifier {
    public new static readonly uint MAGIC = 0x08D774B2;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        base.Parse(reader);

        var unk3 = reader.ReadNullable();
        var unk4 = reader.ReadNullable();
        var unk5 = reader.ReadStruct<Vector4>(); //probably
        var unk6 = reader.ReadUInt32();
        var unk7 = reader.ReadUInt32();
        var unk8 = reader.ReadUInt32();
        var unk9 = reader.ReadUInt32();
    }
}
