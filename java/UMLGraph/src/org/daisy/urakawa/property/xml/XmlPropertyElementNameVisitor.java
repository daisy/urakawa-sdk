package org.daisy.urakawa.property.xml;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.TreeNode;
import org.daisy.urakawa.core.visitor.TreeNodeVisitor;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsWrongTypeException;

/**
 *
 */
public class XmlPropertyElementNameVisitor implements TreeNodeVisitor {
	private List<String> mNamesToMatch;
	private List<TreeNode> mNodes;
	@SuppressWarnings("unused")
	private Class<XmlProperty> mXmlPropertyType = XmlProperty.class;

	/**
	 * 
	 */
	public XmlPropertyElementNameVisitor() {
		mNamesToMatch = new LinkedList<String>();
		mNodes = new LinkedList<TreeNode>();
		mXmlPropertyType = XmlProperty.class;
	}

	/**
	 * @param newType
	 * @throws MethodParameterIsWrongTypeException
	 */
	public void setXmlPropertyType(Class<XmlProperty> newType)
			throws MethodParameterIsWrongTypeException {
		if (!XmlProperty.class.isAssignableFrom(newType)) {
			throw new MethodParameterIsWrongTypeException();
		}
		mXmlPropertyType = newType;
	}

	/**
	 * @param localName
	 * @param namespaceUri
	 */
	public void addElementName(String localName, String namespaceUri) {
		mNamesToMatch.add(String.format("{0}:{1}", namespaceUri, localName));
	}

	/**
	 * @return list
	 */
	public List<TreeNode> getResults() {
		return mNodes;
	}

	private boolean isMatch(String localName, String namespaceUri) {
		return mNamesToMatch.contains(String.format("{0}:{1}", namespaceUri,
				localName));
	}

	@SuppressWarnings("unused")
	public void postVisit(@SuppressWarnings("unused")
	TreeNode node) throws MethodParameterIsNullException {
	}

	public void preVisit(TreeNode node) throws MethodParameterIsNullException {
		XmlProperty xp = node.<XmlProperty> getProperty(XmlProperty.class);
		try {
			if (xp != null
					&& isMatch(xp.getLocalName(), xp.getNamespace()) == true) {
				mNodes.add(node);
			}
		} catch (IsNotInitializedException e) {
			// Should never happen
			throw new RuntimeException("WTF ??!", e);
		}
	}
}
