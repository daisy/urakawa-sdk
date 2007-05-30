package org.daisy.urakawa;

import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.metadata.MetadataFactory;


/**
 * @depend - Aggregation 1 MetadataFactory
 */
public interface WithMetadataFactory {
	/**
	 * @return the factory object
	 */
	public MetadataFactory getMetadataFactory();

	/**
	 * @param factory
	 *            cannot be null
	 * @throws MethodParameterIsNullException
	 *             if factory is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setMetadataFactory(MetadataFactory factory)
			throws MethodParameterIsNullException;
}
