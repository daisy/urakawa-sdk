package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create 1 FileDataProvider
 * @todo verify / add comments and exceptions
 * @leafInterface see {@link org.daisy.urakawa.LeafInterface}
 * @see org.daisy.urakawa.LeafInterface
 * @stereotype OptionalLeafInterface
 */
public interface FileDataProviderFactory extends DataProviderFactory,
		WithFileDataProviderManager {
	FileDataProvider createFileDataProvider(String mimeType)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	FileDataProvider createFileDataProvider(String mimeType,
			String xukLocalName, String xukNamespaceURI)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	public String getExtensionFromMimeType(String mimeType)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
