using System.Text;

namespace Diamond.FileFormats;

public class MobsScrEntry2
{
    public MobId Id;
    public string Name;       // 17 bytes
    public string Location;   // 33 bytes
    public uint Subarea;
    public uint Unk2_0;
    public float BaseFame;
    public uint Unk2_2;
    public uint Unk2_3;
    public uint Unk2_4;
    public uint Unk2_5;
    public uint Unk2_6;
    public uint Lv;
    public uint Unk0;
    public ushort Unk1;
    public ushort Unk1_2;
    public uint Unk2;
    public uint Unk3;
    public uint Unk4;
    public uint Unk5;
    public uint Unk6;
    public uint Unk7;
    public uint Unk8;
    public uint Unk9;
    public uint Unk10;
    public uint Unk11;
    public uint Unk12;
    public uint Unk13;
    public uint Unk14;
    public uint Unk15;
    public uint Unk16;
    public uint Unk17;
    public uint Unk18;
    public uint Unk19;
    public uint Unk20;
    public uint Unk21;
    public uint Unk22;
    public uint Unk23;
    public uint Unk24;
    public float Unk25;
    public uint Unk26;
    public uint Unk27;
    public uint Unk28;
    public uint Unk29;
    public uint Unk30;
    public uint Unk31;
    public uint Unk32;
    public float Unk33;
    public float Unk34;
    public uint Unk35;
    public uint MinAtk;
    public uint MaxAtk;
    public uint Unk38;
    public uint Unk39;
    public uint HP;
    public uint Unk41;
    public uint AtkSucceed;
    public uint DefSucceed;
    public uint Unk44;
    public uint Unk45;
    public float Unk46;
    public float Unk47;
    public float Unk48;
    public float Unk49;
    public float Unk50;
    public float Unk51;
    public float Unk52;
    public float Unk53;
    public float Unk54;
    public float Unk55;
    public float Unk56;
    public float Unk57;
    public float Unk58;
    public float Unk59;
    public float Unk60;
    public float Unk61;
    public uint Unk62;
    public uint Unk63;
    public uint Unk64;
    public uint Unk65;
    public uint Unk66;
    public uint Unk67;
    public uint Unk68;
    public uint Unk69;
    public uint Unk70;
    public uint Unk71;
    public uint Unk72;
    public uint Unk73;
    public uint Coins;
    public uint Unk75;
    public uint Unk76;
    public float Unk77;
    public float Unk78;
    public uint Skill1;
    public uint Skill2;
    public uint Skill3;
    public uint Unk82;
    public uint Unk83;
    public uint Unk84;
    public uint Unk85;
    public uint Unk86;
    public uint Unk87;
    public uint Unk88;
    public float Unk89;
    public float Unk90;
    public float Unk91;
    public uint Unk92;
    public ushort Unk93;
    public ushort Unk93_2;
    public ushort Unk94;
    public ushort Unk94_2;
    public uint Unk95;
    public uint Unk96;
    public ushort Unk97;
    public ushort Unk97_2;
    public uint Unk98;
    public uint Unk99;

