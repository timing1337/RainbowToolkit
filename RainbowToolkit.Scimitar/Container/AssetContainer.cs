using RainbowToolkit.Scimitar.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Container;

public class AssetContainer {
    private CompressedBlock _headerBlock;
    private CompressedBlock _bodyBlock;
    private Dictionary<ulong, (int, int)>? _entries;
    private Dictionary<ulong, Asset> _assetCache = new();
    private MemoryStream _body;

    public Dictionary<ulong, (int, int)> Entries {
        get {
            if (_entries == null) {
                _entries = DeserializeHeader();
            }
            return _entries;
        }
    }

    public AssetContainer(BinaryReader reader) {
        _headerBlock = CompressedBlock.Read(reader);
        _bodyBlock = CompressedBlock.Read(reader);
    }

    private Dictionary<ulong, (int, int)> DeserializeHeader() {
        var entries = new Dictionary<ulong, (int, int)>();
        var header = new BinaryReader(_headerBlock.Decompress());
        var count = header.ReadUInt32();
        int offset = 0;

        for (int i = 0; i < count; i++) {
            var uid = header.ReadUInt64();
            var size = header.ReadInt32();
            if (size >= 0x40000000) {
                var additionalHeader = header.ReadBytes(16);
                size &= 0x3FFFFFFF;
            }
            entries.Add(uid, (size, offset));
            offset += size;
        }

        return entries;
    }

    public bool ContainsAsset(ulong uid) {
        return Entries.ContainsKey(uid);
    }

    public Asset ReadAsset(ulong uid) {
        if(_assetCache.ContainsKey(uid)) {
            return _assetCache[uid];
        }

        if (!Entries.ContainsKey(uid)) {
            throw new Exception($"Asset with UID {uid} not found.");
        }

        if(_body == null) {
            _body = _bodyBlock.Decompress();
        }

        var (size, offset) = Entries[uid];
        var buffer = new byte[size];
        _body.Seek(offset, SeekOrigin.Begin);
        _body.Read(buffer, 0, size);
        var asset = new Asset(new FastLoadReader(new MemoryStream(buffer)));
        _assetCache[uid] = asset;
        return asset;
    }
}
