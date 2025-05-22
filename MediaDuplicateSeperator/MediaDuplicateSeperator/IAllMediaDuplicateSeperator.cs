using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public interface IAllMediaDuplicateSeperator {
        IEnumerable<IEnumerable<string>> SeperateDuplicates(IEnumerable<string> files);
    }
}
