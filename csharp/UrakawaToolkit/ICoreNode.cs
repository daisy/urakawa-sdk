using System;

namespace urakawa.core
{
	/// <summary>
	/// Interface for the core node of the Urakawa model
	/// </summary>
	public interface ICoreNode : IDOMNode, IVisitableNode
	{
    Presentation getPresentation();
    property.IProperty getProperty(property.PropertyType type);
    void setProperty(property.IProperty prop);
	}
}
