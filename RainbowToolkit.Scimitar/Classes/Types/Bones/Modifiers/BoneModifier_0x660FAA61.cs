using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones.Modifiers;
public class BoneModifier_0x660FAA61 : BoneModifier_Unk1 {
    public static readonly uint MAGIC = 0x90638D6B;
    protected override uint Magic => MAGIC;
    public override void Parse(FastLoadReader reader) {
        base.Parse(reader);
        reader.BaseStream.Seek(16, SeekOrigin.Current);
    }
}

