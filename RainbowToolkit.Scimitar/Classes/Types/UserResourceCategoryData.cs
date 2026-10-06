namespace RainbowToolkit.Scimitar.Classes.Types;

public class UserResourceCategoryData : BaseObject {
    public static readonly uint MAGIC = 0xF67CD84C;
    protected override uint Magic => MAGIC;

    public uint Category;

    public override void Parse(FastLoadReader reader) {
        Category = reader.ReadUInt32();
    }
}
