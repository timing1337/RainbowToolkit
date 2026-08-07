namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledHighResolutionTextureMap : BaseObject {
    public static readonly uint MAGIC = 0x59CE4D13;
    protected override uint Magic => MAGIC;

    public CompiledTextureMap CompiledTextureMap;

    public override void Parse(FastLoadReader reader) {
        CompiledTextureMap = reader.Read<CompiledTextureMap>();
    }
}
