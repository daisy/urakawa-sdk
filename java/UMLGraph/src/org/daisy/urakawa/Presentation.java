package org.daisy.urakawa;

import org.daisy.urakawa.core.TreeNodePresentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.data.MediaDataPresentation;
import org.daisy.urakawa.properties.channel.ChannelPresentation;
import org.daisy.urakawa.properties.xml.XmlPresentation;

/**
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public interface Presentation extends TreeNodePresentation, ChannelPresentation,
		XmlPresentation, MediaDataPresentation, ValueEquatable<Presentation> {
	/**
	 * The project
	 * 
	 * @return cannot be null.
	 */
	public Project getProject();

	/**
	 * @param project
	 *            cannot be null.
	 * @throws MethodParameterIsNullException
	 *             if project is null
	 * @tagvalue Exceptions "MethodParameterIsNull"
	 * @stereotype initialize
	 */
	public void setProject(Project project)
			throws MethodParameterIsNullException;
}