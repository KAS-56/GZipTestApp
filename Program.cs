using System;
using System.Collections.Generic;
using System.IO;
using GZipTestApp.Readers;
using GZipTestApp.Workers;
using GZipTestApp.Writers;

namespace GZipTestApp
{
    internal static class Program
    {
        private const int CorrectArgumentsCount = 3;
        private static readonly int MaxThreads = Environment.ProcessorCount;
        private const int BlockSize = 1024 * 1024;

        private static int Main(string[] args)
        {
            if (!ParseArguments(args, out var options))
            {
                ShowHowToUse();
                return 1;
            }

            bool isSucceeded = Run(options);

            return isSucceeded ? 1 : 0;
        }

        private static bool Run(Options options)
        {
            IWorker worker;
            IReader reader;
            IWriter writer = new ResultWriter(options.TargetFile, MaxThreads);
            switch (options.Mode)
            {
                case Mode.Compress:
                    worker = new CompressorWorker();
                    reader = new CompressorReader(BlockSize, options.SourceFile, MaxThreads);
                    break;
                case Mode.Decompress:
                    worker = new DecompressorWorker();
                    reader = new DecompressorReader(options.SourceFile, MaxThreads);
                    break;
                default:
                    Console.WriteLine("Unexpected mode");
                    return false;
            }

            Synchronizer synchronizer = new Synchronizer(MaxThreads, reader, writer, worker);
            return synchronizer.Run(options);
        }

        private static void ShowHowToUse()
        {
            string file = typeof(Program).Assembly.Location;
            string executableName = Path.GetFileNameWithoutExtension(file);
            Console.WriteLine($"Compress file to archive or decompress archive to file by using GZip.{Environment.NewLine}");
            Console.WriteLine($"Usage: {executableName} mode source_file target_file{Environment.NewLine}");
            Console.WriteLine("   mode\t\t - possible two modes of program run:");
            Console.WriteLine("\t\t   compress   - compress source_file to GZip archive with name target_file");
            Console.WriteLine("\t\t   decompress - decompress source_file GZip archive to file with name target_file");
            Console.WriteLine("   source_file\t - name or path to source file");
            Console.WriteLine("   target_file\t - name or path to target file");
        }

        private static bool ParseArguments(IReadOnlyList<string> args, out Options options)
        {
            options = new Options();
            if (args.Count != CorrectArgumentsCount)
            {
                return false;
            }

            if (!TryParseMode(args[0], out var mode) || !mode.HasValue)
            {
                return false;
            }

            options.Mode = mode.Value;
            options.SourceFile = args[1];
            options.TargetFile = args[2];

            return true;
        }

        private static bool TryParseMode(string arg, out Mode? mode)
        {
            if (arg.Equals(Mode.Compress.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                mode = Mode.Compress;
                return true;
            }

            if (arg.Equals(Mode.Decompress.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                mode = Mode.Decompress;
                return true;
            }

            mode = null;
            return false;
        }
    }
}
