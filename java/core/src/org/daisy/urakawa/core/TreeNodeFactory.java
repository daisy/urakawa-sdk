package org.daisy.urakawa.core;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public final class TreeNodeFactory extends GenericFactory<TreeNode> {
	public ITreeNode create() {

		try {
			return create(TreeNode.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
