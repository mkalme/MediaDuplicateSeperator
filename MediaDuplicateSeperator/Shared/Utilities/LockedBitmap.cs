using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace MediaDuplicateSeperator {
    public class LockedBitmap : IEquatable<LockedBitmap>, IDisposable {
        private bool _disposedValue;

        public Bitmap Source { get; set; }
        public BitmapData Data { get; set; }
        public byte[] RGBValues { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public LockedBitmap() { }
        public LockedBitmap(Bitmap bitmap) {
            Lock(bitmap);
        }
        public LockedBitmap(string path){
            var bytes = File.ReadAllBytes(path);

            using (var ms = new MemoryStream(bytes)) {
                Lock((Bitmap)Image.FromStream(ms));
            }
        }

        public void Lock(Bitmap bitmap) {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(data.Scan0, rgbValues, 0, bytes);

            Source = bitmap;
            Data = data;
            RGBValues = rgbValues;
            Width = bitmap.Width;
            Height = bitmap.Height;
        }
        public void Unlock() {
            if (Data == null) {
                using (MemoryStream stream = new MemoryStream(RGBValues)) {
                    Source = new Bitmap(stream);
                }
            } else {
                Marshal.Copy(RGBValues, 0, Data.Scan0, RGBValues.Length);
                Source.UnlockBits(Data);
            }
        }

        public bool Equals(LockedBitmap other) {
            if (Width != other.Width || Height != other.Height) return false;

            for (int i = 0; i < RGBValues.Length; i++) {
                if (RGBValues[i] != other.RGBValues[i]) return false;
            }

            return true;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (disposing) {
                    RGBValues = new byte[0];
                }

                Source.Dispose();
                _disposedValue = true;
            }
        }
    }
}