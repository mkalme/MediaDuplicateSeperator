using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using MediaDuplicateSeperator;

namespace DemoConsole {
    public class MediaSorter {
        public string SourceFolder { get; set; }
        public string OutputFolder { get; set; }
        public string BaseFolder { get; set; }

        public HashSet<Size> AllowedSizes { get; set; } = new HashSet<Size>() {
            new Size(2592, 1944),
            new Size(1944, 2592)
        };

        private CachedNodeProvider<LockedBitmap> _bitmapCache;
        private CachedNodeProvider<Video> _videoCache;
        private IMediaComparer _mediaComparer;

        public MediaSorter(string sourceFolder, string outputFolder, string baseFolder) {
            SourceFolder = sourceFolder;
            OutputFolder = outputFolder;
            BaseFolder = baseFolder;
        }

        public void Sort() {
            CreateMediaComparer();
            LoadCache();
            ValidateCache();

            IEnumerable<IEnumerable<string>> duplicates = GetDuplicates();
            IEnumerable<FileDate> fileDates = GetUniqueFiles(FileDate.CreateFileDates(CreateMediaDateFinder(), duplicates));
            IEnumerable<FileDate> uniqueFiles = RemoveDuplicates(fileDates);

            CopyFiles(uniqueFiles);
        }

        private void CreateMediaComparer() {
            ShallowMediaComparerFactory shallowFactory = new ShallowMediaComparerFactory();
            MediaComparerFactory deepFactory = new MediaComparerFactory();

            _bitmapCache = shallowFactory.CachedBitmapProvider;
            _videoCache = shallowFactory.CachedVideoProvider;

            shallowFactory.BitmapCache = _bitmapCache;
            shallowFactory.VideoCache = _videoCache;

            deepFactory.BitmapCache = _bitmapCache;
            deepFactory.VideoCache = _videoCache;

            _mediaComparer = new LayeredMediaComparer() {
                MediaComparers = new List<IMediaComparer>() {
                    new MediaComparer() { MediaComparerFactory = shallowFactory },
                    new MediaComparer() { MediaComparerFactory = deepFactory }
                }
            };
        }
        private void LoadCache() {
            string[] files = Directory.GetFiles(SourceFolder, "*", SearchOption.AllDirectories);

            _bitmapCache.PathValidator = path => {
                string extension = Path.GetExtension(path);
                return MediaUtilities.ImageFormats.Contains(extension) || string.IsNullOrEmpty(extension);
            };

            _bitmapCache.Preload(files);

            HashSet<string> videoFiles = new HashSet<string>(files);

            videoFiles.AddRange(_bitmapCache.UnloadeableFiles.Where(x => string.IsNullOrEmpty(Path.GetExtension(x))));
            videoFiles.RemoveWhere(x => _bitmapCache.Cache.ContainsKey(x));

            _videoCache.PathValidator = path => {
                string extension = Path.GetExtension(path);
                return MediaUtilities.VideoFormats.Contains(extension) || string.IsNullOrEmpty(extension);
            };

            _videoCache.Preload(videoFiles);
        }
        private void ValidateCache() {
            int index = 0;
            while (index < _bitmapCache.Cache.Count) {
                var pair = _bitmapCache.Cache.ElementAt(index);

                if (!AllowedSizes.Contains(new Size(pair.Value.Width, pair.Value.Height)) ||
                    pair.Key.Contains("google")) {

                    _bitmapCache.Cache.TryRemove(pair.Key, out LockedBitmap bitmap);
                    bitmap.Dispose();
                } else {
                    index++;
                }
            }
        }

        private IEnumerable<IEnumerable<string>> GetDuplicates() {
            IAllMediaDuplicateSeperator seperator = new AllMediaDuplicateSeperator() {
                DuplicateSeperator = new SingleDuplicateSeperator() {
                    MediaComparer = _mediaComparer
                }
            };

            List<string> files = new List<string>(_bitmapCache.Cache.Keys);
            files.AddRange(_videoCache.Cache.Keys);

            return seperator.SeperateDuplicates(files);
        }
        private IEnumerable<FileDate> GetUniqueFiles(IEnumerable<FileDate> fileDates) {
            List<FileDate> files = new List<FileDate>();

            foreach (FileDate date in fileDates) {
                if (!string.IsNullOrEmpty(date.OldestFile)) {
                    files.Add(new FileDate(date.OldestFile, date.OldestDate));
                } else {
                    files.Add(new FileDate(date.Files.First(), DateTime.MaxValue));
                }

                foreach (var file in date.Files) {
                    if (file == files.Last().OldestFile) continue;

                    RemoveCache(file);
                }
            }

            return files;
        }
        private IEnumerable<FileDate> RemoveDuplicates(IEnumerable<FileDate> files) {
            var folders = OranizeFiles(files);

            List<FileDate> output = new List<FileDate>();

            ICommonMediaDuplicateSeperator seperator = new CommonMediaDuplicateSeperator() {
                DuplicateSeperator = new SingleDuplicateSeperator() {
                    MediaComparer = _mediaComparer
                },
                StopAtFirstEncounter = true
            };

            foreach (var pair in folders) {
                if (!Directory.Exists(pair.Key)) continue;

                string[] baseFiles = Directory.GetFiles(pair.Key, "*");
                IEnumerable<string> otherFiles = pair.Value.Select(x => x.OldestFile);

                _bitmapCache.Preload(baseFiles);
                _videoCache.Preload(baseFiles);

                Console.WriteLine(pair.Key);

                IEnumerable<CommonMediaDuplicate> duplicates = seperator.SeperateDuplicates(baseFiles, new IEnumerable<string>[] { otherFiles });

                foreach (var file in baseFiles) {
                    RemoveCache(file);
                }

                Console.WriteLine(pair.Key + " ==========================================");
                foreach (var duplicate in duplicates) {
                    foreach (var file in duplicate.Duplicates) {
                        FileDate date = pair.Value.Where(x => x.OldestFile == file).First();

                        pair.Value.Remove(date);
                        RemoveCache(date.OldestFile);
                    }
                }

                output.AddRange(pair.Value);
            }

            return output;
        }

