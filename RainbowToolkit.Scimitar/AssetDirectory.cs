using RainbowToolkit.Scimitar.Assets;
using RainbowToolkit.Scimitar.Classes;
using RainbowToolkit.Scimitar.Container;

namespace RainbowToolkit.Scimitar;

public class AssetDirectory {
    public readonly Dictionary<string, ScimitarFile> Forges = new();
    private readonly Dictionary<ulong, ScimitarFile> _files = new();
    private readonly Dictionary<ulong, AssetContainer> _loaded = new();

    public static AssetDirectory Open(string folder) {
        var directory = new AssetDirectory();
        foreach (var path in Directory.GetFiles(folder, "*.forge")) {
            var forge = ScimitarFile.Open(path);
            directory.Forges[Path.GetFileNameWithoutExtension(path)] = forge;
            foreach (var uid in forge.Files.Keys) {
                directory._files.TryAdd(uid, forge);
            }
        }

        var baseForge = directory.Forges["datapc64"];
        foreach (var uid in baseForge.Files.Keys) {
            try {
                directory.Load(baseForge, uid);
            } catch { }
        }

        return directory;
    }

    public Asset? Find(ulong uid) {
        if (!_loaded.ContainsKey(uid) && _files.TryGetValue(uid, out var forge)) {
            Load(forge, uid);
        }
        return _loaded.GetValueOrDefault(uid)?.ReadAsset(uid);
    }

    public T Get<T>(ulong uid) where T : BaseObject {
        return Find(uid)?.As<T>() ?? throw new Exception($"Asset {uid:X} not found or not a {typeof(T).Name}.");
    }

    private void Load(ScimitarFile forge, ulong fileUid) {
        var container = forge.GetAssetContainer(fileUid);
        foreach (var uid in container.Entries.Keys) {
            _loaded[uid] = container;
        }
    }
}
