using RainbowToolkit.Scimitar.Classes.Types.Bones;
using System;
using System.Collections.Generic;
using System.Text;

namespace RainbowToolkit.Scimitar.Classes.Types;

public class Skeleton : BaseObject {
    public static readonly uint MAGIC = 0x6CA3CBFA;
    protected override uint Magic => MAGIC;

    public Bone[] Bones = [];

    public override void Parse(FastLoadReader reader) {
        var unk = reader.ReadUInt32();
        var boneCount = reader.ReadUInt32();
        Bones = new Bone[boneCount];
        for (uint i = 0; i < boneCount; i++) {
            Bones[i] = (Bone)reader.ReadNullable();
        }
    }

    public override void Link(FastLoadReader reader) {
        foreach (var bone in Bones) {
            if(bone.ParentBoneUid == 0) {
                continue;
            }

            bone.ParentBone = reader.GetLocalObject<Bone>(bone.ParentBoneUid);
        }
    }
}
