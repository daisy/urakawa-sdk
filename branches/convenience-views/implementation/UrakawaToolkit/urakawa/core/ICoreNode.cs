using System;
using urakawa.core.visitor;
using urakawa.core.property;
using urakawa.xuk;

namespace urakawa.core
{
	/// <summary>
	/// Interface for the core node of the Urakawa model
	/// </summary>
	public interface ICoreNode : ICoreNodeReadOnlyMethods, ICoreNodeWriteOnlyMethods, IVisitableCoreNode, IXukAble, IValueEquatable<ICoreNode>
	{

    /// <summary>
		/// Gets the <see cref="ICorePresentation"/> that owns the core node
    /// </summary>
    /// <returns>The owner</returns>
    ICorePresentation getPresentation();

    /// <summary>
    /// Gets the <see cref="IProperty"/> of the given <see cref="Type"/>
    /// </summary>
    /// <param localName="propType">The given <see cref="Type"/></param>
    /// <returns>The <see cref="IProperty"/> of the given <see cref="Type"/>,
    /// <c>null</c> if no property of the given <see cref="Type"/> has been set</returns>
    IProperty getProperty(Type propType);

		/// <summary>
		/// Gets an array of the <see cref="Type"/>s of <see cref="IProperty"/> set for the <see cref="ICoreNode"/>
		/// </summary>
		/// <returns>The array</returns>
		Type[] getListOfUsedPropertyTypes();

    /// <summary>
    /// Sets a <see cref="IProperty"/>, possible overwriting previously set <see cref="IProperty"/>
    /// of the same <see cref="Type"/>
    /// </summary>
    /// <param localName="prop">The <see cref="IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="IProperty"/>
    /// was overwritten
    /// </returns>
    bool setProperty(IProperty prop);

		/// <summary>
		/// Remove a <see cref="IProperty"/> of a given <see cref="Type"/>
		/// </summary>
		/// <param localName="propType">The given <see cref="Type"/></param>
		/// <returns>The <see cref="IProperty"/> that was just removed,
		/// <c>null</c> if no <see cref="IProperty"/> of the given type existed</returns>
		IProperty removeProperty(Type propType);
	}
}
