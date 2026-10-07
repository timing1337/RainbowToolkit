
using RainbowToolkit.Scimitar.Classes;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Text;

namespace RainbowToolkit.Scimitar.Assets;

public class Asset {
    public FastLoadReader Reader;
    public string Name;
    public ushort Flags;
    public uint DataLength;
    public uint ClassId;
    public BaseObject? Data;

    public Asset(FastLoadReader reader) {
        Reader = reader;
        var nameLength = reader.ReadUInt16();
        Flags = reader.ReadUInt16();
        Name = Convert.ToHexString(reader.ReadBytes(nameLength));
        DataLength = reader.ReadUInt32();
        ClassId = reader.ReadUInt32();

        if (ClassRegistry.RegisteredClasses.ContainsKey(ClassId)) {
            Data = reader.ReadObject();
        }
    }

    public T? As<T>() where T : BaseObject {
        if(Data == null) {
            Data = Reader.ReadObject();
        }
        return Data as T;
    }
}
