using System;
using System.IO;
using System.IO.Compression;

namespace MediaDuplicateSeperator {
    public class GZipCompression : IStreamModifier {
        public void ModifyStream(Stream input, Stream output) {
            using (GZipStream zc = new GZipStream(output, CompressionMode.Compress)) {
                input.CopyTo(zc);
                zc.Flush();
            }
        }
    }
}
