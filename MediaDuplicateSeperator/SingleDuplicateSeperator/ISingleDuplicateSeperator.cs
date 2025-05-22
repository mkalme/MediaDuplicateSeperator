using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public interface ISingleDuplicateSeperator {
        IEnumerable<string> SeperateDuplicates(string mainFile, IEnumerable<string> files);
    }
}
