using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaDuplicateSeperator {
    public class CachedNodeProvider<TNode> : INodeProvider<TNode>, IDisposable {
        public INodeProvider<TNode> NodeProvider { get; set; }
        public ConcurrentDictionary<string, TNode> Cache { get; set; }

        public Func<string, bool> PathValidator { get; set; }
        public Action<TNode> DisposeFunction { get; set; }

        public ConcurrentBag<string> UnloadeableFiles { get; set; }

        public CachedNodeProvider(INodeProvider<TNode> nodeProvider) {
            NodeProvider = nodeProvider;
            Cache = new ConcurrentDictionary<string, TNode>();
            UnloadeableFiles = new ConcurrentBag<string>();
        }

        public TNode ProvideNode(string nodePath) {
            if (!Cache.TryGetValue(nodePath, out TNode output)) {
                output = NodeProvider.ProvideNode(nodePath);
                Cache.TryAdd(nodePath, output);
            }

            return output;
        }
        public void Preload(IEnumerable<string> files) {
            int count = 0, fileCount = files.Count();

            Parallel.ForEach(files, file => {
                if (!PathValidator(file)) {
                    Interlocked.Increment(ref count);
                    return;
                }
                if (!Cache.ContainsKey(file)) {

                    try {
                        Cache.TryAdd(file, NodeProvider.ProvideNode(file));
                    } catch {
                        UnloadeableFiles.Add(file);
                    }
                }

                Interlocked.Increment(ref count);
                Console.WriteLine($"{count}/{fileCount}");
            });
        }

        public void Dispose() {
            foreach (var pair in Cache) {
                DisposeFunction(pair.Value);
            }
            Cache.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
