package org.daisy.urakawa.core.visitor.examples;

import java.util.LinkedList;
import java.util.List;

import org.daisy.urakawa.core.ITreeNode;
import org.daisy.urakawa.core.visitor.ITreeNodeVisitor;
import org.daisy.urakawa.exception.IsNotInitializedException;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.exception.MethodParameterIsWrongTypeException;
import org.daisy.urakawa.property.xml.IXmlProperty;

/**
 *
 */
public class XmlPropertyElementNameVisitor implements ITreeNodeVisitor
{
    private List<String> mNamesToMatch;
    private List<ITreeNode> mNodes;
    @SuppressWarnings("unused")
    private Class<IXmlProperty> mXmlPropertyType = IXmlProperty.class;

    /**
	 * 
	 */
    public XmlPropertyElementNameVisitor()
    {
        mNamesToMatch = new LinkedList<String>();
        mNodes = new LinkedList<ITreeNode>();
        mXmlPropertyType = IXmlProperty.class;
    }

    /**
     * @param newType
     * @throws MethodParameterIsWrongTypeException
     */
    public void setXmlPropertyType(Class<IXmlProperty> newType)
            throws MethodParameterIsWrongTypeException
    {
        if (!IXmlProperty.class.isAssignableFrom(newType))
        {
            throw new MethodParameterIsWrongTypeException();
        }
        mXmlPropertyType = newType;
    }

    /**
     * @param localName
     * @param namespaceUri
     */
    public void addElementName(String localName, String namespaceUri)
    {
        mNamesToMatch.add(String.format("{0}:{1}", namespaceUri, localName));
    }

    /**
     * @return list
     */
    public List<ITreeNode> getResults()
    {
        return mNodes;
    }

    private boolean isMatch(String localName, String namespaceUri)
    {
        return mNamesToMatch.contains(String.format("{0}:{1}", namespaceUri,
                localName));
    }

    public void postVisit(ITreeNode node) throws MethodParameterIsNullException
    {
        /**
         * Does nothing.
         */
    }

    public boolean preVisit(ITreeNode node)
            throws MethodParameterIsNullException
    {
        IXmlProperty xp = node.<IXmlProperty> getProperty(IXmlProperty.class);
        try
        {
            if (xp != null
                    && isMatch(xp.getLocalName(), xp.getNamespace()) == true)
            {
                mNodes.add(node);
            }
        }
        catch (IsNotInitializedException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
        return true;
    }
}
