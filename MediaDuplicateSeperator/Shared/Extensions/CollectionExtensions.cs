using System;
using System.Collections.Generic;

namespace MediaDuplicateSeperator {
    public static class CollectionExtensions {
        public static bool TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey[] keys, out TValue value) {
            for (int i = 0; i < keys.Length; i++) {
                if (dictionary.TryGetValue(keys[i], out value)) {
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }
        public static void AddRange<TValue>(this ISet<TValue> set, IEnumerable<TValue> range) {
            foreach (TValue value in range) {
                set.Add(value);
            }
        }

        public static IDictionary<string, string> ExtractMetadata(string metadata) {
            string[] lines = metadata.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Dictionary<string, string> tags = new Dictionary<string, string>(lines.Length);

            foreach (string line in lines) {
                var pair = line.Split(new char[] { ':' }, 2);

                if (pair.Length >= 2) {
                    var key = pair[0].TrimEnd();
                    var value = pair[1].TrimStart();

                    tags[key] = value;
                }
            }

            return tags;
        }
        public static bool TryParseTag(string[] tagValues, IDictionary<string, string> exifData, out int value) {
            if (exifData.TryGetValue(tagValues, out string tag) && int.TryParse(tag, out value)) {
                return true;
            }

            value = 0;
            return false;
        }
    }
}
