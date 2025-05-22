using System;

namespace MediaDuplicateSeperator {
    public interface IMediaComparerFactory {
        IMediaComparer CreateMediaComparer(string firstPath, string secondPath);
    }
}
