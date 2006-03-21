using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for the core node of the Urakawa model
	/// </summary>
	public interface ICoreNode : IDOMNode, IVisitableNode
	{
    /// <summary>
    /// Gets the <see cref="Presentation"/> that owns the core node
    /// </summary>
    /// <returns>The owner</returns>
    Presentation getPresentation();

    /// <summary>
    /// Gets the <see cref="property.IProperty"/> of the given <see cref="property.PropertyType"/>
    /// </summary>
    /// <param name="type">The given <see cref="property.PropertyType"/></param>
    /// <returns>The <see cref="property.IProperty"/> of the given <see cref="property.PropertyType"/>,
    /// <c>null</c> if no property of the given <see cref="property.PropertyType"/> has been set</returns>
    property.IProperty getProperty(property.PropertyType type);

    /// <summary>
    /// Sets a <see cref="property.IProperty"/>, possible overwriting previously set <see cref="property.IProperty"/>
    /// of the same <see cref="property.PropertyType"/>
    /// </summary>
    /// <param name="prop">The <see cref="property.IProperty"/> to set. 
    /// If <c>null</c> is passed, an <see cref="exception.MethodParameterIsNullException"/> is thrown</param>
    /// <returns>A <see cref="bool"/> indicating if a previously set <see cref="property.IProperty"/>
    /// was overwritten
    /// </returns>
    bool setProperty(property.IProperty prop);
	}
}
