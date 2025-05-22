using System;

namespace MediaDuplicateSeperator {
    public interface IMediaDateFinderFactory {
        IMediaDateFinder CreateMediaDateFinder(string path);
    }
}
