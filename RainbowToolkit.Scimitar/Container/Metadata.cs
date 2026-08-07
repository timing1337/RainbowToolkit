using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Container;

public class Metadata {

    public Dictionary<uint, object> Entries = new();

    private static object ReadValue(BinaryReader reader, uint itemType) {
        switch (itemType) {
            case 0:
                return reader.ReadUInt32();
            case 1:
                int strLen = reader.ReadInt32();
                string result;
                if (strLen > 1) {
                    result = Encoding.UTF8.GetString(reader.ReadBytes(strLen));
                } else {
                    result = string.Empty;
                }
                reader.ReadByte();
                return result;
            case 2:
            case 4:
            case 6:
                return reader.ReadBytes(reader.ReadInt32());
            case 3:
                return reader.ReadBoolean();
            case 5:
                return reader.ReadUInt64();
            default:
                throw new Exception("Unknown item type: " + itemType);
        }
    }

    public static Metadata Read(BinaryReader reader) {
        reader.ReadUInt32();

        var metadata = new Metadata();

        while (true) {
            var index = reader.ReadUInt32();
            if (index == 0) {
                break;
            }
            var itemType = reader.ReadUInt32();
            var value = ReadValue(reader, itemType);
            metadata.Entries[index] = value;
        }
        return metadata;
    }
}
