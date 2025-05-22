using System;
using System.IO;

namespace MediaDuplicateSeperator {
    public class ShallowMediaComparerFactory : MediaComparerFactory {
        public CachedNodeProvider<LockedBitmap> CachedBitmapProvider { get; set; } = new CachedNodeProvider<LockedBitmap>(new ResizedBitmapProvider(new BitmapProvider())) {
            PathValidator = path => MediaUtilities.ImageFormats.Contains(Path.GetExtension(path)),
            DisposeFunction = bitmap => bitmap.Dispose()
        };
        public CachedNodeProvider<Video> CachedVideoProvider { get; set; } = new CachedNodeProvider<Video>(new ShallowVideoProvider()) {
            PathValidator = path => MediaUtilities.VideoFormats.Contains(Path.GetExtension(path)),
            DisposeFunction = video => video.Dispose()
        };

        protected override IMediaComparer CreateImageComparer() {
            return new ImageComparer() { 
                BitmapProvider = CachedBitmapProvider
            };
        }
        protected override IMediaComparer CreateVideoComparer() {
            return new VideoComparer() {
                VideoProvider = CachedVideoProvider
            };
        }
    }
}
