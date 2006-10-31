using System;

namespace urakawa.validation.core
{
	/// <summary>
	/// Interface for a factory for creating <see cref="ICoreNodeValidator"/>s
	/// </summary>
	public interface ICoreNodeValidatorFactory
	{
    /// <summary>
    /// Creates the default <see cref="ICoreNodeValidator"/> of any implementation of the factory
    /// </summary>
    /// <returns>The default <see cref="ICoreNodeValidator"/></returns>
		ICoreNodeValidator createNodeValidator();
	}
}
