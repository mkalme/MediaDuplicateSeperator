using System;

namespace MediaDuplicateSeperator {
    public class VideoComparer : IMediaComparer {
        public INodeProvider<Video> VideoProvider { get; set; }
        public bool Shallow { get; set; } = true;

        public VideoComparer() {
            VideoProvider = new DeepVideoProvider(new ShallowVideoProvider());
        }

        public bool TryCompare(string firstPath, string secondPath, out bool equals) {
            try {
                Video firstVideo = VideoProvider.ProvideNode(firstPath);
                Video secondVideo = VideoProvider.ProvideNode(secondPath);

                equals = firstVideo.Equals(secondVideo, Shallow);
                return true;
            } catch {
                equals = false;
                return false;
            }
        }
    }
}
