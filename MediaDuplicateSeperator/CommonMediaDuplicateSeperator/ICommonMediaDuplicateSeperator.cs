using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public interface ICommonMediaDuplicateSeperator {
        IEnumerable<CommonMediaDuplicate> SeperateDuplicates(IEnumerable<string> main, IEnumerable<IEnumerable<string>> other);
    }
}
