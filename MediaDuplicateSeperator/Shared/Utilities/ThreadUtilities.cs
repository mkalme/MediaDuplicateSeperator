using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaDuplicateSeperator {
    public static class ThreadUtilities {
        public static void ForEach<TSource>(IEnumerable<TSource> source, int buffer, Action<TSource> body) {
            int count = source.Count(), at = 0;

            while (at < count) {
                int thisBuffer = Math.Min(count - at, buffer); 
                
                Parallel.For(at, at + thisBuffer, i => {
                    body(source.ElementAt(i));
                });

                at += thisBuffer;
            }
        }
    }
}
