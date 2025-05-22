using System;

namespace MediaDuplicateSeperator {
    public class DeepVideoProvider : INodeProvider<Video> {
        public INodeProvider<Video> VideoProvider { get; set; }

        public DeepVideoProvider(INodeProvider<Video> videoProvider) {
            VideoProvider = videoProvider;
        }

        public Video ProvideNode(string file) {
            Video video = VideoProvider.ProvideNode(file);
            video.LoadFrames((int)Math.Ceiling(video.FrameRate));

            return video;
        }
    }
}
