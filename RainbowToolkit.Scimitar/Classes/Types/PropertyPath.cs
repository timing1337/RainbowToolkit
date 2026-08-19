using RainbowToolkit.Scimitar.Classes.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class PropertyPath : BaseObject {
    public static readonly uint MAGIC = 0x3c00b2e0;
    protected override uint Magic => MAGIC;

    public PropertyPathNode[] Nodes = [];
    public bool TargetMustBeUnique;
    public bool WholeArray;

    public override void Parse(FastLoadReader reader) {
        var nodeCount = reader.ReadUInt32();
        Nodes = new PropertyPathNode[nodeCount];
        for (int i = 0; i < nodeCount; i++) {
            Nodes[i] = reader.Read<PropertyPathNode>();
        }
        TargetMustBeUnique = reader.ReadBoolean();
        WholeArray = reader.ReadBoolean();
    }
}
