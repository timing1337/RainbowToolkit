using RainbowToolkit.Scimitar.Compression;

namespace RainbowToolkit.Scimitar;

public class ChunkedData {
    public int CompressedLength;
    public int DecompressedLength;
    public long Offset;
    public uint Checksum;
}

public class CompressedBlock {
    private readonly BinaryReader _reader;
    private readonly List<ChunkedData> _chunks;
    private readonly int _blockSize;

    public CompressedBlock(BinaryReader reader, int blockSize, List<ChunkedData> chunks) {
        _reader = reader;
        _chunks = chunks;
        _blockSize = blockSize;
    }

    public MemoryStream Decompress() {
        var stream = new MemoryStream();
        foreach (var chunk in _chunks) {
            bool isCompressed = chunk.CompressedLength != chunk.DecompressedLength;
            _reader.BaseStream.Seek(chunk.Offset, SeekOrigin.Begin);
            var data = _reader.ReadBytes(chunk.CompressedLength);
            if (!isCompressed) {
                stream.Write(data, 0, data.Length);
            } else {
                var raw = new byte[chunk.DecompressedLength];
                var result = Oodle.Decompress(data, raw);
                if (result != chunk.DecompressedLength) {
                    throw new Exception("Decompression failed");
                }
                stream.Write(raw, 0, raw.Length);
            }
        }
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public static CompressedBlock Read(BinaryReader reader) {
        var headerMagic = reader.ReadUInt64();
        if (headerMagic != 0x1015FA9957FBAA37) {
            throw new Exception($"Unsupported compressed block format. {headerMagic:X16}");
        }

        var version = reader.ReadUInt16();
        if (version >= 4) {
            throw new Exception("Unsupported compressed block version");
        }

        var compressionAlgo = reader.ReadByte();
        if (compressionAlgo >= 17) {
            throw new Exception("Unsupported compression algorithm");
        }

        var blockSize = reader.ReadInt32();
        if (blockSize < 0) {
            blockSize &= 0x7FFFFFFF;
        }

        var numChunks = reader.ReadInt32();
        var chunks = new List<ChunkedData>(numChunks);

        for (var i = 0; i < numChunks; i++) {
            chunks.Add(new ChunkedData {
                DecompressedLength = reader.ReadInt32(),
                CompressedLength = reader.ReadInt32()
            });
        }

        foreach (var chunk in chunks) {
            chunk.Checksum = reader.ReadUInt32();
            chunk.Offset = reader.BaseStream.Position;
            reader.BaseStream.Seek(chunk.CompressedLength, SeekOrigin.Current);
        }

        return new CompressedBlock(reader, blockSize, chunks);
    }
}
