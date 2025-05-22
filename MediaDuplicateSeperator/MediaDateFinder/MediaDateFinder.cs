using System;

namespace MediaDuplicateSeperator {
    public class MediaDateFinder : IMediaDateFinder {
        public IMediaDateFinderFactory MediaDateFinderFactory { get; set; }

        public MediaDateFinder() {
            MediaDateFinderFactory = new MediaDateFinderFactory();
        }

        public bool TryFindDate(string path, out DateTime dateTime) {
            dateTime = DateTime.MinValue;

            IMediaDateFinder dateFinder = MediaDateFinderFactory.CreateMediaDateFinder(path);
            if (dateFinder != null) {
                try {
                    return dateFinder.TryFindDate(path, out dateTime);
                } catch {}
            }

            return false;
        }
    }
}
