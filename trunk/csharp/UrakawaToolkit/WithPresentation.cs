using System;
using System.Collections.Generic;
using System.Text;

namespace urakawa
{
	/// <summary>
	/// Base class for classes that are associated with a <see cref="Presentation"/>
	/// </summary>
	public class WithPresentation : xuk.XukAble, IWithPresentation
	{
		private Presentation mPresentation;

		/// <summary>
		/// Gets the <see cref="Presentation"/> associated with <c>this</c>
		/// </summary>
		/// <returns>The owning <see cref="Presentation"/></returns>
		/// <exception cref="exception.IsNotInitializedException">
		/// Thrown when no <see cref="Presentation"/> has been associated with <c>this</c>
		/// </exception>
		public Presentation getPresentation()
		{
			if (mPresentation == null)
			{
				throw new exception.IsNotInitializedException(String.Format(
					"The {0} has not been initialized with a Presentation",
					GetType().Name));
			}
			return mPresentation;
		}

		/// <summary>
		/// Sets the <see cref="Presentation"/> associated with <c>this</c>. For internal use only!!!
		/// </summary>
		/// <param name="newPres">The Presentation with which to associate</param>
		/// <exception cref="exception.MethodParameterIsNullException">Thrown when <paramref name="newPres"/> is <c>null</c></exception>
		/// <exception cref="exception.IsAlreadyInitializedException">
		/// Thrown when <c>this</c> has already been associated with a <see cref="Presentation"/>
		/// </exception>
		public virtual void setPresentation(Presentation newPres)
		{
			if (newPres==null)
			{
				throw new exception.MethodParameterIsNullException(String.Format(
					"The {0} can not have a null Presentation",
					GetType().Name));
			}
			if (mPresentation!=null)
			{
				throw new exception.IsAlreadyInitializedException(String.Format(
					"The {0} has already been initialized with a Presentation",
					GetType().Name));
			}
			mPresentation = newPres;
		}
	}
}
