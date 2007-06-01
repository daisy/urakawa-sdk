package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.TreeNode;

/**
 * This concrete class provides the implementation required for isIncluded(),
 * based on filtering by class type.
 */
public class TypeFilterNavigator extends FilterNavigatorAbstractImpl {
	Class<TreeNode> m_klass = null;

	public TypeFilterNavigator(Class<TreeNode> klass) {
		m_klass = klass;
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

	public boolean isIncluded(TreeNode node) {
		return m_klass.isInstance(node);
	}
}
