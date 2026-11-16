using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;
public class ShaderCode : BaseObject {
    public static readonly uint MAGIC = 0xFF23BFC4;
    protected override uint Magic => MAGIC;

    public List<ulong> Unk8FD5CB95 = new();
    public uint[] ParamSlots = [];
    public string Source = ""; 

    public override void Parse(FastLoadReader reader) {
        var count = reader.ReadInt32();
        for (var i = 0; i < count; i++) Unk8FD5CB95.Add(reader.ReadUInt64());
        ParamSlots = new uint[reader.ReadInt32()];
        for (var i = 0; i < ParamSlots.Length; i++) ParamSlots[i] = reader.ReadUInt32();
        Source = reader.ReadString();
    }
}
