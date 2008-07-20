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
	public QualifiedName(String name, String ns)
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
	public String getNamespaceUri() {
		return mNS;
	}

	/**
	 * @return a non-null and non-empty string
	 */
	public String getLocalName() {
		return mName;
	}

	@Override
	public String toString() {
		if (mNS.length() == 0)
			return mName;
		return mNS + ":" + mName;
	}

	/**
	 * @param qn
	 * @return
	 * @throws MethodParameterIsNullException
	 */
	public static QualifiedName parseQName(String qn)
			throws MethodParameterIsNullException {
		if (qn == null) {
			throw new MethodParameterIsNullException();
		}
		String[] parts = qn.split(":");
		String ln;
		String ns = "";
		if (parts.length > 1) {
			ln = parts[parts.length - 1];
			ns = parts[0];
			for (int i = 1; i < parts.length - 1; i++) {
				ns += ":" + parts[i];
			}
		} else {
			ln = qn;
		}
		try {
			return new QualifiedName(ln, ns);
		} catch (MethodParameterIsEmptyStringException e) {

			// Should never happen
			throw new RuntimeException("WTF ?!", e);
		}
	}
}
