using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class BoneInitialTransforms : BaseObject {
    public static readonly uint MAGIC = 0x710755B9;
    protected override uint Magic => MAGIC;

    public Matrix4x4 Transform;

    public override void Parse(FastLoadReader reader) {
        Transform = reader.ReadStruct<Matrix4x4>();
    }
}
