using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;
using urakawa.exception;
using urakawa.xuk;

namespace urakawa
{
    /// <summary>
    /// Generic base class for creation of instances of types implementing <see cref="WithPresentation"/>
    /// </summary>
    /// <typeparam name="T">The base <see cref="Type"/> of instances created by the factory</typeparam>
    public abstract class GenericWithPresentationFactory<T> : GenericXukAbleFactory<T> where T : WithPresentation
    {

        public override bool IsPrettyFormat()
        {
            return Presentation.IsPrettyFormat();
        }

        public override void SetPrettyFormat(bool pretty)
        {
            Presentation.SetPrettyFormat(pretty);
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
