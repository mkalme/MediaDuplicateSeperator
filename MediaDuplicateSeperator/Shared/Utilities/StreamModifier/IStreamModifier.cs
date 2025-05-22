using System;
using System.IO;

namespace MediaDuplicateSeperator {
    public interface IStreamModifier {
        void ModifyStream(Stream input, Stream output);
    }
}
