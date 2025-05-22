using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public class AllMediaDuplicateSeperator : IAllMediaDuplicateSeperator {
        public ISingleDuplicateSeperator DuplicateSeperator { get; set; }

        public IEnumerable<IEnumerable<string>> SeperateDuplicates(IEnumerable<string> files) {
            List<IEnumerable<string>> output = new List<IEnumerable<string>>();

            int count = 0;

            HashSet<string> free = new HashSet<string>(files);
            foreach (var mainFile in files) {
                count++;

                if (!free.Contains(mainFile)) continue;
                free.Remove(mainFile);

                List<string> group = new List<string>() { mainFile };
                group.AddRange(DuplicateSeperator.SeperateDuplicates(mainFile, free));

                output.Add(group);
                foreach (var taken in group) {
                    free.Remove(taken);
                }

                Console.WriteLine(count);
            }

            return output;
        }
    }
}
