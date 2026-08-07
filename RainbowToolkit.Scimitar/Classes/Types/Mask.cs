namespace RainbowToolkit.Scimitar.Classes.Types;

public class Mask : BaseObject {
    public static readonly uint MAGIC = 0xdf5d6c0e;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        // No additional data beyond the header.
    }
}
