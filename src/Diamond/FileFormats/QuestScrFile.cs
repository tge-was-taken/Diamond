namespace Diamond.FileFormats;

public class QuestScrEntryStep
{
    public string Name;
    public ushort Unk2;
    public uint Count;
    public uint[] Unk; // size 3
    public MobId MobId;
    public ushort Unk10;
    public uint[] Unk3; // size 5
    public uint ItemId;
    public uint[] Unk5; // size 4
    public string Description; // 68 bytes
    public uint Exp;
    public uint Fame;
    public uint Gold;
    public uint[] Unk9; // size 9

    public static QuestScrEntryStep Read(BinaryReader br)
    {
        var start = br.BaseStream.Position;
        var step = new QuestScrEntryStep();
        step.Name = EucKrEncoding.Instance.GetString(br.ReadBytes(62)).TrimEnd('\0');
        step.Unk2 = br.ReadUInt16();
        step.Count = br.ReadUInt32();
        step.Unk = new uint[3];
        for (int i = 0; i < 3; i++) step.Unk[i] = br.ReadUInt32();
        step.MobId = (MobId)br.ReadUInt16();
        step.Unk10 = br.ReadUInt16();
        step.Unk3 = new uint[5];
        for (int i = 0; i < 5; i++) step.Unk3[i] = br.ReadUInt32();
        step.ItemId = br.ReadUInt32();
        step.Unk5 = new uint[4];
        for (int i = 0; i < 4; i++) step.Unk5[i] = br.ReadUInt32();
        step.Description = EucKrEncoding.Instance.GetString(br.ReadBytes(68)).TrimEnd('\0');
        step.Exp = br.ReadUInt32();
        step.Fame = br.ReadUInt32();
        step.Gold = br.ReadUInt32();
        step.Unk9 = new uint[9];
        for (int i = 0; i < 9; i++) step.Unk9[i] = br.ReadUInt32();

        // Seek to end of step (0xF0 bytes)
        br.BaseStream.Seek(start+0xF0, SeekOrigin.Begin);
        return step;
    }
}

public class QuestScrEntry
{
    public QuestId Id;
    public byte Unk;
    public string Name;
    public byte Unk1Count;
    public byte[] Unk1; // size 7
    public uint[] MessageIds; // size 5
    public uint Unk3_0;
    public uint Unk3_1;
    public uint StepCount;
    public QuestScrEntryStep[] Steps; // size 20
    public byte[] Unk5; // ubyte array to cover all 40 bytes
    public byte FactionQuestType;
    public FactionId FactionAfterCompletion;
    public ushort MinLv;
    public ushort MaxLv;
    public byte Unk6;
    public byte AllowWarrior;
    public byte AllowAssassin;
    public byte AllowMage;
    public byte AllowMonk;
    public FactionId RequiredFaction;
    public byte Unk7;
    public byte Unk8;
    public byte Unk9;
    public byte IsEvent;
    public byte Unk11;
    public byte Unk12;

    public static QuestScrEntry Read(BinaryReader br)
    {
        var entry = new QuestScrEntry();
        long startPos = br.BaseStream.Position;

        entry.Id = (QuestId)br.ReadUInt16();
        entry.Unk = br.ReadByte();
        entry.Name = EucKrEncoding.Instance.GetString(br.ReadBytes(61)).TrimEnd('\0');
        entry.Unk1Count = br.ReadByte();
        entry.Unk1 = br.ReadBytes(7);
        entry.MessageIds = new uint[5];
        for (int i = 0; i < 5; i++) entry.MessageIds[i] = br.ReadUInt32();
        entry.Unk3_0 = br.ReadUInt32();
        entry.Unk3_1 = br.ReadUInt32();
        entry.StepCount = br.ReadUInt32();

        entry.Steps = new QuestScrEntryStep[20];
        for (int i = 0; i < 20; i++)
            entry.Steps[i] = QuestScrEntryStep.Read(br);

        entry.Unk5 = br.ReadBytes(28);
        entry.FactionQuestType = br.ReadByte();
        entry.FactionAfterCompletion = (FactionId)br.ReadByte();
        br.ReadBytes(10);
        entry.MinLv = br.ReadUInt16();
        entry.MaxLv = br.ReadUInt16();
        entry.Unk6 = br.ReadByte();
        entry.AllowWarrior = br.ReadByte();
        entry.AllowAssassin = br.ReadByte();
        entry.AllowMage = br.ReadByte();
        entry.AllowMonk = br.ReadByte();
        entry.RequiredFaction = (FactionId)br.ReadByte();
        entry.Unk7 = br.ReadByte();
        entry.Unk8 = br.ReadByte();
        entry.Unk9 = br.ReadByte();
        entry.IsEvent = br.ReadByte();
        entry.Unk11 = br.ReadByte();
        entry.Unk12 = br.ReadByte();

        // Seek to end of entry (0x1360 bytes)
        long endPos = startPos + 0x1360;
        br.BaseStream.Seek(endPos, SeekOrigin.Begin);

        // Assertions (optional, can throw if violated)
        if (entry.Unk3_0 != 0 || entry.Unk3_1 != 0) Console.WriteLine($"Warning: unexpected unk3 values in quest {entry.Id}");

        return entry;
    }
}

public class QuestScrFile
{
    public QuestScrEntry[] Entries;

    public static QuestScrFile Load(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        long fileSize = fs.Length;
        int entryCount = (int)(fileSize / 0x1360);
        var file = new QuestScrFile();
        file.Entries = new QuestScrEntry[entryCount];

        for (int i = 0; i < entryCount; i++)
            file.Entries[i] = QuestScrEntry.Read(br);

        return file;
    }
}