    public static MobsScrEntry2 Read(BinaryReader br)
    {
        var entry = new MobsScrEntry2();
        entry.Id = (MobId)br.ReadUInt16();
        entry.Name = EucKrEncoding.Instance.GetString(br.ReadBytes(17)).TrimEnd('\0');
        entry.Location = EucKrEncoding.Instance.GetString(br.ReadBytes(33)).TrimEnd('\0');
        entry.Subarea = br.ReadUInt32();
        entry.Unk2_0 = br.ReadUInt32();
        entry.BaseFame = br.ReadSingle();
        entry.Unk2_2 = br.ReadUInt32();
        entry.Unk2_3 = br.ReadUInt32();
        entry.Unk2_4 = br.ReadUInt32();
        entry.Unk2_5 = br.ReadUInt32();
        entry.Unk2_6 = br.ReadUInt32();
        entry.Lv = br.ReadUInt32();
        entry.Unk0 = br.ReadUInt32();
        entry.Unk1 = br.ReadUInt16();
        entry.Unk1_2 = br.ReadUInt16();
        entry.Unk2 = br.ReadUInt32();
        entry.Unk3 = br.ReadUInt32();
        entry.Unk4 = br.ReadUInt32();
        entry.Unk5 = br.ReadUInt32();
        entry.Unk6 = br.ReadUInt32();
        entry.Unk7 = br.ReadUInt32();
        entry.Unk8 = br.ReadUInt32();
        entry.Unk9 = br.ReadUInt32();
        entry.Unk10 = br.ReadUInt32();
        entry.Unk11 = br.ReadUInt32();
        entry.Unk12 = br.ReadUInt32();
        entry.Unk13 = br.ReadUInt32();
        entry.Unk14 = br.ReadUInt32();
        entry.Unk15 = br.ReadUInt32();
        entry.Unk16 = br.ReadUInt32();
        entry.Unk17 = br.ReadUInt32();
        entry.Unk18 = br.ReadUInt32();
        entry.Unk19 = br.ReadUInt32();
        entry.Unk20 = br.ReadUInt32();
        entry.Unk21 = br.ReadUInt32();
        entry.Unk22 = br.ReadUInt32();
        entry.Unk23 = br.ReadUInt32();
        entry.Unk24 = br.ReadUInt32();
        entry.Unk25 = br.ReadSingle();
        entry.Unk26 = br.ReadUInt32();
        entry.Unk27 = br.ReadUInt32();
        entry.Unk28 = br.ReadUInt32();
        entry.Unk29 = br.ReadUInt32();
        entry.Unk30 = br.ReadUInt32();
        entry.Unk31 = br.ReadUInt32();
        entry.Unk32 = br.ReadUInt32();
        entry.Unk33 = br.ReadSingle();
        entry.Unk34 = br.ReadSingle();
        entry.Unk35 = br.ReadUInt32();
        entry.MinAtk = br.ReadUInt32();
        entry.MaxAtk = br.ReadUInt32();
        entry.Unk38 = br.ReadUInt32();
        entry.Unk39 = br.ReadUInt32();
        entry.HP = br.ReadUInt32();
        entry.Unk41 = br.ReadUInt32();
        entry.AtkSucceed = br.ReadUInt32();
        entry.DefSucceed = br.ReadUInt32();
        entry.Unk44 = br.ReadUInt32();
        entry.Unk45 = br.ReadUInt32();
        entry.Unk46 = br.ReadSingle();
        entry.Unk47 = br.ReadSingle();
        entry.Unk48 = br.ReadSingle();
        entry.Unk49 = br.ReadSingle();
        entry.Unk50 = br.ReadSingle();
        entry.Unk51 = br.ReadSingle();
        entry.Unk52 = br.ReadSingle();
        entry.Unk53 = br.ReadSingle();
        entry.Unk54 = br.ReadSingle();
        entry.Unk55 = br.ReadSingle();
        entry.Unk56 = br.ReadSingle();
        entry.Unk57 = br.ReadSingle();
        entry.Unk58 = br.ReadSingle();
        entry.Unk59 = br.ReadSingle();
        entry.Unk60 = br.ReadSingle();
        entry.Unk61 = br.ReadSingle();
        entry.Unk62 = br.ReadUInt32();
        entry.Unk63 = br.ReadUInt32();
        entry.Unk64 = br.ReadUInt32();
        entry.Unk65 = br.ReadUInt32();
        entry.Unk66 = br.ReadUInt32();
        entry.Unk67 = br.ReadUInt32();
        entry.Unk68 = br.ReadUInt32();
        entry.Unk69 = br.ReadUInt32();
        entry.Unk70 = br.ReadUInt32();
        entry.Unk71 = br.ReadUInt32();
        entry.Unk72 = br.ReadUInt32();
        entry.Unk73 = br.ReadUInt32();
        entry.Coins = br.ReadUInt32();
        entry.Unk75 = br.ReadUInt32();
        entry.Unk76 = br.ReadUInt32();
        entry.Unk77 = br.ReadSingle();
        entry.Unk78 = br.ReadSingle();
        entry.Skill1 = br.ReadUInt32();
        entry.Skill2 = br.ReadUInt32();
        entry.Skill3 = br.ReadUInt32();
        entry.Unk82 = br.ReadUInt32();
        entry.Unk83 = br.ReadUInt32();
        entry.Unk84 = br.ReadUInt32();
        entry.Unk85 = br.ReadUInt32();
        entry.Unk86 = br.ReadUInt32();
        entry.Unk87 = br.ReadUInt32();
        entry.Unk88 = br.ReadUInt32();
        entry.Unk89 = br.ReadSingle();
        entry.Unk90 = br.ReadSingle();
        entry.Unk91 = br.ReadSingle();
        entry.Unk92 = br.ReadUInt32();
        entry.Unk93 = br.ReadUInt16();
        entry.Unk93_2 = br.ReadUInt16();
        entry.Unk94 = br.ReadUInt16();
        entry.Unk94_2 = br.ReadUInt16();
        entry.Unk95 = br.ReadUInt32();
        entry.Unk96 = br.ReadUInt32();
        entry.Unk97 = br.ReadUInt16();
        entry.Unk97_2 = br.ReadUInt16();
        entry.Unk98 = br.ReadUInt32();
        entry.Unk99 = br.ReadUInt32();

        return entry;
    }

    public override string ToString() => $"{Id}\t\"{Name}\"\t{Lv}\t\"{Location}\"";
}

public class MobsScrFile2
{
    public List<MobsScrEntry2> Entries = new List<MobsScrEntry2>();

    public static MobsScrFile2 Load(string path)
    {
        var file = new MobsScrFile2();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            int entrySize = 488;
            while (br.BaseStream.Position + entrySize <= br.BaseStream.Length)
            {
                var entry = MobsScrEntry2.Read(br);
                file.Entries.Add(entry);
            }
        }

        return file;
    }
}
