using System.Text;

namespace Diamond.FileFormats;

public class NpcsScrEntryInventoryItem
{
    public uint Unk;
    public ItemId ItemId;
    public uint[] Unk2; // size 2

    public static NpcsScrEntryInventoryItem Read(BinaryReader br)
    {
        var item = new NpcsScrEntryInventoryItem();
        item.Unk = br.ReadUInt32();
        item.ItemId = (ItemId)br.ReadUInt32();
        item.Unk2 = new uint[2] { br.ReadUInt32(), br.ReadUInt32() };
        return item;
    }
}

public class NpcsScrEntry
{
    public NpcId Id;
    public string Name;
    public string Title;
    public byte[] Unk; // size 64
    public ushort ModelId;
    public ushort NpcType;
    public uint Unk4;
    public NpcsScrEntryInventoryItem[] Inventory; // size 60
    public QuestId GiveQuest1, GiveQuest2, GiveQuest3, GiveQuest4, GiveQuest5, GiveQuest6;
    public uint Unk8_3, Unk9;
    public uint[] Unk10; // size 3
    public uint Unk11;
    public uint[] Unk12; // size 7
    public QuestId RewardQuest;
    public ushort Unk14;
    public uint Unk15;
    public byte[] Unk16; // size 4
    public ushort Unk17;
    public QuestId RewardQuest2;
    public byte Unk19_1, Unk19_2, Unk19_3, Unk19_4;
    public uint DefaultDialogId, EventDialogId, IdleDialogId, QuestDialogId, QuestDialogId2;
    public uint[] Unk24; // size 16
    public string Map;
    public string Location;
    public string Category;
    public string Category2;

    public static NpcsScrEntry Read(BinaryReader br)
    {
        var entry = new NpcsScrEntry();
        entry.Id = (NpcId)br.ReadUInt16();
        entry.Name = EucKrEncoding.Instance.GetString(br.ReadBytes(17)).TrimEnd('\0');
        entry.Title = EucKrEncoding.Instance.GetString(br.ReadBytes(33)).TrimEnd('\0');
        entry.Unk = br.ReadBytes(64);
        entry.ModelId = br.ReadUInt16();
        entry.NpcType = br.ReadUInt16();
        entry.Unk4 = br.ReadUInt32();

        entry.Inventory = new NpcsScrEntryInventoryItem[60];
        for (int i = 0; i < 60; i++)
            entry.Inventory[i] = NpcsScrEntryInventoryItem.Read(br);

        entry.GiveQuest1 = (QuestId)br.ReadUInt16();
        entry.GiveQuest2 = (QuestId)br.ReadUInt16();
        entry.GiveQuest3 = (QuestId)br.ReadUInt16();
        entry.GiveQuest4 = (QuestId)br.ReadUInt16();
        entry.GiveQuest5 = (QuestId)br.ReadUInt16();
        entry.GiveQuest6 = (QuestId)br.ReadUInt16();

        entry.Unk8_3 = br.ReadUInt32();
        entry.Unk9 = br.ReadUInt32();
        entry.Unk10 = new uint[3] { br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32() };
        entry.Unk11 = br.ReadUInt32();
        entry.Unk12 = new uint[7];
        for (int i = 0; i < 7; i++) entry.Unk12[i] = br.ReadUInt32();

        entry.RewardQuest = (QuestId)br.ReadUInt16();
        entry.Unk14 = br.ReadUInt16();
        entry.Unk15 = br.ReadUInt32();
        entry.Unk16 = br.ReadBytes(4);
        entry.Unk17 = br.ReadUInt16();
        entry.RewardQuest2 = (QuestId)br.ReadUInt16();
        entry.Unk19_1 = br.ReadByte();
        entry.Unk19_2 = br.ReadByte();
        entry.Unk19_3 = br.ReadByte();
        entry.Unk19_4 = br.ReadByte();

        entry.DefaultDialogId = br.ReadUInt32();
        entry.EventDialogId = br.ReadUInt32();
        entry.IdleDialogId = br.ReadUInt32();
        entry.QuestDialogId = br.ReadUInt32();
        entry.QuestDialogId2 = br.ReadUInt32();

        entry.Unk24 = new uint[16];
        for (int i = 0; i < 16; i++) entry.Unk24[i] = br.ReadUInt32();

        entry.Map = EucKrEncoding.Instance.GetString(br.ReadBytes(20)).TrimEnd('\0');
        entry.Location = EucKrEncoding.Instance.GetString(br.ReadBytes(20)).TrimEnd('\0');
        entry.Category = EucKrEncoding.Instance.GetString(br.ReadBytes(22)).TrimEnd('\0');
        entry.Category2 = EucKrEncoding.Instance.GetString(br.ReadBytes(602)).TrimEnd('\0');

        return entry;
    }
}

public class NpcsScrFile
{
    public NpcsScrEntry[] Entries;

    public static NpcsScrFile Load(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs);

        long fileSize = fs.Length;
        int entryCount = (int)(fileSize / 0x77C);
        var file = new NpcsScrFile();
        file.Entries = new NpcsScrEntry[entryCount];

        for (int i = 0; i < entryCount; i++)
            file.Entries[i] = NpcsScrEntry.Read(br);

        return file;
    }
}
