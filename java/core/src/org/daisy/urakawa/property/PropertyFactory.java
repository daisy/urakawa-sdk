package org.daisy.urakawa.property;

import org.daisy.urakawa.GenericWithPresentationFactory;
import org.daisy.urakawa.Presentation;
import org.daisy.urakawa.exception.MethodParameterIsNullException;
import org.daisy.urakawa.property.channel.ChannelsProperty;
import org.daisy.urakawa.property.xml.XmlProperty;

/**
 * Extension of the generic factory to handle one or more specific types derived
 * from the base specified class, in order to provide convenience create()
 * methods.
 * 
 * @xhas - - 1 org.daisy.urakawa.Presentation
 * @depend - Create - org.daisy.urakawa.property.Property
 * @depend - Create - org.daisy.urakawa.property.xml.XmlProperty
 * @depend - Create - org.daisy.urakawa.property.channel.ChannelsProperty
 */
public final class PropertyFactory extends GenericWithPresentationFactory<Property>
{
    /**
     * @param pres
     * @throws MethodParameterIsNullException
     */
    public PropertyFactory(Presentation pres)
            throws MethodParameterIsNullException
    {
        super(pres);
    }
    /**
     * @return
     */
    public Property createProperty()
    {
        try
        {
            return create(Property.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @return
     */
    public ChannelsProperty createChannelsProperty()
    {
        try
        {
            return create(ChannelsProperty.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }

    /**
     * @return
     */
    public XmlProperty createXmlProperty()
    {
        try
        {
            return create(XmlProperty.class);
        }
        catch (MethodParameterIsNullException e)
        {
            // Should never happen
            throw new RuntimeException("WTF ??!", e);
        }
    }
}
