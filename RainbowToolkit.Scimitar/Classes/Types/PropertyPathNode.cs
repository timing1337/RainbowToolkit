
using RainbowToolkit.Scimitar.Classes.Types;
using RainbowToolkit.Scimitar.Enums;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class PropertyPathNode : BaseObject {
    public static readonly uint MAGIC = 0xD26BFC71;
    protected override uint Magic => MAGIC;

    public ushort Index;
    public NodeCondition Condition;
    public NodeCondition Condition2;
    public bool MatchAllConditions;
    public bool ArrayMultiTargeting;
    public uint TargetPropertyId;
    public ulong ArrayOriginalId;
    public uint ConditionPropertyId;
    public ulong ConditionData;
    public uint Condition2PropertyId;
    public ulong Condition2Data;

    public override void Parse(FastLoadReader reader) {
        Index = reader.ReadUInt16();
        Condition = (NodeCondition)reader.ReadUInt32();
        Condition2 = (NodeCondition)reader.ReadUInt32();
        MatchAllConditions = reader.ReadBoolean();
        ArrayMultiTargeting = reader.ReadBoolean();
        TargetPropertyId = reader.ReadUInt32();
        ArrayOriginalId = reader.ReadUInt64();
        ConditionPropertyId = reader.ReadUInt32();
        ConditionData = reader.ReadUInt64();
        Condition2PropertyId = reader.ReadUInt32();
        Condition2Data = reader.ReadUInt64();
        var nodeSolver = reader.ReadNullable();
    }
}
