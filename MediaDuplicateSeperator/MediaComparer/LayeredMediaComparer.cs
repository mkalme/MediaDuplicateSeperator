using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public class LayeredMediaComparer : IMediaComparer {
        public ICollection<IMediaComparer> MediaComparers { get; set; }

        public bool TryCompare(string firstFile, string secondFile, out bool equals) {
            equals = false;

            foreach (var comparer in MediaComparers) {
                if (comparer.TryCompare(firstFile, secondFile, out equals)) {
                    if (!equals) return true;
                } else {
                    return false;
                }
            }

            return true;
        }
    }
}
