using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.test.unitTests
{
	/// <summary>
	/// Summary description for ForAnEmptyPresentation.
	/// </summary>
	[TestFixture] 
	public class ForAnEmptyPresentation
	{
		urakawa.core.Presentation mPresentation = null;

		[SetUp] public void Init() 
		{
			mPresentation = new Presentation();
		}
		[Test] public void IsPresentationNull()
		{
			Assert.IsNotNull(mPresentation);
		}
		[Test] public void IsRootNodeNull()
		{
			Assert.IsNotNull(mPresentation.getRootNode());
		}
		[Test] public void IsChannelsManagerNull()
		{
			Assert.IsNotNull(mPresentation.getChannelsManager());
		}
		[Test] public void IsChannelFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getChannelFactory());
		}
		[Test] public void IsCoreNodeFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getCoreNodeFactory());
		}
		[Test] public void IsMediaFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getMediaFactory());
		}
		[Test] public void IsPropertyFactoryNull()
		{
			Assert.IsNotNull(mPresentation.getPropertyFactory());
		}
		[Test] public void TryToSetNullProperty()
		{
			urakawa.core.CoreNode root = mPresentation.getRootNode();
			if (root != null)
			{
				bool bGotException = false;
				try
				{
					root.setProperty(null);
				}
				catch (urakawa.exception.MethodParameterIsNullException)
				{
					bGotException = true;
				}
				if (bGotException == false)
				{
					Assert.Fail("Got no exception where one was expected");
				}
			}
		}
		[Test] public void GetRootParent()
		{
			urakawa.core.CoreNode root = mPresentation.getRootNode();
			if (root != null)
			{
				Assert.IsNull(root.getParent(), "Parent of root is null");
			}
		}
		
	}
}
