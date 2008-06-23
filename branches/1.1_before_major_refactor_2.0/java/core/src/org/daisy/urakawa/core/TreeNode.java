package org.daisy.urakawa.core;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.core.visitor.VisitableTreeNode;
import org.daisy.urakawa.event.EventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.WithProperties;
import org.daisy.urakawa.xuk.XukAble;

/**
 * <p>
 * This is the base type for nodes of the document tree. The visitor pattern for
 * browsing the tree is included by design, via the
 * {@link org.daisy.urakawa.core.visitor.VisitableTreeNode} interface.
 * </p>
 * <p>
 * For clarity, the methods have been separated into 2 categories: "read-only"
 * and "write-only". See {@link org.daisy.urakawa.core.TreeNodeReadOnlyMethods}
 * and {@link org.daisy.urakawa.core.TreeNodeWriteOnlyMethods}.
 * </p>
 * <p>
 * Note: this interface assembles a set of other interfaces, but does not
 * introduce new methods itself.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.Presentation
 * @depend - Composition 0..n org.daisy.urakawa.property.Property
 * @stereotype XukAble
 */
public interface TreeNode extends WithProperties, WithPresentation,
		TreeNodeReadOnlyMethods, TreeNodeWriteOnlyMethods, VisitableTreeNode,
		XukAble, ValueEquatable<TreeNode>, EventHandler<DataModelChangedEvent> {
	/**
	 * @param destinationNode
	 * @throws MethodParameterIsNullException
	 */
	public void copyChildren(TreeNode destinationNode)
			throws MethodParameterIsNullException;
}