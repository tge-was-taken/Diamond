using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Diamond.FileFormats;

// ----------------- Main Program -----------------
class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (args.Length == 0) { PrintUsage(); return; }

        string cmd = args[0].ToLower();
        switch (cmd)
        {
            case "read":
                if (args.Length != 2) throw new ArgumentException("Usage: read <file.inf>");
                ReadInf(args[1]); break;
            case "extract":
                if (args.Length != 4) throw new ArgumentException("Usage: extract <file.inf> <file.vfs> <output_folder>");
                Extract(args[1], args[2], args[3]); break;
            default: PrintUsage(); break;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  read <file.inf>");
        Console.WriteLine("  extract <file.inf> <file.vfs> <output_folder>");
    }

    // ----------------- Core Operations -----------------
    static void ReadInf(string infPath)
    {
        var file = InfFile.Load(infPath);
        Console.WriteLine($"Version: {file.Version}");
        int i = 1;
        foreach (var e in file.Entries)
        {
            if (file.Version == 1)
            {
                var ent = (InfEntryV1)e;
                Console.WriteLine($"{i:D3}: {ent.FileName} Offset={ent.Offset} Size={ent.Size}");
            }
            else
            {
                var ent = (InfEntryV2)e;
                Console.WriteLine($"{i:D3}: {ent.FileName} Offset={ent.Offset} Size={ent.Size}");
            }
            i++;
        }
    }

    static void Extract(string infPath, string vfsPath, string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        var file = InfFile.Load(infPath);
        using var brVfs = new BinaryReader(File.OpenRead(vfsPath));

        foreach (var e in file.Entries)
        {
            string outPath;
            ulong offset, size;
            if (file.Version == 1)
            {
                var ent = (InfEntryV1)e;
                outPath = Path.Combine(outputDir, ent.FileName);
                offset = ent.Offset; size = ent.Size;
            }
            else
            {
                var ent = (InfEntryV2)e;
                outPath = Path.Combine(outputDir, ent.FileName);
                offset = ent.Offset; size = ent.Size;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? ".");
            brVfs.BaseStream.Seek((long)offset, SeekOrigin.Begin);
            File.WriteAllBytes(outPath, brVfs.ReadBytes((int)size));
            Console.WriteLine($"Extracted: {outPath}");
        }
    }
}
