using System;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for CoreNodeValidatorFactory.
	/// </summary>
	public class CoreNodeValidatorFactory:ICoreNodeValidatorFactory
	{
		public CoreNodeValidatorFactory()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		#region ICoreNodeValidatorFactory Members


		//@todo
		//the problem with this function is that it exposes to the end-user the ability to make
		//different kinds of validators, whereas (I think!) we would prefer if they
		//only used a Composite validator
		public ICoreNodeValidator createNodeValidator(urakawa.core.CoreNodeValidatorType type)
		{
			if (type == CoreNodeValidatorType.CHANNELSPROPERTY)
			{
				return createChannelsPropertyCoreNodeValidator();
			}
			else if (type == CoreNodeValidatorType.XMLPROPERTY)
			{
				return createXmlPropertyCoreNodeValidator();
			}
			else if (type == CoreNodeValidatorType.TREENODE)
			{
				return createTreeNodeValidator();
			}
			else if (type == CoreNodeValidatorType.COMPOSITE)
			{
				return createCompositeCoreNodeValidator();
			}
		}

		private ICoreNodeValidator createCompositeNodeValidator()
		{
			return new CompositeCoreNodeValidator();
		}

		private ICoreNodeValidator createTreeNodeValidator()
		{
			return new TreeNodeValidator();
		}

		private ICoreNodeValidator createXmlPropertyCoreNodeValidator()
		{
			return new XmlPropertyCoreNodeValidator();
		}

		private ICoreNodeValidator createChannelsPropertyCoreNodeValidator()
		{
			return new ChannelsPropertyCoreNodeValidator();
		}

		#endregion
	}
}
