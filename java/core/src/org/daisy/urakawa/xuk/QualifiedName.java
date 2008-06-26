package org.daisy.urakawa.xuk;

import org.daisy.urakawa.exception.MethodParameterIsEmptyStringException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;

/**
 *
 */
public class QualifiedName {
	private String mNS;
	private String mName;

	/**
	 * @param ns
	 * @param name
	 * @throws MethodParameterIsNullException
	 * @throws MethodParameterIsEmptyStringException
	 */
	public QualifiedName(String ns, String name)
			throws MethodParameterIsNullException,
			MethodParameterIsEmptyStringException {
		if (name == null || ns == null) {
			throw new MethodParameterIsNullException();
		}
		if (name.length() == 0) {
			throw new MethodParameterIsEmptyStringException();
		}
		mNS = ns;
		mName = name;
	}

	/**
	 * @return a non-null but potentially empty string
	 */
	public String getNamespace() {
		return mNS;
	}

	/**
	 * @return a non-null and non-empty string
	 */
	public String getName() {
		return mName;
	}
}
