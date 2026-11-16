using RainbowToolkit.Scimitar.Classes;

namespace RainbowToolkit.Scimitar;

public sealed class FastLoadReader : BinaryReader {
    private readonly Dictionary<ulong, BaseObject> _cache = new();

    public FastLoadReader(Stream input) : base(input) { }

    public T GetLocalObject<T>(ulong uid) where T : BaseObject {
        if (_cache.TryGetValue(uid, out var obj)) {
            return (T)obj;
        }
        throw new KeyNotFoundException($"Object with UID {uid} not found in cache.");
    }

    public T Read<T>() where T : BaseObject {
        return (T)ReadObject();
    }

    public BaseObject ReadObject() {
        var uid = ReadUInt64();
        var classId = ReadUInt32();
        return Parse(classId, uid);
    }

    public override string ReadString() {
        var length = ReadInt32();
        return length == 0 ? "" : System.Text.Encoding.UTF8.GetString(ReadBytes(length + 1), 0, length);
    }

    public ulong ReadBits(ulong bits, int offset, int width = 1, bool dword = false) {
        ulong value = dword ? ReadUInt32() : ReadByte();
        var mask = (1UL << width) - 1;
        return bits & ~(mask << offset) | (value & mask) << offset;
    }

    public object ReadNullable() {
        var flag = ReadByte();
        if (flag == 3) {
            return null;
        }

        var uid = ReadUInt64();
        if (flag != 0) {
            return uid;
        }

        var classId = ReadUInt32();
        return Parse(classId, uid);
    }


    private BaseObject Parse(uint classId, ulong uid) {
        var obj = ClassRegistry.Create(classId);
        if (uid != 0) {
            obj.Uid = uid;
            _cache[uid] = obj;
        }
        obj.Parse(this);
        obj.Link(this);
        return obj;
    }
}
