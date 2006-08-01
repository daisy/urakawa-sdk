using System;

namespace urakawa.core
{
	/// <summary>
	/// Default implementation of <see cref="ICoreNodeValidatorFactory"/>
	/// </summary>
	public class CoreNodeValidatorFactory:ICoreNodeValidatorFactory
	{
    /// <summary>
    /// Default constructor
    /// </summary>
		public CoreNodeValidatorFactory()
		{
			// Nothing in particular is done
		}
		#region ICoreNodeValidatorFactory Members

		//this function is here to satisfy the interface requirement
		ICoreNodeValidator ICoreNodeValidatorFactory.createNodeValidator()
		{
			return createNodeValidator();
		}

		/// <summary>
		/// Creates the default validator in this implementation, 
		/// which is a <see cref="CompositeCoreNodeValidator"/> consisting of
		/// a <see cref="TreeNodeValidator"/>, a <see cref="XMLPropertyCoreNodeValidator"/> 
		/// and a <see cref="ChannelsPropertyCoreNodeValidator"/>
		/// </summary>
		/// <returns>The default <see cref="CompositeCoreNodeValidator"/></returns>
		public CompositeCoreNodeValidator createNodeValidator()
		{
			TreeNodeValidator treeNodeVal = createTreeNodeValidator();
			XMLPropertyCoreNodeValidator xmlPropCNVal = 
				createXmlPropertyCoreNodeValidator();
			ChannelsPropertyCoreNodeValidator channelsPropCNVal = 
				createChannelsPropertyCoreNodeValidator();

			ICoreNodeValidator[] validators = new ICoreNodeValidator[]
				{treeNodeVal, xmlPropCNVal, channelsPropCNVal};
			
			return createCustomCompositeNodeValidator(validators);
		}

    /// <summary>
    /// Creates a custon <see cref="CompositeCoreNodeValidator"/> 
    /// consisting of given member <see cref="ICoreNodeValidator"/>s
    /// </summary>
    /// <param name="arr">The array of given <see cref="ICoreNodeValidator"/>s</param>
    /// <returns>The custon <see cref="CompositeCoreNodeValidator"/></returns>
		public CompositeCoreNodeValidator createCustomCompositeNodeValidator(ICoreNodeValidator[] arr)
		{
			return new CompositeCoreNodeValidator(arr);
		}

    /// <summary>
    /// Creates a <see cref="TreeNodeValidator"/>
    /// </summary>
    /// <returns>The created <see cref="TreeNodeValidator"/></returns>
		public TreeNodeValidator createTreeNodeValidator()
		{
			return new TreeNodeValidator();
		}

    /// <summary>
    /// Creates a <see cref="XMLPropertyCoreNodeValidator"/>
    /// </summary>
    /// <returns>The created <see cref="XMLPropertyCoreNodeValidator"/></returns>
    public XMLPropertyCoreNodeValidator createXmlPropertyCoreNodeValidator()
		{
			return new XMLPropertyCoreNodeValidator();
		}

    /// <summary>
    /// Creates a <see cref="ChannelsPropertyCoreNodeValidator"/>
    /// </summary>
    /// <returns>The created <see cref="ChannelsPropertyCoreNodeValidator"/></returns>
    public ChannelsPropertyCoreNodeValidator createChannelsPropertyCoreNodeValidator()
		{
			return new ChannelsPropertyCoreNodeValidator();
		}

		#endregion
	}
}
