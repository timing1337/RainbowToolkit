using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace RainbowToolkit.Scimitar.Utils;

public static class BinaryReaderExtension {

    public static void Advance(this BinaryReader reader, int bytes) {
        reader.BaseStream.Seek(bytes, SeekOrigin.Current);
    }

    public static T ReadStruct<T>(this BinaryReader reader) where T : unmanaged {
        Span<byte> buffer = stackalloc byte[Unsafe.SizeOf<T>()];
        reader.BaseStream.ReadExactly(buffer);
        return MemoryMarshal.Read<T>(buffer);
    }

    public static Vector3 ReadUInt64AsPos(this BinaryReader r) {
        const float bias = 0x7FFF;

        var x = (float)r.ReadInt16();
        var y = (float)r.ReadInt16();
        var z = (float)r.ReadInt16();
        var s = (float)r.ReadInt16();

        return new Vector3(x * s / bias, y * s / bias, z * s / bias);
    }

    public static Vector3 ReadUInt32AsVec(this BinaryReader reader) {
        const float bias = 0x7F;

        var x = reader.ReadByte();
        var y = reader.ReadByte();
        var z = reader.ReadByte();
        var l = reader.ReadByte();

        return new Vector3(x / bias - 1, y / bias - 1, z / bias - 1);
    }

    public static Vector2 ReadUInt32AsUv(this BinaryReader reader) {
        var u = reader.ReadHalf();
        var v = reader.ReadHalf();

        return new Vector2((float)u, (float)v);
    }

    public static Color ReadUInt32AsColor(this BinaryReader r) {
        var red = r.ReadByte();
        var green = r.ReadByte();
        var blue = r.ReadByte();
        var alpha = r.ReadByte();

        return Color.FromArgb(alpha, red, green, blue);
    }
}
