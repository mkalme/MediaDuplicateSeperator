using System;

namespace MediaDuplicateSeperator {
    public class ShallowVideoProvider : INodeProvider<Video> {
        public Video ProvideNode(string file) {
            Video video = new Video();
            video.Open(file);

            return video;
        }
    }
}
