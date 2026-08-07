using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class BuildRow : BaseObject {
    public static readonly uint MAGIC = 0x5BF66831;
    protected override uint Magic => MAGIC;

    public ulong TableId;
    public float Weight;
    public double[] TaggedWeights = [];
    public DynamicProperty[] DynamicProperties = [];

    public override void Parse(FastLoadReader reader) {
        TableId = reader.ReadUInt64();
        Weight = reader.ReadSingle();

        var taggedWeightsCount = reader.ReadUInt32();
        TaggedWeights = new double[taggedWeightsCount];
        for(int i = 0; i < taggedWeightsCount; i++) {
            TaggedWeights[i] = reader.ReadDouble();
        }

        var dynamicPropertyCount = reader.ReadUInt32();
        DynamicProperties = new DynamicProperty[dynamicPropertyCount];
        for(int i = 0; i < dynamicPropertyCount; i++) {
            DynamicProperties[i] = DynamicProperty.Read(reader);
        }
    }
}
