using RainbowToolkit.Sandbox.Helpers;
using RainbowToolkit.Scimitar;
using RainbowToolkit.Scimitar.Classes.Types;

namespace RainbowToolkit.Sandbox;

internal class Program {
    static void Main(string[] args) {
        var assets = AssetDirectory.Open(@"E:\Program Files (x86)\Steam\steamapps\common\Tom Clancy's Rainbow Six Siege");
        //0000001873267BE3:CHR_SPETSNAZ_Tachanka_UNF_Y3S3_SEASONALCAMO_MasterTP.BuildTable
        //0000001873267B6A:CHR_SPETSNAZ_Tachanka_HDG_Y3S3_SEASONALCAMO_MasterTP.BuildTable

        BuildTableHelper.ExportCharacterBuildTable(assets, assets.Get<BuildTable>(0x1873267BE3), @"E:\");
        BuildTableHelper.ExportCharacterBuildTable(assets, assets.Get<BuildTable>(0x1873267B6A), @"E:\");
    }
}
