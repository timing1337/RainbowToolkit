using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones;

public abstract class BoneModifier : BaseObject {
    public static readonly uint MAGIC = 0x8A901914;
    protected override uint Magic => MAGIC;
}
