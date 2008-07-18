package org.daisy.urakawa.core;

import org.daisy.urakawa.GenericFactory;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 */
public final class TreeNodeFactory extends GenericFactory<TreeNode> {
	public TreeNode create() {

		try {
			return create(TreeNode.class);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
