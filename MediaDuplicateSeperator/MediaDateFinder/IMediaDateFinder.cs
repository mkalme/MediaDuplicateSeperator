using System;

namespace MediaDuplicateSeperator {
    public interface IMediaDateFinder {
        bool TryFindDate(string path, out DateTime dateTime);
    }
}
