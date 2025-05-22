using System;

namespace MediaDuplicateSeperator {
    public class MediaComparer : IMediaComparer {
        public IMediaComparerFactory MediaComparerFactory { get; set; }

        public MediaComparer() {
            MediaComparerFactory = new MediaComparerFactory();
        }

        public bool TryCompare(string firstPath, string secondPath, out bool equals) {
            IMediaComparer mediaComparer = MediaComparerFactory.CreateMediaComparer(firstPath, secondPath);

            if (mediaComparer != null) {
                try {
                    return mediaComparer.TryCompare(firstPath, secondPath, out equals);
                } catch { }
            }

            equals = false;
            return false;
        }
    }
}
