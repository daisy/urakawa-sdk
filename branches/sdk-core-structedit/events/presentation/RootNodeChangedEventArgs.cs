using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa.events.presentation
{
    /// <summary>
    /// Arguments of the <see cref="Presentation.RootNodeChanged"/> event
    /// </summary>
    public class RootNodeChangedEventArgs : PresentationEventArgs
    {
        /// <summary>
        /// Constructor setting the source <see cref="Presentation"/> of the event 
        /// and the previous+new root node
        /// </summary>
        /// <param name="source">The source <see cref="Presentation"/> of the event </param>
        /// <param name="newRoot">The new root node</param>
        /// <param name="prevRoot">The root node prior to the change</param>
        public RootNodeChangedEventArgs(Presentation source, urakawa.core.TreeNode newRoot,
                                        urakawa.core.TreeNode prevRoot)
            : base(source)
        {
            NewRootNode = newRoot;
            PreviousRootNode = prevRoot;
        }

        /// <summary>
        /// The new root node
        /// </summary>
        public readonly urakawa.core.TreeNode NewRootNode;

        /// <summary>
        /// The root node prior to the change
        /// </summary>
        public readonly urakawa.core.TreeNode PreviousRootNode;
    }
}