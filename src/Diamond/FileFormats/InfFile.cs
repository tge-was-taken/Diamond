using System.Text;

namespace Diamond.FileFormats;

public interface IInfEntry
{
    string FileName { get; set; }
    ulong Offset { get; set; }
    ulong Size { get; set; }
    void Read(BinaryReader br);
    void Write(BinaryWriter bw);
}

public static class BinaryReaderExtensions
{
    public static string ReadFixedString(this BinaryReader br, int length)
    {
        var bytes = br.ReadBytes(length);
        int nullIndex = Array.IndexOf<byte>(bytes, 0);
        if (nullIndex >= 0)
            return EucKrEncoding.Instance.GetString(bytes, 0, nullIndex);
        return EucKrEncoding.Instance.GetString(bytes);
    }
}


public static class BinaryWriterExtensions
{
    public static void WriteFixedString(this BinaryWriter bw, string text, int length)
    {
        var bytes = EucKrEncoding.Instance.GetBytes(text ?? "");
        Array.Resize(ref bytes, length);
        bw.Write(bytes);
    }
}

public class InfEntryV1 : IInfEntry
{
    public string FileName { get; set; } = string.Empty;
    public uint Offset { get; set; }
    public uint Size { get; set; }

    // Version 1 specific fields
    public uint F6C { get; set; }
    public uint F70 { get; set; }
    public uint F74 { get; set; }
    public uint F78 { get; set; }
    public uint F7C { get; set; }
    public uint F80 { get; set; }
    ulong IInfEntry.Offset { get => Offset; set => Offset = (uint)value; }
    ulong IInfEntry.Size { get => Size; set => Size = (uint)value; }

    public void Read(BinaryReader br)
    {
        FileName = br.ReadFixedString(100);
        Offset = br.ReadUInt32();
        Size = br.ReadUInt32();
        F6C = br.ReadUInt32();
        F70 = br.ReadUInt32();
        F74 = br.ReadUInt32();
        F78 = br.ReadUInt32();
        F7C = br.ReadUInt32();
        F80 = br.ReadUInt32();
    }

    public void Write(BinaryWriter bw)
    {
        bw.WriteFixedString(FileName, 100);
        bw.Write((uint)Offset);
        bw.Write((uint)Size);
        bw.Write(F6C);
        bw.Write(F70);
        bw.Write(F74);
        bw.Write(F78);
        bw.Write(F7C);
        bw.Write(F80);
    }
}

public class InfEntryV2 : IInfEntry
{
    public string FileName { get; set; } = string.Empty;
    public ulong Offset { get; set; }
    public ulong Size { get; set; }

    // Version 2 specific fields
    public ulong Hash1 { get; set; }
    public ulong Hash2 { get; set; }
    public ulong Hash3 { get; set; }

    public void Read(BinaryReader br)
    {
        FileName = br.ReadFixedString(104);
        Offset = br.ReadUInt64();
        Size = br.ReadUInt64();
        Hash1 = br.ReadUInt64();
        Hash2 = br.ReadUInt64();
        Hash3 = br.ReadUInt64();
    }

    public void Write(BinaryWriter bw)
    {
        bw.WriteFixedString(FileName, 104);
        bw.Write(Offset);
        bw.Write(Size);
        bw.Write(Hash1);
        bw.Write(Hash2);
        bw.Write(Hash3);
    }
}

public class InfFile
{
    public uint Magic1 { get; set; }
    public uint Magic2 { get; set; }
    public uint F08 { get; set; }
    public ulong VfsFileSize { get; set; }
    public int Version { get; set; }

    public List<IInfEntry> Entries { get; } = [];

    // ───────────────────────────
    // Reading
    // ───────────────────────────
    public static InfFile Load(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        // Peek ahead to determine version
        var checkpoint = fs.Position;
        fs.Position = 0x14;
        var version = br.ReadUInt32() == 0 ? 2 : 1;
        fs.Position = checkpoint;

        var inf = new InfFile
        {
            Magic1 = br.ReadUInt32(),
            Magic2 = br.ReadUInt32(),
            F08 = br.ReadUInt32(),
            Version = version
        };

        if (inf.Version == 2)
        {
            uint fileCount = br.ReadUInt32();
            inf.VfsFileSize = br.ReadUInt64();
            for (int i = 0; i < fileCount; i++)
            {
                var entry = new InfEntryV2();
                entry.Read(br);
                inf.Entries.Add(entry);
            }
        }
        else
        {
            uint fileCount = br.ReadUInt32();
            inf.VfsFileSize = br.ReadUInt32();
            for (int i = 0; i < fileCount; i++)
            {
                var entry = new InfEntryV1();
                entry.Read(br);
                inf.Entries.Add(entry);
            }
        }

        return inf;
    }

    // ───────────────────────────
    // Writing
    // ───────────────────────────
    public void Save(string path)
    {
        using var fs = File.Create(path);
        using var bw = new BinaryWriter(fs);

        bw.Write(Magic1);
        bw.Write(Magic2);
        bw.Write(F08);
        bw.Write((uint)Entries.Count);

        if (Version == 1)
        {
            bw.Write((uint)VfsFileSize);
            foreach (var e in Entries)
                e.Write(bw);
        }
        else
        {
            bw.Write(VfsFileSize);
            foreach (var e in Entries)
                e.Write(bw);
        }
    }
}