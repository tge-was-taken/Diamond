using System.Text.RegularExpressions;

namespace Diamond.FileFormats;

public class MobArrEntry
{
    public MobId MobId { get; set; }
    public ushort F2 { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Dx { get; set; }
    public float Dy { get; set; }

    public float Radius => MathF.Sqrt(Dx * Dx + Dy * Dy);
}

public class MobArrFile
{
    public MapId? MapId { get; private set; }
    public List<MobArrEntry> Entries { get; set; } = new();

    private static MapId? ExtractMapId(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(fileName))
            return null;

        var match = Regex.Match(fileName, @"(?:mob)\s*(\d{1,4})", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
            return (MapId)id;

        return null; // default if not found
    }

    public static MobArrFile Load(string path)
    {
        var data = File.ReadAllBytes(path);
        var file = new MobArrFile();
        file.MapId = ExtractMapId(path);

        using (var br = new BinaryReader(new MemoryStream(data)))
        {
            long entryCount = data.Length / 20;
            for (int i = 0; i < entryCount; i++)
            {
                var entry = new MobArrEntry
                {
                    MobId = (MobId)br.ReadUInt16(),
                    F2 = br.ReadUInt16(),
                    X = br.ReadSingle(),
                    Y = br.ReadSingle(),
                    Dx = br.ReadSingle(),
                    Dy = br.ReadSingle()
                };
                file.Entries.Add(entry);
            }
        }

        return file;
    }
}
