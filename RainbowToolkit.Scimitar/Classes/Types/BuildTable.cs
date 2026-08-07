using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class BuildTable : BaseObject {
    public static readonly uint MAGIC = 0xB4361608;
    protected override uint Magic => MAGIC;

    public BuildColumn[] Columns = [];
    public BuildRow[] Rows = [];
    public override void Parse(FastLoadReader reader) {
        var columnCount = reader.ReadUInt32();
        Columns = new BuildColumn[columnCount];
        for (int i = 0; i < columnCount; i++) {
            Columns[i] = reader.Read<BuildColumn>();
        }

        var rowCount = reader.ReadUInt32();
        Rows = new BuildRow[rowCount];
        for (int i = 0; i < rowCount; i++) {
            Rows[i] = reader.Read<BuildRow>();
        }
    }
}
