using RainbowToolkit.Scimitar.Container;
using RainbowToolkit.Scimitar.FAT;

namespace RainbowToolkit.Scimitar;

public class ScimitarFile : IDisposable {
    private readonly BinaryReader _reader;

    public readonly string Path;

    public uint Version;
    public long PosFat;
    public long GlobalMetaKey;
    public byte Flag;
    public uint EncryptionKey;

    public Dictionary<ulong, FatFile> Files = new();
    public Dictionary<ulong, AssetContainer> Cached = new();

    private ScimitarFile(BinaryReader reader, string path) {
        Path = path;
        _reader = reader;

        Index();
    }

    private void Index() {
        var header = _reader.ReadBytes(9);
        Version = _reader.ReadUInt32();
        PosFat = _reader.ReadInt64();
        GlobalMetaKey = _reader.ReadInt64();
        Flag = _reader.ReadByte();

        if (Version > 33) {
            EncryptionKey = _reader.ReadUInt32();
        } else {
            EncryptionKey = Version switch {
                30 => 1u,
                31 => 2u,
                32 => 3u,
                33 => 4u,
                _ => 0u
            };
        }

        _reader.BaseStream.Seek(PosFat, SeekOrigin.Begin);
        var maxFile = _reader.ReadUInt32();
        var maxDir = _reader.ReadUInt32();
        var maxKey = _reader.ReadUInt64();
        var root = _reader.ReadUInt32();
        var firstFreeFile = _reader.ReadUInt32();
        var firstFreeDir = _reader.ReadUInt32();
        var sizeofFat = _reader.ReadUInt32();
        var nbFat = _reader.ReadUInt32();
        var positionFatDescriptors = _reader.ReadInt64();

        _reader.BaseStream.Seek(positionFatDescriptors, SeekOrigin.Begin);
        var descriptors = new FatDescriptor[nbFat];
        for (int i = 0; i < nbFat; i++) {
            var descriptor = FatDescriptor.Read(_reader);

            _reader.BaseStream.Seek((long)descriptor.posFat, SeekOrigin.Begin);
            for (int j = 0; j < descriptor.maxFile; j++) {
                var file = FatFile.Read(_reader, EncryptionKey);
                Files[file.Uid] = file;
            }

            if (descriptor.nextPosFat != -1) {
                _reader.BaseStream.Seek(descriptor.nextPosFat, SeekOrigin.Begin);
            }
        }
    }

    public static ScimitarFile Open(string path) {
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var scimitar = new ScimitarFile(new BinaryReader(stream), path);
        return scimitar;
    }

    public FatFile GetFatFile(ulong uid) {
        if (Files.TryGetValue(uid, out var file)) {
            return file;
        }

        throw new Exception($"File with UID {uid} not found.");
    }

    public MemoryStream GetFileStream(ulong uid) {
        var file = GetFatFile(uid);
        _reader.BaseStream.Seek((long)file.Offset, SeekOrigin.Begin);
        var data = _reader.ReadBytes((int)file.Size);
        return new MemoryStream(data);
    }

    public AssetContainer GetAssetContainer(ulong uid) {
        if(Cached.ContainsKey(uid)) {
            return Cached[uid];
        }

        var container = new AssetContainer(new BinaryReader(GetFileStream(uid)));
        Cached[uid] = container;
        return container;
    }

    public void Dispose() {
        _reader.Dispose();
    }
}
