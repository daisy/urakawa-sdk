using System;
using System.Xml;
using urakawa.xuk;

namespace urakawa
{
    /// <summary>
    /// Generic base class for creation of instances of types implementing <see cref="WithPresentation"/>
    /// </summary>
    /// <typeparam name="T">The base <see cref="Type"/> of instances created by the factory</typeparam>
    public abstract class GenericWithPresentationFactory<T> : GenericXukAbleFactory<T> where T : WithPresentation
    {
        protected override void XukInChild(XmlReader source, progress.IProgressHandler handler)
        {
            if (source.LocalName == XukStrings.RegisteredTypes && source.NamespaceURI == XukAble.XUK_NS)
            {
                XukInRegisteredTypes(source, handler);
            }
            else if (!Presentation.Project.PrettyFormat)
            {
                XukInRegisteredType(source);
            }

            //we're replacing the super class method completely
            //base.XukInChild(source, handler);
        }
        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, progress.IProgressHandler handler)
        {
            if (Presentation.Project.PrettyFormat)
            {
                destination.WriteStartElement(XukStrings.RegisteredTypes, XukAble.XUK_NS);
            }

            XukOutRegisteredTypes(destination, baseUri, handler);

            if (Presentation.Project.PrettyFormat)
            {
                destination.WriteEndElement();
            }
            //we're replacing the super class method completely
            //base.XukOutChildren(destination, baseUri, handler);
        }


        public override bool PrettyFormat
        {
            set
            {
                throw new NotImplementedException("PrettyFormat");
                //Presentation.PrettyFormat = value;
            }
            get
            {
                return Presentation.PrettyFormat;
            }
        }

        private Presentation mPresentation;

        /// <summary>
        /// Gets the <see cref="Presentation"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The owning <see cref="Presentation"/></returns>
        public Presentation Presentation
        {
            get
            {
                return mPresentation;
            }
        }

        public GenericWithPresentationFactory(Presentation pres)
        {
            mPresentation = pres;
        }

        /// <summary>
        /// Inistalizes an created instance by assigning it an owning <see cref="urakawa.Presentation"/>
        /// </summary>
        /// <param name="instance">The instance to initialize</param>
        /// <remarks>
        /// In derived factories, this method can be overridden in order to do additional initialization.
        /// In this case the developer must remember to call <c>base.InitializeInstance(instance)</c>
        /// </remarks>
        protected override void InitializeInstance(T instance)
        {
            base.InitializeInstance(instance);
            instance.Presentation = Presentation;
        }
    }
}
