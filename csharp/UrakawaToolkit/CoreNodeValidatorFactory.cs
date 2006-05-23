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

		//this function is here to satisfy the interface requirement
		ICoreNodeValidator ICoreNodeValidatorFactory.createNodeValidator()
		{
			return createNodeValidator();
		}

		/// <summary>
		/// This is the default validator in this implementation
		/// </summary>
		/// <returns></returns>
		public CompositeCoreNodeValidator createNodeValidator()
		{
			TreeNodeValidator treeNodeVal = createTreeNodeValidator();
			XmlPropertyCoreNodeValidator xmlPropCNVal = 
				createXmlPropertyCoreNodeValidator();
			ChannelsPropertyCoreNodeValidator channelsPropCNVal = 
				createChannelsPropertyCoreNodeValidator();

			Array validators = new Array.CreateInstance(typeof(ICoreNodeValidator));
			validators.SetValue(0, treeNodeVal);
			validators.SetValue(1, xmlPropCNVal);
			validators.SetValue(2, channelsPropCNVal);
			
			return buildCompositeCoreNodeValidator(validators);
		}

		public CompositeCoreNodeValidator createCustomCompositeNodeValidator(ICoreNodeValidator[] arr)
		{
			return buildCompositeCoreNodeValidator(arr);
		}

		public TreeNodeValidator createTreeNodeValidator()
		{
			return new TreeNodeValidator();
		}

		public XMLPropertyCoreNodeValidator createXmlPropertyCoreNodeValidator()
		{
			return new XMLPropertyCoreNodeValidator();
		}

		public ChannelsPropertyCoreNodeValidator createChannelsPropertyCoreNodeValidator()
		{
			return new ChannelsPropertyCoreNodeValidator();
		}

		private CompositeCoreNodeValidator buildCompositeCoreNodeValidator(ICoreNodeValidator[] arr)
		{
			return new CompositeCoreNodeValidator(arr);
		}

		#endregion
	}
}
