using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace MediaDuplicateSeperator {
    public class Video : IDisposable {
        private bool _disposedValue;

        public string FilePath { get; set; }

        public Size Size { get; set; }
        public TimeSpan Duration { get; set; }
        public double FrameRate { get; set; }
        public long FrameCount => (long)Math.Floor(Duration.TotalSeconds * FrameRate);

        public INodeProvider<LockedBitmap> BitmapReader { get; set; } = new CompressedBitmapProvider(new BitmapProvider());

        public IList<LockedBitmap> Frames { get; set; } = new List<LockedBitmap>();

        public void Open(string path) {
            FilePath = path;

            IDictionary<string, string> tags = CollectionExtensions.ExtractMetadata(ProcessUtilities.ExecuteProcess("D:\\exiftool.exe", $"\"{path}\"", 5000));

            Size = VideoUtilities.GetSize(path, tags);
            Duration = VideoUtilities.GetDuration(path, tags);
            FrameRate = VideoUtilities.GetFrameRate(path, tags);
        }
        public bool LoadFrames(long frameCount) {
            frameCount = Math.Min(frameCount, FrameCount);

            string outputDirectory = $"{Path.GetTempPath()}\\{Path.GetRandomFileName()}";
            string arguments = $"-ss 00:00:00 -i \"{FilePath}\" -frames:v {frameCount} \"{outputDirectory}\\frame_%000d.bmp\"";

            Directory.CreateDirectory(outputDirectory);

            int exitCode = ProcessUtilities.ExecuteProcessWithExitCode("C:\\ffmpeg\\bin\\ffmpeg.exe", arguments);
            if (exitCode == 0) {
                try {
                    LockedBitmap[] bitmaps = new LockedBitmap[frameCount];

                    Parallel.For(0, frameCount, i => {
                        bitmaps[i] = BitmapReader.ProvideNode($"{outputDirectory}\\frame_{i + 1}.bmp");
                        bitmaps[i].Source.Dispose();
                    });

                    foreach (var bitmap in bitmaps) {
                        Frames.Add(bitmap);
                    }
                } catch {
                    exitCode = 1;
                }
            }

            Directory.Delete(outputDirectory, true);

            return exitCode == 0;
        }

        public bool Equals(Video other, bool shallow = false) {
            if (Size != other.Size || Duration != other.Duration || FrameRate != other.FrameRate) return false;
            if (Frames.Count != other.Frames.Count) return false;

            if (shallow) return true;

            bool equals = true;
            Parallel.For(0, Frames.Count, (i, state) => {
                if (!Frames[i].Equals(other.Frames[i])) {
                    state.Stop();
                    equals = false;
                }
            });

            return equals;
        }
        public void Dispose() {
            if (!_disposedValue) {
                foreach (var bitmap in Frames) {
                    bitmap.Dispose();
                }

                Frames.Clear();
                _disposedValue = true;
            }

            GC.SuppressFinalize(this);
        }
    }
}
