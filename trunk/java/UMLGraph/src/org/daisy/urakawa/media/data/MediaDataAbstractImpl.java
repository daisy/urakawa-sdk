package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.FactoryCannotCreateTypeException;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.WithPresentationImpl;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.IsNotManagerOfException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * Partial reference implementation of the interface.
 * 
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype Abstract
 */
public abstract class MediaDataAbstractImpl extends WithPresentationImpl
		implements MediaData {
	private String mName = "";

	public MediaDataManager getMediaDataManager()
			throws IsNotInitializedException {
		return getPresentation().getMediaDataManager();
	}

	public String getUID() {
		try {
			return getMediaDataManager().getUidOfMediaData(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotManagerOfException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	public String getName() {
		return mName;
	}

	public void setName(String newName) throws MethodParameterIsNullException {
		if (newName == null) {
			throw new MethodParameterIsNullException();
		}
		mName = newName;
	}

	public abstract List<DataProvider> getListOfUsedDataProviders();

	public void delete() {
		try {
			getMediaDataManager().removeMediaData(this);
		} catch (MethodParameterIsNullException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}

	protected abstract MediaData copyProtected();

	public MediaData copy() {
		return copyProtected();
	}

	protected abstract MediaData protectedExport(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException;

	public MediaData export(Presentation destPres)
			throws MethodParameterIsNullException,
			FactoryCannotCreateTypeException {
		return protectedExport(destPres);
	}

	public boolean ValueEquals(MediaData other)
			throws MethodParameterIsNullException {
		if (other == null) {
			throw new MethodParameterIsNullException();
		}
		if (getClass() != other.getClass())
			return false;
		if (getName() != other.getName())
			return false;
		return true;
	}
}
