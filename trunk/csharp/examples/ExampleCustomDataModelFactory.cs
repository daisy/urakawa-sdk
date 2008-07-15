using System;
using System.Collections.Generic;
using System.Text;
using urakawa;

namespace urakawa.examples
{
    /// <summary>
    /// A <see cref="DataModelFactory"/> for the example custom data model, including the ability to create:
    /// <list>
    /// <item><see cref="ExampleCustomPropertyFactory"/></item>
    /// </list>
    /// </summary>
    public class ExampleCustomDataModelFactory : DataModelFactory
    {
        /// <summary>
        /// Namespace uri for <see cref="ExampleCustomTreeNode"/> and <see cref="ExampleCustomProperty"/> XUK nodes
        /// </summary>
        public static string EX_CUST_NS = "http://www.daisy.org/urakawa/example";

        /// <summary>
        /// Creates a <see cref="ExampleCustomPropertyFactory"/> 
        /// </summary>
        /// <returns>The <see cref="ExampleCustomPropertyFactory"/></returns>
        public override property.PropertyFactory CreatePropertyFactory()
        {
            return CreatePropertyFactory(typeof (ExampleCustomPropertyFactory).Name, EX_CUST_NS);
        }

        /// <summary>
        /// Creates a <see cref="property.PropertyFactory"/> of type matching a given Xuk QName
        /// </summary>
        /// <param name="localName">The local name part of the given Xuk QName</param>
        /// <param name="namespaceUri">The namespace uri part of the given Xuk QName</param>
        /// <returns>The created <see cref="property.PropertyFactory"/></returns>
        public override property.PropertyFactory CreatePropertyFactory(string localName, string namespaceUri)
        {
            if (localName == typeof (ExampleCustomPropertyFactory).Name && namespaceUri == EX_CUST_NS)
            {
                return new ExampleCustomPropertyFactory();
            }
            return base.CreatePropertyFactory(localName, namespaceUri);
        }
    }
}