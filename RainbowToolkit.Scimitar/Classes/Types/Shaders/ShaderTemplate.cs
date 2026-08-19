using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class ShaderTemplate : BaseObject {
    public static readonly uint MAGIC = 0x6f74ddb4;
    protected override uint Magic => MAGIC;


    public override void Parse(FastLoadReader reader) {
        var shaderCode = reader.ReadObject();
        var unk0 = reader.ReadByte();
    }
}
