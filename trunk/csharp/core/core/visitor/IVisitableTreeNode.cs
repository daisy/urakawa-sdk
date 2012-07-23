using System;
using urakawa.core;
using urakawa.navigation;

namespace urakawa.core.visitor
{
    /// <summary>
    /// Provides methods for accepting <see cref="ITreeNodeVisitor"/>s
    /// </summary>
    public interface IVisitableTreeNode
    {
        void AcceptDepthFirst(ITreeNodeVisitor visitor);
        void AcceptDepthFirst(PreVisitDelegate preVisit, PostVisitDelegate postVisit);

        void AcceptBreadthFirst(ITreeNodeVisitor visitor);
        void AcceptBreadthFirst(PreVisitDelegate preVisit);



        void AcceptDepthFirst(INavigator navigator, ITreeNodeVisitor visitor);
        void AcceptDepthFirst(INavigator navigator, PreVisitDelegate preVisit, PostVisitDelegate postVisit);

        void AcceptBreadthFirst(INavigator navigator, ITreeNodeVisitor visitor);
        void AcceptBreadthFirst(INavigator navigator, PreVisitDelegate preVisit);
    }

    /// <summary>
    /// Delegate for pre-visit
    /// </summary>
    /// <param name="node">The <see cref="TreeNode"/> being visited</param>
    /// <returns>A <see cref="bool"/> indicating if the children of <paramref localName="node"/>
    /// should be visited</returns>
    public delegate bool PreVisitDelegate(TreeNode node);

    /// <summary>
    /// Delegate for post-visit
    /// </summary>
    /// <param name="node">The <see cref="TreeNode"/> being visited</param>
    public delegate void PostVisitDelegate(TreeNode node);
}