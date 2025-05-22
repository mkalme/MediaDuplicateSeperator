using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediaDuplicateSeperator {
    public class SingleDuplicateSeperator : ISingleDuplicateSeperator {
        public IMediaComparer MediaComparer { get; set; }

        public IEnumerable<string> SeperateDuplicates(string mainFile, IEnumerable<string> files) {
            ConcurrentBag<string> output = new ConcurrentBag<string>();

            ThreadUtilities.ForEach(files, 4, file => {
                if (MediaComparer.TryCompare(mainFile, file, out bool equals) && equals) {
                    output.Add(file);
                }
            });

            return output;
        }
    }
}
