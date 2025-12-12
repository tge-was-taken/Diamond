using System.Text.RegularExpressions;

namespace Diamond.FileFormats;

public class RegionTableEntry
{
    public string Name { get; set; } = "";
    public float X { get; set; }
    public float Y { get; set; }
    public uint Unk1 { get; set; }
    public uint Unk2 { get; set; }

    public override string ToString() => $"{Name} ({X}, {Y})";
}

public class RegionTableFile
{
    public MapId? MapId { get; private set; }
    public List<RegionTableEntry> Entries { get; } = new();
    public List<uint> Unk { get; } = new();

    private static MapId? ExtractMapId(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(fileName))
            return null;

        var match = Regex.Match(fileName, @"(?:regiontable)\s*(\d{1,4})", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
            return (MapId)id;

        return null; // default if not found
    }

    public static RegionTableFile Load(string path)
    {
        var file = new RegionTableFile();
        file.MapId = ExtractMapId(path);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs, EucKrEncoding.Instance);

        // There are always 32 entries
        for (int i = 0; i < 32; i++)
        {
            var nameBytes = br.ReadBytes(32);
            var name = EucKrEncoding.Instance.GetString(nameBytes).TrimEnd('\0');
            var x = br.ReadSingle();
            var y = br.ReadSingle();
            var unk1 = br.ReadUInt32();
            var unk2 = br.ReadUInt32();

            file.Entries.Add(new RegionTableEntry
            {
                Name = name,
                X = x,
                Y = y,
                Unk1 = unk1,
                Unk2 = unk2
            });
        }

        // The template has 32 uints after the entries
        for (int i = 0; i < 32; i++)
        {
            file.Unk.Add(br.ReadUInt32());
        }

        return file;
    }
}
