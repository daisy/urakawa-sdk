using System;

namespace urakawa.core
{
	/// <summary>
	/// 
	/// </summary>
	public interface ICoreNodeValidatorFactory
	{
		ICoreNodeValidator createNodeValidator(CoreNodeValidatorType type);
	}
}
