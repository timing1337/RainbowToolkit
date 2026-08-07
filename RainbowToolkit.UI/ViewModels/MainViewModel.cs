using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using RainbowToolkit.Scimitar;

namespace RainbowToolkit.UI.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    // TEMP: hardcoded game install path — swap for a folder picker later.
    private const string GameDirectory =
        @"E:\Program Files (x86)\Steam\steamapps\common\Tom Clancy's Rainbow Six Siege";

    // TEMP: every archive in the game directory is opened, but only these two go in the
    // tree for now. The rest wait, fully loaded, in BackgroundArchives.
    private static readonly string[] ShownArchives =
    [
        "datapc64.forge",
        "datapc64_ondemand.forge",
    ];

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial NodeViewModelBase? SelectedNode { get; set; }

    /// <summary>Footer line: load progress while archives open, then whatever failed. Null when idle.</summary>
    [ObservableProperty]
    public partial string? StatusMessage { get; set; }

    /// <summary>Archives the directory tree shows.</summary>
    public ObservableCollection<ArchiveNodeViewModel> Archives { get; } = new();

    /// <summary>
    /// Archives that are open and indexed but deliberately kept out of the tree. Moving one
    /// into <see cref="Archives"/> is all it takes to show it — nothing is reloaded.
    /// </summary>
    public ObservableCollection<ArchiveNodeViewModel> BackgroundArchives { get; } = new();

    public MainViewModel()
    {
        // The Avalonia previewer instantiates this too; don't touch the disk there.
        if (Design.IsDesignMode)
        {
            LoadDesignData();
            return;
        }

        _ = LoadArchivesAsync();
    }

    /// <summary>
    /// Opens every archive in the game directory off the UI thread, one at a time, showing
    /// each as soon as it is indexed. Fire-and-forget, so it never throws.
    /// </summary>
    private async Task LoadArchivesAsync()
    {
        List<string> paths;
        try
        {
            paths = EnumerateArchivePaths().ToList();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Couldn't read {GameDirectory} — {ex.Message}";
            return;
        }

        var opened = 0;
        var failed = 0;
        foreach (var path in paths)
        {
            StatusMessage = $"Loading archives… {++opened}/{paths.Count}";
            try
            {
                // Opening walks the whole FAT, so do it on a worker thread; the await puts
                // us back on the UI thread to touch the collections.
                var archive = await Task.Run(() => LoadArchive(path));
                var target = IsShown(path) ? Archives : BackgroundArchives;
                target.Add(archive);
            }
            catch (Exception)
            {
                failed++;
            }
        }

        StatusMessage = failed == 0 ? null : $"{failed} of {paths.Count} archives couldn't be read";
    }

    /// <summary>
    /// Archive paths in load order: the ones the tree shows come first so it fills in right
    /// away, then everything else alphabetically.
    /// </summary>
    private static IEnumerable<string> EnumerateArchivePaths() =>
        Directory.EnumerateFiles(GameDirectory, "*.forge")
            // soundbanks use a different format!
            .Where(path => !Path.GetFileName(path).Contains("soundmedia", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(IsShown)
            .ThenBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);

    private static bool IsShown(string path) =>
        ShownArchives.Contains(Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);

    /// <summary>Opens and indexes one archive. Runs off the UI thread.</summary>
    private static ArchiveNodeViewModel LoadArchive(string path)
    {
        // Not disposed on purpose: the node reads its entries lazily on expand and they read
        // their assets lazily in turn, so the archive stays open for the app's lifetime.
        var scimitar = ScimitarFile.Open(path);
        return new ArchiveNodeViewModel(scimitar, Path.GetFileName(path));
    }

    private void LoadDesignData()
    {
        // Placeholder data mirroring the Figma design, shown only in the previewer.
        foreach (var archiveName in ShownArchives)
        {
            var archive = new ArchiveNodeViewModel(archiveName) { IsExpanded = true };
            for (var i = 0; i < 3; i++)
            {
                var entry = new EntryNodeViewModel("0x0000000000000000") { IsExpanded = true };
                entry.Children.Add(new AssetNodeViewModel(0xA1B2C3D4u, 2048));
                entry.Children.Add(new AssetNodeViewModel(0x11223344u, 512));
                archive.Children.Add(entry);
            }

            Archives.Add(archive);
        }
    }
}
