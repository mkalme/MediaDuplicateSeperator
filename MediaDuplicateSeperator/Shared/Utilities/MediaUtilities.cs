using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public static class MediaUtilities {
        public static IReadOnlySet<string> ImageFormats => new HashSet<string>() {
            ".tif", ".tiff", ".bmp", ".jpg", ".jpeg", ".png"
        };
        public static IReadOnlySet<string> VideoFormats => new HashSet<string>() {
            ".mp4", ".3gp", ".avi", ".mov", ".gif"
        };
    }
}
