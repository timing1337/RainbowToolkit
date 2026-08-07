using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class OverrideDefinition : BaseObject {
    public static readonly uint MAGIC = 0x9E095329;
    protected override uint Magic => MAGIC;

    public ulong MaterialToReplace;
    public ulong NewMaterial;

    public override void Parse(FastLoadReader reader) {
        MaterialToReplace = reader.ReadUInt64();
        NewMaterial = reader.ReadUInt64();
    }
}
