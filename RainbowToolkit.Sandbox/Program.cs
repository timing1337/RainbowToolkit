using RainbowToolkit.Sandbox.Helpers;
using RainbowToolkit.Scimitar.Classes.Types;

namespace RainbowToolkit.Sandbox;

internal class Program {
    static void Main(string[] args) {
        ScimitarManager.Initialize(@"E:\Program Files (x86)\Steam\steamapps\common\Tom Clancy's Rainbow Six Siege");
        var scimitar_mgr = ScimitarManager.Instance;

        var onDemand = scimitar_mgr.GetFileByName("datapc64_ondemand");
        var container = onDemand.GetAssetContainer(0x1873267BE3);
        var asset = container.ReadAsset(0x1873267BE3).As<BuildTable>()!;

        BuildTableHelper.ExportCharacterBuildTable(container, asset, @"E:\");
    }
}
