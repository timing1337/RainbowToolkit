using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class FacialPoseGroup : BaseObject {
    public static readonly uint MAGIC = 0x8063DF97;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var count = reader.ReadUInt32();
        for (int i = 0; i < count; i++) {
            reader.ReadNullable();
        }
    }
}
