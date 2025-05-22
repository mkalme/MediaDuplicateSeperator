using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace MediaDuplicateSeperator {
    static class VideoUtilities {
        private const string FfprobePath = "C:\\ffmpeg\\bin\\ffprobe.exe";

        public static TimeSpan GetDuration(string path, IDictionary<string, string> exifData) {
            TimeSpan duration = TimeSpan.FromSeconds(0);
            bool parsed = false;

            if (exifData.TryGetValue("Duration", out string durationTag)) {
                if (TimeSpan.TryParseExact(durationTag, "h\\:mm\\:ss", CultureInfo.InvariantCulture, out duration)) {
                    parsed = true;
                }
            }

            if (!parsed) {
                string arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{path}\"";
                string output = ProcessUtilities.ExecuteProcess(FfprobePath, arguments);

                duration = TimeSpan.FromSeconds(double.Parse(output.Replace(".", ",")));
            }

            return duration;
        }
        public static Size GetSize(string path, IDictionary<string, string> exifData) {
            string[] widthTags = { "Image Width", "Source Image Width" };
            string[] heightTags = { "Image Height", "Source Image Height" };

            if (CollectionExtensions.TryParseTag(widthTags, exifData, out int width) &&
                CollectionExtensions.TryParseTag(heightTags, exifData, out int height)) {
                return new Size(width, height);
            }

            string sizeOutput;

            if (!exifData.TryGetValue("Image Size", out sizeOutput)) {
                string arguments = $"-v error -select_streams v -show_entries stream=width,height -of csv=p=0:s=x \"{path}\"";
                sizeOutput = ProcessUtilities.ExecuteProcess(FfprobePath, arguments);
            }

            string[] entries = sizeOutput.Split("x");
            
            return new Size(int.Parse(entries[0]), int.Parse(entries[1]));
        }
        public static double GetFrameRate(string path, IDictionary<string, string> exifData) {
            if (exifData.TryGetValue("Video Frame Rate", out string tag) &&
                double.TryParse(tag.Replace(".", ","), out double frameRate)) {

                return frameRate;
            }

            string arguments = $"-v error -select_streams v -of default=noprint_wrappers=1:nokey=1 -show_entries stream=r_frame_rate \"{path}\"";
            string output = ProcessUtilities.ExecuteProcess(FfprobePath, arguments);

            string[] entires = output.Split("/");

            return double.Parse(entires[0]) / double.Parse(entires[1]);
        }
    }
}
