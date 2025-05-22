using System;

namespace MediaDuplicateSeperator {
    public interface IMediaComparer {
        bool TryCompare(string firstFile, string secondFile, out bool equals);
    }
}
