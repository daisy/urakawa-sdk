using urakawa.xuk;

namespace urakawa.core
{
    [XukNameUglyPrettyAttribute("nodFct", "TreeNodeFactory")]
    public sealed class TreeNodeFactory : GenericWithPresentationFactory<TreeNode>
    {
        public TreeNodeFactory(Presentation pres) : base(pres)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TreeNode"/>
        /// </summary>
        /// <returns>The new <see cref="TreeNode"/></returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when the <see cref="Presentation"/> of the 
        /// </exception>
        public TreeNode Create()
        {
            return Create<TreeNode>();
        }
    }
}