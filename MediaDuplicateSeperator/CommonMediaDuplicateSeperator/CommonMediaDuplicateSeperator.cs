using System;
using System.Linq;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public class CommonMediaDuplicateSeperator : ICommonMediaDuplicateSeperator {
        public ISingleDuplicateSeperator DuplicateSeperator { get; set; }

        public bool StopAtFirstEncounter { get; set; } = false;

        public IEnumerable<CommonMediaDuplicate> SeperateDuplicates(IEnumerable<string> main, IEnumerable<IEnumerable<string>> other) {
            List<CommonMediaDuplicate> output = new List<CommonMediaDuplicate>();

            HashSet<string> taken = new HashSet<string>();
            foreach (var firstFile in main) {
                List<string> duplicates = new List<string>();

                foreach (IEnumerable<string> collection in other) {
                    IEnumerable<string> group = DuplicateSeperator.SeperateDuplicates(firstFile, collection.Where(x => !taken.Contains(x)));

                    if (StopAtFirstEncounter) {
                        foreach (var thisFile in group) {
                            taken.Add(thisFile);
                        }
                    }

                    duplicates.AddRange(group);
                }

                output.Add(new CommonMediaDuplicate() {
                    MainFile = firstFile,
                    Duplicates = duplicates
                });
            }

            return output;
        }
    }
}
