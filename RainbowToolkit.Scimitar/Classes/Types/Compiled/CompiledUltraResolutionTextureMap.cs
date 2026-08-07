namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledUltraResolutionTextureMap : BaseObject {
    public static readonly uint MAGIC = 0x9f492d22;
    protected override uint Magic => MAGIC;

    public CompiledTextureMap CompiledTextureMap;

    public override void Parse(FastLoadReader reader) {
        CompiledTextureMap = reader.Read<CompiledTextureMap>();
    }
}
