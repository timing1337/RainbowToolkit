using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;

public class BoneModifier_0xA4F64286 : BoneModifier_Unk0 {
    public new static readonly uint MAGIC = 0xB0DDD122;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        base.Parse(reader);
        var unk4 = reader.ReadUInt32();
        var unk5 = reader.ReadUInt32();
        var unk6 = reader.ReadUInt32();

        var unk7 = reader.ReadUInt32();
        var unk8 = reader.ReadUInt32();
        var unk9 = reader.ReadUInt32();

        var unk10 = reader.ReadUInt32();
        var unk11 = reader.ReadUInt32();
        var unk12 = reader.ReadUInt32();

        var unk13 = reader.ReadUInt32();
        var unk14 = reader.ReadUInt32();
        var unk15 = reader.ReadUInt32();

        var unk16 = reader.ReadStruct<Vector4>();
        var unk17 = reader.ReadUInt32();
        var unk18 = reader.ReadUInt32();

        var unk19 = reader.ReadUInt32();
        var unk20 = reader.ReadUInt64(); // reference uid
        var unk21 = reader.ReadNullable();
        var unk22 = reader.ReadUInt64(); //reference
    }
}
