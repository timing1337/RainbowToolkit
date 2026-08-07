using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar;

public class FatDescriptor {

    public uint maxFile;
    public uint maxDir;
    public ulong posFat;
    public long nextPosFat;
    public uint firstIndex;
    public uint lastIndex;
    public ulong posFatExt;
    public ulong posFatDirExt;

    public static FatDescriptor Read(BinaryReader reader) {
        var descriptor = new FatDescriptor {
            maxFile = reader.ReadUInt32(),
            maxDir = reader.ReadUInt32(),
            posFat = reader.ReadUInt64(),
            nextPosFat = reader.ReadInt64(),
            firstIndex = reader.ReadUInt32(),
            lastIndex = reader.ReadUInt32(),
            posFatExt = reader.ReadUInt64(),
            posFatDirExt = reader.ReadUInt64()
        };

        return descriptor;
    }
}
