package org.daisy.urakawa;

import org.daisy.urakawa.core.CorePresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.xml.XmlPresentation;

/**
 * @zdepend 1 Aggregation 1 Project
 */
public interface Presentation extends CorePresentation, ChannelPresentation,
		XmlPresentation, MediaDataPresentation, ValueEquatable<Presentation> {
	
	/**
	 * 
	 * @return cannot be null.
	 */
	public Project getProject();

	/**
	 * 
	 * @param project
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 */
	public void setProject(Project project) throws MethodParameterIsNullException;
}