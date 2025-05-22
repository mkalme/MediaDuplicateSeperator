using System;
using System.IO;

namespace MediaDuplicateSeperator {
    public class MediaComparerFactory : IMediaComparerFactory {
        public CachedNodeProvider<LockedBitmap> BitmapCache { get; set; }
        public CachedNodeProvider<Video> VideoCache { get; set; }

        public IMediaComparer CreateMediaComparer(string firstPath, string secondPath) {
            if (BitmapCache.Cache.ContainsKey(firstPath) && BitmapCache.Cache.ContainsKey(secondPath)) return CreateImageComparer();
            else if (VideoCache.Cache.ContainsKey(firstPath) && VideoCache.Cache.ContainsKey(secondPath)) return CreateVideoComparer();

            return null;
        }

        protected virtual IMediaComparer CreateImageComparer() {
            return new ImageComparer() { 
                DisposeAfterComparison = true
            };
        }
        protected virtual IMediaComparer CreateVideoComparer() {
            return new VideoComparer() { 
                Shallow = false
            };
        }
    }
}
