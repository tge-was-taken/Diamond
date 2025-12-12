using System;
using System.IO;
using Diamond.FileFormats;
using System.Linq;
using System.Diagnostics;

namespace Diamond.MotCmd;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: Diamond.MotCmd.exe -i <input> -o <output> -fps <fps>");
            return;
        }

        string? inputPath = null;
        string? outputPath = null;
        float targetFps = 60f;
        bool linear = true;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-i": inputPath = args[++i]; break;
                case "-o": outputPath = args[++i]; break;
                case "-fps": targetFps = float.Parse(args[++i]); break;
                case "-linear": linear = true; break;
            }
        }

        if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

        var files = File.Exists(inputPath)
            ? new string[] { inputPath }
            : Directory.GetFiles(inputPath, "*.mot");

        foreach (var filePath in files)
        {
            Console.WriteLine($"Processing {filePath}...");
            try
            {
                var motFile = MotFile.Read(filePath);

                //if (Path.GetFileNameWithoutExtension(filePath) == "g170399907")
                //    Debugger.Break();

                foreach (var bone in motFile.Bones)
                {
                    bone.Keys = linear ?
                        MotResampler.ResampleKeysLinear(bone.Keys, 20, 60) :
                        MotResampler.ResampleKeysSmooth(bone.Keys, 20, 60);
                }

                string outPath = Path.Combine(outputPath, Path.GetFileName(filePath));
                motFile.Write(outPath);
                Console.WriteLine($"Saved interpolated file to {outPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        Console.WriteLine("All files processed.");
    }
}
