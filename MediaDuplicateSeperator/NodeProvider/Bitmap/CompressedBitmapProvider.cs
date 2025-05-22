using System;
using System.IO;

namespace MediaDuplicateSeperator {
    public class CompressedBitmapProvider : INodeProvider<LockedBitmap> {
        public INodeProvider<LockedBitmap> BitmapProvider { get; set; }
        public IStreamModifier Compressor { get; set; } = new GZipCompression();

        public CompressedBitmapProvider(INodeProvider<LockedBitmap> bitmapProvider) {
            BitmapProvider = bitmapProvider;
        }

        public LockedBitmap ProvideNode(string file) {
            using (LockedBitmap bitmap = BitmapProvider.ProvideNode(file))
            using (MemoryStream input = new MemoryStream(bitmap.RGBValues))
            using (MemoryStream output = new MemoryStream()) {
                Compressor.ModifyStream(input, output);

                return new LockedBitmap() {
                    Source = bitmap.Source,
                    Width = bitmap.Width,
                    Height = bitmap.Height,
                    RGBValues = output.ToArray()
                };
            }
        }
    }
}
