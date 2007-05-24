package org.daisy.urakawa;

import org.daisy.urakawa.exceptions.MethodParameterIsNullException;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;

/**
 * @depend - Aggregation 1 Presentation
 */
public interface PropertyFactory extends XmlPropertyFactory,
		ChannelsPropertyFactory {

	/**
	 * 
	 */
	public Presentation getPresentation();

	/**
	 * 
	 * @param pres
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setPresentation(Presentation pres)
			throws MethodParameterIsNullException;
}
