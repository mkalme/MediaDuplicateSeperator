using System;

namespace MediaDuplicateSeperator {
    public class ImageComparer : IMediaComparer {
        public INodeProvider<LockedBitmap> BitmapProvider { get; set; }
        public bool DisposeAfterComparison { get; set; } = false;

        public ImageComparer() {
            BitmapProvider = new BitmapProvider();
        }

        public bool TryCompare(string firstPath, string secondPath, out bool equals) {
            try {
                LockedBitmap firstBitmap = BitmapProvider.ProvideNode(firstPath);
                LockedBitmap secondBitmap = BitmapProvider.ProvideNode(secondPath);
                
                equals = firstBitmap.Equals(secondBitmap);

                if (DisposeAfterComparison) {
                    firstBitmap.Dispose();
                    secondBitmap.Dispose();
                }
                
                return true;
            } catch {
                equals = false;
                return false;
            }
        }
    }
}
