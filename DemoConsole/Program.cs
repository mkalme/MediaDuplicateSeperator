using System;
using System.Diagnostics;

namespace DemoConsole {
    class Program {
        private static string InputFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Input";
        private static string OutputFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Output";
        private static string BaseFolder = "D:\\Backup\\Pictures & Videos on Phone\\Network, Attēli & Video";

        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            MediaSorter sorter = new MediaSorter(InputFolder, OutputFolder, BaseFolder);
            sorter.Sort();

            watch.Stop();
            Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
