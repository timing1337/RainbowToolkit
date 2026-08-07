
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class BuildColumn : BaseObject {
    public static readonly uint MAGIC = 0xAA77362B;
    protected override uint Magic => MAGIC;

    public uint PassField;
    public PropertyPath PropertyPath;
    public uint ColumnId;
    public uint ColumnMask;
    public uint ColumnMaskToReject;
    public DynamicProperty[] DynamicProperties = [];

    public override void Parse(FastLoadReader reader) {
        PassField = reader.ReadUInt32();
        PropertyPath = reader.Read<PropertyPath>();
        ColumnId = reader.ReadUInt32();
        ColumnMask = reader.ReadUInt32();
        ColumnMaskToReject = reader.ReadUInt32();
        var dynamic_property_count = reader.ReadUInt32();
        DynamicProperties = new DynamicProperty[dynamic_property_count];
        for (int i = 0; i < dynamic_property_count; i++) {
            DynamicProperties[i] = DynamicProperty.Read(reader);
        }
    }
}
