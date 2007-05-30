package org.daisy.urakawa;

import java.net.URI;
import java.util.List;

import org.daisy.urakawa.core.CorePresentationImpl;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.media.MediaFactory;
import org.daisy.urakawa.media.data.DataProviderManager;
import org.daisy.urakawa.media.data.MediaDataFactory;
import org.daisy.urakawa.media.data.MediaDataManager;
import org.daisy.urakawa.properties.channel.ChannelFactory;
import org.daisy.urakawa.properties.channel.ChannelsManager;
import org.daisy.urakawa.properties.channel.ChannelsPropertyFactory;
import org.daisy.urakawa.properties.xml.XmlPropertyFactory;

/**
 * Reference implementation of the interface.
 */
public class PresentationImpl extends CorePresentationImpl implements
		Presentation {
	/**
	 * @hidden
	 */
	public Project getProject() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setProject(Project project)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public ChannelFactory getChannelFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setChannelFactory(ChannelFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public ChannelsPropertyFactory getChannelsPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setChannelsPropertyFactory(ChannelsPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public ChannelsManager getChannelsManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setChannelsManager(ChannelsManager manager)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public XmlPropertyFactory getXmlPropertyFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setXmlPropertyFactory(XmlPropertyFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public URI getBaseUri() {
		return null;
	}

	/**
	 * @hidden
	 */
	public List<Media> getListOfUsedMedia() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setBaseUri(URI newBase) {
	}

	/**
	 * @hidden
	 */
	public MediaFactory getMediaFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaFactory(MediaFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public MediaDataFactory getMediaDataFactory() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataFactory(MediaDataFactory factory)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public MediaDataManager getMediaDataManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setMediaDataManager(MediaDataManager manager)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public DataProviderManager getDataProviderManager() {
		return null;
	}

	/**
	 * @hidden
	 */
	public void setDataProviderManager(DataProviderManager manager)
			throws MethodParameterIsNullException {
	}

	/**
	 * @hidden
	 */
	public boolean ValueEquals(Presentation other)
			throws MethodParameterIsNullException {
		return false;
	}
}
