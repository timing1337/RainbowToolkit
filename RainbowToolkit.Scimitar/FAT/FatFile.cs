using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RainbowToolkit.Scimitar.FAT;

public class FatFile {
    public ulong Offset;
    public ulong Uid;
    public uint Size;
    public ulong End;

    public static FatFile Read(BinaryReader reader, uint encryptionKey) {
        var chunk = new FatFile {
            Offset = reader.ReadUInt64(),
            Uid = reader.ReadUInt64(),
            Size = reader.ReadUInt32()
        };

        switch (encryptionKey) {
            case 3:
            case 4:
                chunk.Uid = BitOperations.RotateRight(chunk.Uid + 0xAFADDBC7C7BBBC9C, (int)(chunk.Offset % 62) + 1) ^ 0x3934394E23482361;
                break;
            case 5:
                chunk.Offset = (chunk.Offset ^ 0xE0418BA6204D12AE) + 0x313067CC03C7F77D;
                chunk.Uid = (chunk.Uid - 0x42B3380C989AE014) ^ 0xFFD756BB69CA634B;
                chunk.Size = (chunk.Size ^ 0x88B68D4D) - 0x43A4E3F5;
                break;
            case 6:
                chunk.Offset = (chunk.Offset ^ 0x7CC2583535777B15) - 0x507EE14611C4AC31;
                chunk.Uid = (chunk.Uid - 0x33432D300A4C8048) ^ 0xD615D7B0ABF4B6A4;
                chunk.Size = (chunk.Size ^ 0x6D4053A0) - 0x3C48E3F9;
                break;
        }

        chunk.End = chunk.Offset + chunk.Size;
        return chunk;
    }
}
