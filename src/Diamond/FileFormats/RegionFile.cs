namespace Diamond.FileFormats;

using Diamond;

using System;
using System.IO;
using System.Text.RegularExpressions;

public class RegionFile
{
    public uint Width { get; private set; }
    public uint Height { get; private set; }
    public byte[] RegionMap { get; private set; } = Array.Empty<byte>();
    public int OriginX { get; private set; }
    public int OriginY { get; private set; }
    public MapId? MapId { get; set; }

    private static MapId? ExtractMapId(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path);
        if (string.IsNullOrEmpty(fileName))
            return null;

        var match = Regex.Match(fileName, @"(?:region)\s*(\d{1,4})", RegexOptions.IgnoreCase);
        if (match.Success && int.TryParse(match.Groups[1].Value, out int id))
            return (MapId)id;

        return null; // default if not found
    }

    public static RegionFile Load(string path)
    {
        using var br = new BinaryReader(File.OpenRead(path));

        var region = new RegionFile
        {
            MapId = ExtractMapId(path),
            Width = br.ReadUInt32(),
            Height = br.ReadUInt32()
        };

        int totalCells = checked((int)(region.Width * region.Height));
        region.RegionMap = br.ReadBytes(totalCells);

        region.OriginX = br.ReadInt32();
        region.OriginY = br.ReadInt32();

        return region;
    }

    public int? GetRegionIndexFromLocal(float x, float y, float gridSize = 512f)
    {
        // Convert world coords → grid indices using origin + grid size
        int gridX = (int)((x - OriginX) / gridSize);
        int gridY = (int)((y - OriginY) / gridSize);

        if (gridX < 0 || gridY < 0 || gridX >= Height || gridY >= Width)
            return null;

        return RegionMap[gridY * (int)Height + gridX];
    }

    public int? GetRegionIndexFromWorld(float x, float y, float gridSize = 512f)
    {
        // Convert world coords → grid indices using origin + grid size
        int gridX = (int)((x - OriginX) / gridSize);
        int gridY = (int)((y - OriginY) / gridSize);

        if (gridX < 0 || gridY < 0 || gridX >= Height || gridY >= Width)
            return null;

        return RegionMap[gridY * (int)Height + gridX];
    }

    public override string ToString()
    {
        return $"RegionFile: {Height}x{Width}, Origin=({OriginX},{OriginY})";
    }
}