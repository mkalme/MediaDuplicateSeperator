using System;
using System.Drawing;

namespace MediaDuplicateSeperator {
    public class ResizedBitmapProvider : INodeProvider<LockedBitmap> {
        public INodeProvider<LockedBitmap> BitmapProvider { get; set; }
        public Size Size { get; set; } = new Size(128, 128);

        public ResizedBitmapProvider(INodeProvider<LockedBitmap> bitmapProvider) {
            BitmapProvider = bitmapProvider;
        }

        public LockedBitmap ProvideNode(string nodePath) {
            using (LockedBitmap bitmap = BitmapProvider.ProvideNode(nodePath)) {
                bitmap.Unlock();

                return new LockedBitmap(new Bitmap(bitmap.Source, Size)) { 
                    Width = bitmap.Width,
                    Height = bitmap.Height
                };
            }
        }
    }
}
