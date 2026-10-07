using RainbowToolkit.Scimitar.Compression;
using System.IO.Compression;
using Zstandard.Net;

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
    private readonly int _compressionAlgo;


    public CompressedBlock(BinaryReader reader, int blockSize, int compressionAlgo, List<ChunkedData> chunks) {
        _reader = reader;
        _chunks = chunks;
        _blockSize = blockSize;
        _compressionAlgo = compressionAlgo;
    }

    public MemoryStream Decompress() {
        var stream = new MemoryStream();
        foreach (var chunk in _chunks) {
            _reader.BaseStream.Seek(chunk.Offset, SeekOrigin.Begin);
            var data = _reader.ReadBytes(chunk.CompressedLength);

            if (chunk.CompressedLength == chunk.DecompressedLength) {
                stream.Write(data);
                continue;
            }

            var raw = new byte[chunk.DecompressedLength];
            switch (_compressionAlgo) {
                case 3:
                    using (var zstd = new ZstandardStream(new MemoryStream(data), CompressionMode.Decompress)) {
                        zstd.ReadExactly(raw);
                    }
                    break;
                default:
                    if (Oodle.Decompress(data, raw) != chunk.DecompressedLength) {
                        throw new Exception("Decompression failed");
                    }
                    break;
            }

            stream.Write(raw);
        }
        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    public static CompressedBlock Read(BinaryReader reader) {
        var headerMagic = reader.ReadUInt64();
        if (headerMagic != 0x1015FA9957FBAA37 && headerMagic != 0x1014FA9957FBAA34) {
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

        return new CompressedBlock(reader, blockSize, compressionAlgo, chunks);
    }
}
