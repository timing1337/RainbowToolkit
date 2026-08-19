using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;


public class ShaderCodeModuleUserMaterial : BaseObject {
    public static readonly uint MAGIC = 0x1c9a0555;
    protected override uint Magic => MAGIC;
    public byte[] Code;
    public override void Parse(FastLoadReader reader) {
        var size = reader.ReadUInt32();
        var buffer1 = reader.ReadBytes((int)size + 1);

        var size2 = reader.ReadUInt32();
        var buffer2 = reader.ReadBytes((int)size2 + 1);
    }
}
