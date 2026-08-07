
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes;

public class DynamicProperty {
    public uint Version;

    public uint PropertyId;
    public ulong PropertyType;
    public uint FieldType;

    public object? Value;

    // idk tbh, needs to cross check with older version
    public bool IsRef => Version == 28;

    public ulong ValueUid => IsRef ? (ulong)Value! : 0;

    public static DynamicProperty Read(FastLoadReader reader) {
        var propertyId = reader.ReadUInt32();
        var propertyType = reader.ReadUInt64();
        var flag = reader.ReadUInt32();

        var classId = (uint)(propertyType & 0xFFFFFFFF);
        var version = (uint)((propertyType >> 48) & 0x3F); //?? not sure

        var dynamicProperty = new DynamicProperty() {
            Version = version,
            FieldType = classId,
            PropertyId = propertyId,
            PropertyType = propertyType,
        };

        switch (version) {
            case 28:
                dynamicProperty.Value = reader.ReadUInt64();
                break;
            case 22:
                dynamicProperty.Value = reader.ReadObject();
                break;
        }
        return dynamicProperty;
    }
}
