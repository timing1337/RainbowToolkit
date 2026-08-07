namespace RainbowToolkit.Scimitar.Classes.Types.Compiled;

public class CompiledTextureMapObject : BaseObject {
    public static readonly uint MAGIC = 0;
    protected override uint Magic => MAGIC;

    public CompiledTextureMap CompiledTextureMap;

    public override void Parse(FastLoadReader reader) {
        CompiledTextureMap = reader.Read<CompiledTextureMap>();
    }
}

public class CompiledFutureResolutionTextureMap : CompiledTextureMapObject {
    public new static readonly uint MAGIC = 0x3876CCDF;
}

public class CompiledUltraResolutionTextureMap : CompiledTextureMapObject {
    public new static readonly uint MAGIC = 0x9f492d22;
}

public class CompiledHighResolutionTextureMap : CompiledTextureMapObject {
    public new static readonly uint MAGIC = 0x59ce4d13;
}

public class CompiledMediumResolutionTextureMap : CompiledTextureMapObject {
    public new static readonly uint MAGIC = 0x3f5e4d13;
}

public class CompiledLowResolutionTextureMap : CompiledTextureMapObject {
    public new static readonly uint MAGIC = 0x1f5e4d13;
}
