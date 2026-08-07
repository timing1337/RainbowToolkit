namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledMediumResolutionTextureMap : BaseObject {
    public static readonly uint MAGIC = 0xf9c80707;
    protected override uint Magic => MAGIC;

    public CompiledTextureMap CompiledTextureMap;

    public override void Parse(FastLoadReader reader) {
        CompiledTextureMap = reader.Read<CompiledTextureMap>();
    }
}
