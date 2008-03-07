package org.daisy.urakawa.property.channel;

import java.net.URI;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsAlreadyInitializedException;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.media.Media;
import org.daisy.urakawa.nativeapi.XmlDataReader;
import org.daisy.urakawa.nativeapi.XmlDataWriter;
import org.daisy.urakawa.xuk.XukDeserializationFailedException;
import org.daisy.urakawa.xuk.XukSerializationFailedException;

/**
 * Reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 */
public class ChannelImpl extends WithPresentationImpl implements Channel {
	private String mName = "";
	private String mLanguage = null;
	private ChannelsManager mChannelsManager = null;

	/**
	 * @param chMgr
	 * @throws MethodParameterIsNullException
	 */
	public ChannelImpl(ChannelsManager chMgr)
			throws MethodParameterIsNullException {
		try {
			setChannelsManager(chMgr);
		} catch (IsAlreadyInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public ChannelsManager getChannelsManager()
			throws IsNotInitializedException {
		if (mChannelsManager == null) {
			throw new IsNotInitializedException();
		}
		return mChannelsManager;
	}

	public void setChannelsManager(ChannelsManager man)
			throws MethodParameterIsNullException,
			IsAlreadyInitializedException {
		if (man == null) {
			throw new MethodParameterIsNullException();
		}
		if (mChannelsManager != null) {
			throw new IsAlreadyInitializedException();
		}
	}

	public boolean isEquivalentTo(Channel otherChannel)
			throws MethodParameterIsNullException {
		if (otherChannel == null) {
			throw new MethodParameterIsNullException();
		}
		if (this.getClass() != otherChannel.getClass())
			return false;
		try {
			if (this.getName() != otherChannel.getName())
				return false;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (this.getLanguage() != otherChannel.getLanguage())
			return false;
		return true;
	}

	public Channel export(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		return exportProtected(destPres);
	}

	protected Channel exportProtected(Presentation destPres)
			throws FactoryCannotCreateTypeException, IsNotInitializedException,
			MethodParameterIsNullException {
		Channel exportedCh;
		try {
			exportedCh = destPres.getChannelFactory().createChannel(
					getXukLocalName(), getXukNamespaceURI());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (exportedCh == null) {
			throw new FactoryCannotCreateTypeException();
		}
		try {
			exportedCh.setName(getName());
			exportedCh.setLanguage(getLanguage());
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		return exportedCh;
	}

	@SuppressWarnings("unused")
	public boolean canAccept(@SuppressWarnings("unused")
	Media media) throws MethodParameterIsNullException {
		return true;
	}

	public void setName(String name) throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (name == null) {
			throw new MethodParameterIsNullException();
		}
		if (name == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		mName = name;
	}

	public String getName() throws IsNotInitializedException {
		if (mName == null) {
			throw new IsNotInitializedException();
		}
		return mName;
	}

	public void setLanguage(String lang)
			throws MethodParameterIsEmptyStringException {
		if (lang == "") {
			throw new MethodParameterIsEmptyStringException();
		}
		mLanguage = lang;
	}

	public String getLanguage() {
		return mLanguage;
	}

	public String getUid() {
		try {
			return getChannelsManager().getUidOfChannel(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (ChannelDoesNotExistException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukInAttributes(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		String name = source.getAttribute("name");
		if (name == null)
			name = "UNDEFINED_NAME";
		try {
			setName(name);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		String lang = source.getAttribute("language");
		if (lang != null)
			lang = lang.trim();
		if (lang == "")
			lang = null;
		try {
			setLanguage(lang);
		} catch (MethodParameterIsEmptyStringException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	@Override
	@SuppressWarnings("unused")
	protected void xukInChild(XmlDataReader source)
			throws MethodParameterIsNullException,
			XukDeserializationFailedException {
		if (source == null) {
			throw new MethodParameterIsNullException();
		}
		boolean readItem = false;
		if (!(readItem || source.isEmptyElement())) {
			source.readSubtree().close();
		}
	}

	@SuppressWarnings("unused")
	@Override
	protected void xukOutAttributes(XmlDataWriter destination, URI baseUri)
			throws MethodParameterIsNullException,
			XukSerializationFailedException {
		try {
			destination.writeAttributeString("name", getName());
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		destination.writeAttributeString("language", getLanguage());
	}

	public boolean ValueEquals(Channel other)
			throws MethodParameterIsNullException {
		if (other == null)
			throw new MethodParameterIsNullException();
		if (getClass() != other.getClass())
			return false;
		try {
			if (getName() != other.getName())
				return false;
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
		if (getLanguage() != other.getLanguage())
			return false;
		return true;
	}
}
