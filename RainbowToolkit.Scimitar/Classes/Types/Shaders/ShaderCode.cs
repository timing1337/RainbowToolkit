using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class ShaderCode : BaseObject {
    public static readonly uint MAGIC = 0x18E2D3AE;
    protected override uint Magic => MAGIC;
    public byte[] Code;
    public override void Parse(FastLoadReader reader) {
        var size = reader.ReadUInt32();
        var buffer = reader.ReadBytes((int)size + 1);

        var count1 = reader.ReadUInt32();
        for (int i = 0; i < count1; i++) {
           reader.ReadUInt32();
        }

        var count2 = reader.ReadUInt32();
        for (int i = 0; i < count2; i++) {
            var unk = reader.ReadUInt64(); //ShaderCodeModuleBase
        }
    }
}
