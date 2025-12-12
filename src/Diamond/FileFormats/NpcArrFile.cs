using System.Text.RegularExpressions;

namespace Diamond.FileFormats;

public class NpcArrEntry
{
    public NpcId NpcId { get; set; }
    public ushort F2 { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float AngleRadian { get; set; }
    public uint F3 { get; set; }
    public uint F4 { get; set; }
    public uint F5 { get; set; }

    public float AngleDegrees => AngleRadian * (180f / MathF.PI);
}

public class NpcArrFile
{
    public MapId? MapId { get; private set; }
    public List<NpcArrEntry> Entries { get; set; } = new();

    private static MapId? ExtractMapId(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(fileName))
            return null;

        var match = Regex.Match(fileName, @"(?:npc)\s*(\d{1,4})", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
            return (MapId)id;

        return null; // default if not found
    }

    public static NpcArrFile Load(string path)
    {
        var data = File.ReadAllBytes(path);
        var file = new NpcArrFile();
        file.MapId = ExtractMapId(path);

        using (var br = new BinaryReader(new MemoryStream(data)))
        {
            long entryCount = data.Length / 28;
            for (int i = 0; i < entryCount; i++)
            {
                var e = new NpcArrEntry
                {
                    NpcId = (NpcId)br.ReadUInt16(),
                    F2 = br.ReadUInt16(),
                    X = br.ReadSingle(),
                    Y = br.ReadSingle(),
                    AngleRadian = br.ReadSingle(),
                    F3 = br.ReadUInt32(),
                    F4 = br.ReadUInt32(),
                    F5 = br.ReadUInt32()
                };
                file.Entries.Add(e);
            }
        }

        return file;
    }
}