        private void CopyFiles(IEnumerable<FileDate> files) {
            foreach (var file in files) {
                string directory = $"{OutputFolder}\\{file.OldestDate.ToString("yyyy-MM")}";
                string output = $"{directory}\\{Path.GetFileName(file.OldestFile)}";

                if (string.IsNullOrEmpty(Path.GetExtension(file.OldestFile))) {
                    if (_bitmapCache.Cache.ContainsKey(file.OldestFile)) {
                        output = $"{output}.jpg";
                    } else if (_videoCache.Cache.ContainsKey(file.OldestFile)) {
                        output = $"{output}.mp4";
                    }
                }

                Directory.CreateDirectory(directory);
                File.Copy(file.OldestFile, GetFileName(output));
            }
        }

        private IMediaDateFinder CreateMediaDateFinder() {
            return new MediaDateFinder() {
                MediaDateFinderFactory = new CustomMediaDateFinderFactory() {
                    BitmapCache = _bitmapCache,
                    VideoCache = _videoCache
                }
            };
        }
        private IDictionary<string, IList<FileDate>> OranizeFiles(IEnumerable<FileDate> files) {
            var folders = new Dictionary<string, IList<FileDate>>();
            foreach (var file in files) {
                string folderName = "unsorted";

                if (file.OldestDate != DateTime.MaxValue) {
                    folderName = file.OldestDate.ToString("yyyy-MM");
                }
                folderName = $"{BaseFolder}\\{folderName}";

                if (!folders.TryGetValue(folderName, out IList<FileDate> list)) {
                    list = new List<FileDate>();
                    folders.Add(folderName, list);
                }

                list.Add(file);
            }

            return folders;
        }
        private void RemoveCache(string file) {
            if (_bitmapCache.Cache.TryRemove(file, out LockedBitmap bitmap)) {
                bitmap.Dispose();
            } else if (_videoCache.Cache.TryRemove(file, out Video video)) {
                video.Dispose();
            }
        }

        private static string GetFileName(string file) {
            string output = file;

            int count = 2;
            while (File.Exists(output)) {
                output = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)} ({count++}){Path.GetExtension(file)}";
            }

            return output;
        }
    }

    public class FileDate { 
        public string OldestFile { get; set; }
        public DateTime OldestDate { get; set; }
        public IEnumerable<string> Files { get; set; }

        public FileDate(string oldestFile, DateTime oldestDate) {
            OldestFile = oldestFile;
            OldestDate = oldestDate;
        }

        public static IEnumerable<FileDate> CreateFileDates(IMediaDateFinder dateFinder, IEnumerable<IEnumerable<string>> duplicates) {
            List<FileDate> output = new List<FileDate>();

            foreach (var duplicate in duplicates) {
                string path = "";
                DateTime date = DateTime.MaxValue;

                foreach (var file in duplicate) {
                    if (dateFinder.TryFindDate(file, out DateTime time) && time < date) {
                        path = file;
                        date = time;
                    }
                }

                output.Add(new FileDate(path, date) {
                    Files = duplicate
                });
            }

            return output;
        }
    }

    public class CustomMediaDateFinderFactory : IMediaDateFinderFactory {
        public CachedNodeProvider<LockedBitmap> BitmapCache { get; set; }
        public CachedNodeProvider<Video> VideoCache { get; set; }

        public IMediaDateFinder CreateMediaDateFinder(string path) {
            if (BitmapCache.Cache.ContainsKey(path)) return new ImageDateFinder();
            else if (VideoCache.Cache.ContainsKey(path)) return new VideoDateFinder();

            return null;
        }
    }
}
