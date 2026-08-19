using System.Numerics;
using System.Text.Json.Serialization;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class DetailMapDescriptor : BaseObject {
    public static readonly uint MAGIC = 0x7799a6d5;
    protected override uint Magic => MAGIC;

    [JsonIgnore] public ulong TextureBaseUid;
    public Vector2 Scale;

    public override void Parse(FastLoadReader reader) {
        TextureBaseUid = reader.ReadUInt64();
        var scaleU = reader.ReadSingle();
        var scaleV = reader.ReadSingle();
        Scale = new Vector2(scaleU, scaleV);
    }
}
