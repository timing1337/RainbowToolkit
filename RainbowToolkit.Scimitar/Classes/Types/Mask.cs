namespace RainbowToolkit.Scimitar.Classes.Types;

public class Mask : BaseObject {
    public static readonly uint MAGIC = 0x7D4D9C37;
    protected override uint Magic => MAGIC;

    public override void Parse(FastLoadReader reader) {
        var unk = reader.ReadByte();
    }
}
