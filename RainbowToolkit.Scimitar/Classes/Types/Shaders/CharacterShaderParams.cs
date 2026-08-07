using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Shaders;

public class CharacterShaderParams : BaseObject {
    public static readonly uint MAGIC = 0xf2ce7e39;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var texture = reader.Read<TextureSelector>();
        reader.Advance(16 * 2);
        reader.Advance(4 * 2);
        var texture2 = reader.Read<TextureSelector>();
        reader.Advance(16 * 5);
        reader.Advance(4 * 3);
    }
}
