using RainbowToolkit.Sandbox.Helpers;
using RainbowToolkit.Scimitar;
using RainbowToolkit.Scimitar.Classes.Types.Compiled;
using RainbowToolkit.Scimitar.Container;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Sandbox;

public class ScimitarManager {

    private static ScimitarManager? _instance;
    public static ScimitarManager Instance => _instance ?? throw new InvalidOperationException("ScimitarManager is not initialized. Call ScimitarManager.Initialize(path) first.");

    public readonly string FolderPath;
    public Dictionary<string, ScimitarFile> Scimitars = new();

    private ScimitarManager(string path) {
        FolderPath = path;
    }

    public static void Initialize(string path) {
        _instance = new ScimitarManager(path);
        _instance.Load();
    }

    private void Load() {
        var list = new Dictionary<ulong, string>();

        foreach (var file in Directory.GetFiles(FolderPath, "*.forge")) {
            var name = Path.GetFileNameWithoutExtension(file);
            Scimitars[name] = ScimitarFile.Open(file);
        }
    }

    public ScimitarFile GetFileByName(string name) {
        if (Scimitars.ContainsKey(name)) {
            return Scimitars[name];
        }
        throw new Exception($"Scimitar file '{name}' not found.");
    }

    // i.. hate this a lot but i genuinely don't know how to
    // design a better system for this.
    public AssetContainer? FindAssetContainer(ulong uid) {
        foreach (var scimitar in Scimitars.Values) {
            if (scimitar.Files.ContainsKey(uid)) {
                return scimitar.GetAssetContainer(uid);
            }
        }
        return null;
    }
}
