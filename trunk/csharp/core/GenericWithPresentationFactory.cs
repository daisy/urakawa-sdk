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
    public class GenericWithPresentationFactory<T> : GenericXukAbleFactory<T>, IWithPresentation where T : WithPresentation
    {
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
            instance.Presentation = this.Presentation;
        }

        #region IWithPresentation Members

        private Presentation mPresentation;

        /// <summary>
        /// Gets the <see cref="Presentation"/> associated with <c>this</c>
        /// </summary>
        /// <returns>The owning <see cref="Presentation"/></returns>
        /// <exception cref="exception.IsNotInitializedException">
        /// Thrown when no <see cref="Presentation"/> has been associated with <c>this</c>
        /// </exception>
        public virtual Presentation Presentation
        {
            get
            {
                if (mPresentation == null)
                {
                    throw new exception.IsNotInitializedException(
                        String.Format("The {0} has not been initialized with a Presentation", GetType().Name));
                }
                return mPresentation;
            }
            set
            {
                if (value == null)
                {
                    throw new exception.MethodParameterIsNullException(
                        String.Format("The {0} can not have a null Presentation", GetType().Name));
                }
                if (mPresentation != null)
                {
                    throw new exception.IsAlreadyInitializedException(String.Format(
                                                                          "The {0} has already been initialized with a Presentation",
                                                                          GetType().Name));
                }
                mPresentation = value;
            }
        }

        #endregion
    }
}
