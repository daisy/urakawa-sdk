package org.daisy.urakawa.media.data;

import java.util.List;

import org.daisy.urakawa.ValueEquatable;
import org.daisy.urakawa.WithPresentation;
import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.xuk.XukAble;

/**
 * @todo verify / add comments and exceptions
 * @depend - Composition 0..n MediaData
 */
public interface MediaDataManager extends WithDataProviderFactory,
		WithMediaDataFactory, WithPresentation, XukAble,
		ValueEquatable<MediaDataManager> {
	MediaData getMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	String getUidOfMediaData(MediaData data)throws MethodParameterIsNullException;

	void addMediaData(MediaData data);

	List<String> getListOfUids();

	void detachMediaData(MediaData data)throws MethodParameterIsNullException;

	void deleteMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	MediaData copyMediaData(MediaData data)throws MethodParameterIsNullException;

	MediaData copyMediaData(String uid)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	List<MediaData> getListOfManagedMediaData();
}
