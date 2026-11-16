using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Container;

public class DepGraph {
    public DepGraph(BinaryReader reader) {
        var compressedBlock = CompressedBlock.Read(reader);
        var depGraphReader = new BinaryReader(compressedBlock.Decompress());
        var count = depGraphReader.BaseStream.Length / 24;
        for(int i = 0; i < count; i++) {
            depGraphReader.ReadByte();
            var parentUid = depGraphReader.ReadUInt64();
            var childrenUid = depGraphReader.ReadUInt64();
            var childType = depGraphReader.ReadUInt32();
            depGraphReader.ReadByte();
            depGraphReader.ReadByte();
            depGraphReader.ReadByte();
        }
    }

    public static DepGraph Open(string path) {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(stream);
        return new DepGraph(reader);
    }
}
