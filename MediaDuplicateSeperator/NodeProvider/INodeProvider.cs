using System;

namespace MediaDuplicateSeperator {
    public interface INodeProvider<TNode> {
        TNode ProvideNode(string nodePath);
    }
}
