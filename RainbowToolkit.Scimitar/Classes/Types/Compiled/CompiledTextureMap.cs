
using RainbowToolkit.Scimitar.Enums;

namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledTextureMap : BaseObject {
    public static readonly uint MAGIC = 0x13237FE9;
    protected override uint Magic => MAGIC;

    public PixelFormat PixelFormat;
    public CompiledTextureMapData Data;

    public override void Parse(FastLoadReader reader) {
        Data = reader.Read<CompiledTextureMapData>();
        PixelFormat = (PixelFormat)reader.ReadUInt32();
    }
}
