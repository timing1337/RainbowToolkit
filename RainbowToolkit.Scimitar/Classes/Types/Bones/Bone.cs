
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types.Bones;

public class Bone : BaseObject {
    public static readonly uint MAGIC = 0x4D35B3F7;
    protected override uint Magic => MAGIC;

    public uint BoneId;
    public ulong ParentBoneUid;
    public Bone? ParentBone;
    public BoneInitialTransforms? Transform;

    public override void Parse(FastLoadReader reader) {
        BoneId = reader.ReadUInt32();

        ParentBoneUid = (ulong)(reader.ReadNullable() ?? 0ul);
        Transform = (BoneInitialTransforms)reader.ReadNullable();

        var boneModifierCount = reader.ReadUInt32();
        for (int i = 0; i < boneModifierCount; i++) {
            var obj = reader.ReadNullable();
        }

        var unk0 = reader.ReadUInt32();
        var unk1 = reader.ReadSingle();
        var unk2 = reader.ReadUInt16();
        var unk3 = reader.ReadUInt16();
        var unk4 = reader.ReadUInt32();
        var unk5 = reader.ReadUInt32();
        var unk6 = reader.ReadByte();
        var unk7 = reader.ReadByte();
    }
}
