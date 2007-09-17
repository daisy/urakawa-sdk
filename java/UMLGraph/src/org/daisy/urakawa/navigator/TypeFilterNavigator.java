package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * This concrete class provides the implementation required for isIncluded(),
 * based on filtering by class type.
 */
public class TypeFilterNavigator extends FilterNavigatorAbstractImpl {
	/**
	 * The type to match by the filter function (initialized by constructor)
	 */
	Class<TreeNode> m_klass = null;

	/**
	 * Constructor
	 * 
	 * @param klass
	 *            The type to match by the filter function
	 */
	public TypeFilterNavigator(Class<TreeNode> klass) {
		m_klass = klass;
	}

	/**
	 * The filter function, which we must implement here, as required by our
	 * "super" abstract class.
	 * 
	 * @return true if the passed TreeNode is of the same type as given in the
	 *         constructor
	 */
	public boolean isIncluded(TreeNode node) {
		return m_klass.isInstance(node);
	}

	/**
	 * @hidden
	 */
	public void test(TreeNode node) {
		TypeFilterNavigator nav = new TypeFilterNavigator(TreeNode.class);
		TreeNode parentNode = null;
		try {
			parentNode = (TreeNode) nav.getParent(node);
		} catch (TreeNodeNotIncludedByNavigatorException e) {
			e.printStackTrace();
			return;
		}
		parentNode.getChildCount();
	}

	/**
	 * @hidden
	 */
	public int getIndex() {
		return 0;
	}

	/**
	 * @hidden
	 */
	public int getIndexOf(TreeNode node) throws MethodParameterIsNullException,
			TreeNodeNotIncludedByNavigatorException {
		return 0;
	}
}
