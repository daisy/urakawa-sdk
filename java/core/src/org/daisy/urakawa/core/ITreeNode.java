package org.daisy.urakawa.core;

import org.daisy.urakawa.IValueEquatable;
import org.daisy.urakawa.IWithPresentation;
import org.daisy.urakawa.core.visitor.IVisitableTreeNode;
import org.daisy.urakawa.event.IEventHandler;
import org.daisy.urakawa.event.DataModelChangedEvent;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.IWithProperties;
import org.daisy.urakawa.xuk.IXukAble;

/**
 * <p>
 * This is the base type for nodes of the document tree. The visitor pattern for
 * browsing the tree is included by design, via the
 * {@link org.daisy.urakawa.core.visitor.IVisitableTreeNode} interface.
 * </p>
 * <p>
 * For clarity, the methods have been separated into 2 categories: "read-only"
 * and "write-only". See {@link org.daisy.urakawa.core.ITreeNodeReadOnlyMethods}
 * and {@link org.daisy.urakawa.core.ITreeNodeWriteOnlyMethods}.
 * </p>
 * <p>
 * Note: this interface assembles a set of other interfaces, but does not
 * introduce new methods itself.
 * </p>
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 * @depend - Aggregation 1 org.daisy.urakawa.IPresentation
 * @depend - Composition 0..n org.daisy.urakawa.property.IProperty
 * 
 */
public interface ITreeNode extends IWithProperties, IWithPresentation,
		ITreeNodeReadOnlyMethods, ITreeNodeWriteOnlyMethods, IVisitableTreeNode,
		IXukAble, IValueEquatable<ITreeNode>, IEventHandler<DataModelChangedEvent> {
	/**
	 * @param destinationNode
	 * @throws MethodParameterIsNullException
	 */
	public void copyChildren(ITreeNode destinationNode)
			throws MethodParameterIsNullException;
}