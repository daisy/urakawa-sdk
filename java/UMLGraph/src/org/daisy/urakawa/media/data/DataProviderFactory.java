package org.daisy.urakawa.media.data;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 * @depend - Create - org.daisy.urakawa.media.data.DataProvider
 * @todo verify / add comments and exceptions
 */
public interface DataProviderFactory extends WithDataProviderManager {
	/**
	 * 
	 * @param mimeType
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden
	 */
	DataProvider createDataProvider(String mimeType)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;

	/**
	 * 
	 * @param mimeType
	 * @param xukLocalName
	 * @param xukNamespaceURI
	 * @return
	 * @tagvalue Exceptions "MethodParameterIsNull-MethodParameterIsEmptyString"
	 * @throws MethodParameterIsNullException
	 *             NULL method parameters are forbidden
	 * @throws MethodParameterIsEmptyStringException
	 *             Empty string '' method parameters are forbidden:
	 *             <b>xukLocalName, mimeType</b>
	 */
	DataProvider createDataProvider(String mimeType, String xukLocalName,
			String xukNamespaceURI)throws MethodParameterIsNullException, MethodParameterIsEmptyStringException;
}
