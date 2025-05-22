using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaDuplicateSeperator {
    public class ImageDateFinder : IMediaDateFinder {
        private static Regex r = new Regex(":");

        public bool TryFindDate(string path, out DateTime dateTime) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false)) {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);

                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);

                    dateTime = DateTime.Parse(dateTaken);
                }
            } catch {
                dateTime = File.GetLastWriteTime(path);
            }

            return true;
        }
    }
}
