using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public class CommonMediaDuplicate {
        public string MainFile { get; set; }
        public IEnumerable<string> Duplicates { get; set; }
    }
}
