using System;
using System.Xml;
using urakawa.core;
using urakawa.property;
using urakawa.property.xml;

namespace urakawa.examples
{
    /// <summary>
    /// Example implementation of a custom <see cref="Property"/>
    /// </summary>
    public class ExampleCustomProperty : XmlProperty
    {
        /// <summary>
        /// The data of the custom property
        /// </summary>
        public string CustomData = "";

        internal ExampleCustomProperty() : base()
        {
        }

        /// <summary>
        /// Creates a copy of the <see cref="ExampleCustomProperty"/>
        /// </summary>
        /// <returns>The copy</returns>
        public new ExampleCustomProperty Copy()
        {
            return CopyProtected() as ExampleCustomProperty;
        }

        /// <summary>
        /// Protected version of <see cref="Copy"/> - in place as part of a technicality to have <see cref="Copy"/>
        /// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
        /// </summary>
        /// <returns>The copy</returns>
        protected override Property CopyProtected()
        {
            ExampleCustomProperty exProp = base.CopyProtected() as ExampleCustomProperty;
            if (exProp == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The property factory can not create a ExampleCustomProperty matching QName {0}:{1}",
                                                                         XukNamespaceUri, XukLocalName));
            }
            exProp.CustomData = CustomData;
            return exProp;
        }

        /// <summary>
        /// Exports the <see cref="ExampleCustomProperty"/> to a destination <see cref="Presentation"/>
        /// </summary>
        /// <param name="destPres">The destination presentation</param>
        /// <returns>The exported <see cref="ExampleCustomProperty"/></returns>
        public new ExampleCustomProperty Export(Presentation destPres)
        {
            return exportProtected(destPres) as ExampleCustomProperty;
        }

        /// <summary>
        /// Protected version of <see cref="Export"/> - in place as part of a technicality to have <see cref="Export"/>
        /// return <see cref="ExampleCustomProperty"/> instead of <see cref="XmlProperty"/>
        /// </summary>
        /// <returns>The export</returns>
        protected override Property exportProtected(Presentation destPres)
        {
            ExampleCustomProperty exProp = base.exportProtected(destPres) as ExampleCustomProperty;
            if (exProp == null)
            {
                throw new exception.FactoryCannotCreateTypeException(String.Format(
                                                                         "The property factory can not create a ExampleCustomProperty matching QName {0}:{1}",
                                                                         XukNamespaceUri, XukLocalName));
            }
            exProp.CustomData = CustomData;
            return exProp;
        }

        #region IXUKAble Members

        /// <summary>
        /// Reads data from the attributes of the ExampleCustomProperty element
        /// </summary>
        /// <param name="source">The source xml reader</param>
        protected override void xukInAttributes(XmlReader source)
        {
            base.xukInAttributes(source);
            CustomData = source.GetAttribute("customData");
        }

        /// <summary>
        /// Writes data to attributes of the ExampleCustomProperty element
        /// </summary>
        /// <param name="destination">The destination xml writer</param>
        /// <param name="baseUri">
        /// The base <see cref="Uri"/> used to make written <see cref="Uri"/>s relative, 
        /// if <c>null</c> absolute <see cref="Uri"/>s are written
        /// </param>
        protected override void xukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            if (CustomData != null)
            {
                destination.WriteAttributeString("customData", CustomData);
            }
            base.xukOutAttributes(destination, baseUri);
        }

        /// <summary>
        /// Gets the namespace uri part of the QName representing a <see cref="ExampleCustomProperty"/> in Xuk
        /// </summary>
        /// <returns>The namespace uri part</returns>
        public override string XukNamespaceUri
        {
            get { return ExampleCustomDataModelFactory.EX_CUST_NS; }
        }

        #endregion

        #region IValueEquatable<Property> Members

        /// <summary>
        /// Comapres <c>this</c> with a given other <see cref="Property"/> for equality
        /// </summary>
        /// <param name="other">The other <see cref="Property"/></param>
        /// <returns><c>true</c> if equal, otherwise <c>false</c></returns>
        public override bool ValueEquals(Property other)
        {
            if (!base.ValueEquals(other)) return false;
            if (CustomData != ((ExampleCustomProperty) other).CustomData) return false;
            return true;
        }

        #endregion
    }
}