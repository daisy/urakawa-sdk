package org.daisy.urakawa.core;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.core.visitor.VisitableTreeNode;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @depend - Composition 0..n org.daisy.urakawa.core.property.Property
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Aggregation 0..n org.daisy.urakawa.core.property.PropertyType
 */
public interface TreeNode extends WithProperties, WithPresentation,
		TreeNodeReadOnlyMethods, TreeNodeWriteOnlyMethods, VisitableTreeNode,
		XukAble, ValueEquatable<TreeNode> {
}