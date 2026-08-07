namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledLowResolutionTextureMap : BaseObject {
    public static readonly uint MAGIC = 0xd7b5c478;
    protected override uint Magic => MAGIC;

    public CompiledTextureMap CompiledTextureMap;

    public override void Parse(FastLoadReader reader) {
        CompiledTextureMap = reader.Read<CompiledTextureMap>();
    }
}
