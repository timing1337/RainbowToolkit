using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class TexturePack {
    public ulong LowResUid;
    public ulong MediumResUid;
    public ulong HighResUid;
    public ulong UltraResUid;
    public ulong FutureResUid;

    public static TexturePack Read(BinaryReader reader) {
        var lowResUid = reader.ReadUInt64();
        var mediumResUid = reader.ReadUInt64();
        var highResUid = reader.ReadUInt64();
        var ultraResUid = reader.ReadUInt64();
        var futureResUid = reader.ReadUInt64();

        return new TexturePack {
            LowResUid = lowResUid,
            MediumResUid = mediumResUid,
            HighResUid = highResUid,
            UltraResUid = ultraResUid,
            FutureResUid = futureResUid
        };
    }
}
