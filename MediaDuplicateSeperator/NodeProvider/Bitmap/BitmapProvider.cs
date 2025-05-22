using System;

namespace MediaDuplicateSeperator {
    public class BitmapProvider : INodeProvider<LockedBitmap> {
        public LockedBitmap ProvideNode(string nodePath) {
            return new LockedBitmap(nodePath);
        }
    }
}
