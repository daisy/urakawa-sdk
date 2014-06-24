using System;
using urakawa.xuk;

namespace urakawa
{
    /// <summary>
    /// Base class for classes that are associated with a <see cref="Presentation"/>,
    /// extends <see cref="xuk.XukAble"/> and is therefore also <see cref="xuk.IXukAble"/>
    /// </summary>
    public abstract class WithPresentation : XukAble, IValueEquatable<WithPresentation>
    {

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
                    throw new exception.IsAlreadyInitializedException(
                        String.Format(
                            "The {0} has already been initialized with a Presentation", GetType().Name));
                }
                mPresentation = value;
            }
        }

        public virtual bool ValueEquals(WithPresentation other)
        {
            if (other == null)
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !");
                return false;
            }
            if (other.GetType() != GetType())
            {
                //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
                return false;
            }
            //if (!this.GetType().IsInstanceOfType(other))
            //{
            //    //System.Diagnostics.Debug.Fail("! ValueEquals !"); 
            //    return false;
            //}
            return true;
        }
    }
}