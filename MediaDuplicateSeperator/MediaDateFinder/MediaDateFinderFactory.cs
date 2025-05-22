using System;
using System.IO;

namespace MediaDuplicateSeperator {
    public class MediaDateFinderFactory : IMediaDateFinderFactory {
        public IMediaDateFinder CreateMediaDateFinder(string path) {
            string extension = Path.GetExtension(path).ToLower();

            if (MediaUtilities.ImageFormats.Contains(extension)) return new ImageDateFinder();
            else if (MediaUtilities.VideoFormats.Contains(extension)) return new VideoDateFinder();

            return null;
        }
    }
}
