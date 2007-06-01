package org.daisy.urakawa.core;

import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Aggregation 1 TreeNodeFactory
 */
public interface WithTreeNodeFactory {
	/**
	 * @return the factory object
	 */
	public TreeNodeFactory getTreeNodeFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype Initialize
	 */
	public void setTreeNodeFactory(TreeNodeFactory factory)
			throws MethodParameterIsNullException;
}
