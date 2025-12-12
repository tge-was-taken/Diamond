using System.Text;

namespace Diamond.FileFormats;

public class ItemsScrEntry
{
    public string Name;         // 52 bytes
    public ItemId Id;
    public uint[] Unk1;         // 72 / 4 = 18 uints
    public ItemId BaseId;
    public uint CategoryId;

    public static ItemsScrEntry Read(BinaryReader br, int size)
    {
        long startPos = br.BaseStream.Position;
        var entry = new ItemsScrEntry();
        entry.Name = EucKrEncoding.Instance.GetString(br.ReadBytes(52)).TrimEnd('\0');
        entry.Id = (ItemId)br.ReadUInt32();

        entry.Unk1 = new uint[18];
        for (int i = 0; i < 18; i++)
            entry.Unk1[i] = br.ReadUInt32();

        entry.BaseId = (ItemId)br.ReadUInt32();
        entry.CategoryId = br.ReadUInt32();

        // Seek to the end of this entry, based on the size parameter
        long endPos = startPos + size;
        br.BaseStream.Seek(endPos, SeekOrigin.Begin);

        return entry;
    }

    public override string ToString()
    {
        return $"#{Id} '{Name}'";
    }
}

public class ItemsScrFile
{
    public List<ItemsScrEntry> Entries = new List<ItemsScrEntry>();

    public static ItemsScrFile Load(string path)
    {
        var file = new ItemsScrFile();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (var br = new BinaryReader(fs))
        {
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                long currentPos = br.BaseStream.Position;

                // Read entry (fixed 0x224 bytes)
                var entry = ItemsScrEntry.Read(br, 0x224);

                var endPos = br.BaseStream.Position;

                // Conditional checks like the template
                if (endPos + 0x244 < br.BaseStream.Length)
                {
                    fs.Seek(endPos + 0x34, SeekOrigin.Begin);
                    uint checkId = br.ReadUInt32();
                    fs.Seek(endPos + 0x80, SeekOrigin.Begin);
                    uint checkBaseId = br.ReadUInt32();

                    if (checkId < 100_000_000 || checkBaseId < 100_000_000)
                    {
                        // Optional unknown data structure
                        fs.Seek(endPos + 8, SeekOrigin.Begin); // Skip 8 bytes placeholder
                        endPos = br.BaseStream.Position;
                        if (endPos + 0x244 < br.BaseStream.Length)
                        {
                            fs.Seek(endPos + 0x34, SeekOrigin.Begin);
                            checkId = br.ReadUInt32();
                            fs.Seek(endPos + 0x80, SeekOrigin.Begin);
                            checkBaseId = br.ReadUInt32();

                            if (checkId < 100_000_000 || checkBaseId < 100_000_000)
                            {
                                // Optional unknown data structure
                                fs.Seek(endPos + 8, SeekOrigin.Begin); // Skip 8 bytes placeholder
                                endPos = br.BaseStream.Position;
                                if (endPos + 0x244 < br.BaseStream.Length)
                                {
                                    fs.Seek(endPos + 0x34, SeekOrigin.Begin);
                                    checkId = br.ReadUInt32();
                                    fs.Seek(endPos + 0x80, SeekOrigin.Begin);
                                    checkBaseId = br.ReadUInt32();

                                    if (checkId < 100_000_000 || checkBaseId < 100_000_000)
                                    {
                                        // Optional unknown data structure
                                        fs.Seek(endPos + 8, SeekOrigin.Begin); // Skip 8 bytes placeholder
                                        endPos = br.BaseStream.Position;
                                    }
                                }
                            }
                        }
                    }
                }

                // Add the entry
                file.Entries.Add(entry);
                br.BaseStream.Seek(endPos, SeekOrigin.Begin);
            }
        }

        return file;
    }
}