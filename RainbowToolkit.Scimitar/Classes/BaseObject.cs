
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes;

public abstract class BaseObject {
    public ulong Uid;
    protected abstract uint Magic { get; }

    public void Read(FastLoadReader reader) {
        Uid = reader.ReadUInt64();
        var classId = reader.ReadUInt32();
        if (classId != Magic) {
            throw new Exception($"Invalid magic number for {GetType().Name}: {classId:X8}");
        }
        Parse(reader);
        Link(reader);
    }

    // Hm... maybe not a good idea :/
    public virtual void Link(FastLoadReader reader) {}

    public abstract void Parse(FastLoadReader reader);
}
