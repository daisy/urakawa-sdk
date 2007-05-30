package org.daisy.urakawa.navigator;

import org.daisy.urakawa.core.CoreNode;

/**
 * This concrete class provides the implementation required for isIncluded(),
 * based on filtering by class type.
 */
public class TypeFilterNavigator extends FilterNavigatorAbstractImpl {
	Class<CoreNode> m_klass = null;

	public TypeFilterNavigator(Class<CoreNode> klass) {
		m_klass = klass;
	}

	/**
	 * @hidden
	 */
	public void test(CoreNode node) {
		TypeFilterNavigator nav = new TypeFilterNavigator(CoreNode.class);
		CoreNode parentNode = null;
		try {
			parentNode = (CoreNode) nav.getParent(node);
		} catch (CoreNodeNotIncludedByNavigatorException e) {
			e.printStackTrace();
			return;
		}
		parentNode.getChildCount();
	}

	public boolean isIncluded(CoreNode node) {
		return m_klass.isInstance(node);
	}
}
