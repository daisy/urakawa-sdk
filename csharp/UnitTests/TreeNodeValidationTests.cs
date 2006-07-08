using System;
using NUnit.Framework;
using urakawa.core;

namespace urakawa.unitTests.testbase
{
	/// <summary>
	/// Summary description for TreeNodeValidationTest.
	/// </summary>
	public class TreeNodeValidationTests : TestCollectionBase
	{
		[Test]public void canAppendNewChildToRoot()
		{
			Presentation pres = mProject.getPresentation();
			CoreNode cn = pres.getCoreNodeFactory().createNode();
			TreeNodeValidator tnv = makeTreeNodeValidator();

			Assert.IsTrue(tnv.canAppendChild(cn, pres.getRootNode()));

			//go ahead and do it just to make sure
			pres.getRootNode().appendChild(cn);
		}
		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canAppendNewChildToNull()
		{
			Presentation pres = mProject.getPresentation();
			CoreNode cn = pres.getCoreNodeFactory().createNode();
			TreeNodeValidator tnv = makeTreeNodeValidator();

			//this should throw the expected exception
			tnv.canAppendChild(cn, null);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canAppendNullChildToRoot()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();

			//this should throw the expected exception
			tnv.canAppendChild(null, pres.getRootNode());
		}
		
		[Test]
		public void canAppendChildWhichAlreadyHasParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			
			pres.getRootNode().appendChild(cn);
			pres.getRootNode().appendChild(cn2);

			//we shouldn't be able to append here, because cn2 already has a parent
			Assert.IsFalse(tnv.canAppendChild(cn2, cn));

			//try to detach first and then append (this should work)
			bool can_detach = tnv.canDetach(cn2);

			Assert.IsTrue(can_detach);

			//go ahead and do it just to make sure
			cn2.detach();

			Assert.IsTrue(tnv.canAppendChild(cn2, cn));

			//go ahead and do it just to make sure
			cn.appendChild(cn2);

		}

		[Test]
		public void canDetachNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn);

			Assert.IsTrue(tnv.canDetach(cn));

			cn.detach();
		}

		[Test]
		public void canDetachNodeWhoHasNoParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn = pres.getCoreNodeFactory().createNode();

			Assert.IsFalse(tnv.canDetach(cn));
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canDetachNullNode()
		{
			TreeNodeValidator tnv = makeTreeNodeValidator();

			Assert.IsFalse(tnv.canDetach(null));
		}

		[Test]public void canInsertNodeAtBeginning()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			Assert.IsTrue(tnv.canInsert(cn_new, 0, pres.getRootNode()));

			pres.getRootNode().insert(cn_new, 0);
		}

		[Test]public void canInsertNodeInMiddle()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			Assert.IsTrue(tnv.canInsert(cn_new, 1, pres.getRootNode()));

			pres.getRootNode().insert(cn_new, 1);
		}

		[Test]public void canInsertNodeAtEnd()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			Assert.IsTrue(tnv.canInsert(cn_new, 2, pres.getRootNode()));

			pres.getRootNode().insert(cn_new, 2);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canInsertNodeBeforeBeginning()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			tnv.canInsert(cn_new, -1, pres.getRootNode());
		}

		[Test]
		public void canInsertNodeAfterEnd()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			Assert.IsTrue(tnv.canInsert(cn_new, 3, pres.getRootNode()));

			pres.getRootNode().insert(cn_new, 3);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canInsertNodeWayAfterEnd()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			CoreNode cn3 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);
			pres.getRootNode().appendChild(cn3);

			tnv.canInsert(cn_new, 4, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canInsertNullNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
		
			//this should throw the expected exception
			tnv.canInsert(null, 0, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canInsertNodeToNullParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);

			//this should throw the expected exception
			tnv.canInsert(cn1, 0, null);
		}

		[Test]
		public void canInsertNodeWhoAlreadyHasParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);

			Assert.IsFalse(tnv.canInsert(cn2, 0, pres.getRootNode()));
		}

		[Test]
		public void canInsertIntoEmptySequence()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();

			Assert.IsTrue(tnv.canInsert(cn1, 0, pres.getRootNode()));
		}


		[Test]
		public void canInsertNodeAfterRelativeNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
			
			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canInsertAfter(cn_new, cn1));

			pres.getRootNode().insertAfter(cn_new, cn1);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canInsertNodeAfterNullNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
		
			//this should throw the expected exception
			tnv.canInsertAfter(cn_new, null);
		}

		[Test]
		public void canInsertNodeAfterAnotherNodeWhoHasANullParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
			
			//cn1 has no parent so it shouldn't be possible to
			//insert a node after it
			Assert.IsFalse(tnv.canInsertAfter(cn_new, cn1));
		}

		[Test]
		public void canInsertNodeWhoAlreadyHasAParentAfterAnotherNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);

			//cn2 already has a parent
			Assert.IsFalse(tnv.canInsertAfter(cn2, cn1));
		}

		[Test]
		public void canInsertNodeBeforeRelativeNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
			
			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canInsertBefore(cn_new, cn1));

			pres.getRootNode().insertBefore(cn_new, cn1);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canInsertNodeBeforeNullNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
		
			//this should throw the expected exception
			tnv.canInsertBefore(cn_new, null);
		}

		[Test]
		public void canInsertNodeBeforeAnotherNodeWhoHasANullParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn_new = pres.getCoreNodeFactory().createNode();
			
			//cn1 has no parent so it shouldn't be possible to
			//insert a node before it
			Assert.IsFalse(tnv.canInsertBefore(cn_new, cn1));
		}

		[Test]
		public void canInsertNodeWhoAlreadyHasAParentBeforeAnotherNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);

			//cn2 already has a parent
			Assert.IsFalse(tnv.canInsertBefore(cn2, cn1));
		}

		[Test]
		public void canRemoveChild()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
		
			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canRemoveChild(cn1));

			pres.getRootNode().removeChild(cn1);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canRemoveNullChild()
		{
			TreeNodeValidator tnv = makeTreeNodeValidator();
			
			//triggers exception
			tnv.canRemoveChild(null);
		}

		[Test]
		public void canRemoveChildFromNullParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
		
			//cn1 has no parent
			Assert.IsFalse(tnv.canRemoveChild(cn1));
		}
		
		[Test]
		public void canRemoveChildAtIndex()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
		
			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canRemoveChild(0, pres.getRootNode()));

			pres.getRootNode().removeChild(0);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canRemoveChildAtTooLowIndex()
		{
			TreeNodeValidator tnv = makeTreeNodeValidator();
			
			Presentation pres = mProject.getPresentation();
			
			//triggers exception
			tnv.canRemoveChild(-1, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canRemoveChildAtTooHighIndex()
		{
			TreeNodeValidator tnv = makeTreeNodeValidator();
			Presentation pres = mProject.getPresentation();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			pres.getRootNode().appendChild(cn1);
			
			//triggers exception
			tnv.canRemoveChild(2, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canRemoveChildAtIndexFromNullParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			
			//triggers exception
			tnv.canRemoveChild(0, null);
		}
		
		[Test]
		public void canReplaceChildAtIndex()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canReplaceChild(cn2, 0, pres.getRootNode()));

			pres.getRootNode().replaceChild(cn2, 0);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canReplaceChildAtIndexWithNullNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);

			//triggers exception
			tnv.canReplaceChild(null, 0, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canReplaceChildAtTooLowIndex()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			
			pres.getRootNode().appendChild(cn1);

			//triggers exception
			tnv.canReplaceChild(cn2, -1, pres.getRootNode());
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsOutOfBoundsException))]
		public void canReplaceChildAtTooHighIndex()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();
			
			pres.getRootNode().appendChild(cn1);

			//triggers exception
			tnv.canReplaceChild(cn2, 1, pres.getRootNode());
		}

		[Test]
		public void canReplaceChildAtIndexWithNodeWhoAlreadyHasAParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);

			Assert.IsFalse(tnv.canReplaceChild(cn2, 0, pres.getRootNode()));
		}

		[Test]
		public void canReplaceChild()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			
			Assert.IsTrue(tnv.canReplaceChild(cn2, cn1));

			pres.getRootNode().replaceChild(cn2, cn1);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canReplaceChildWithNullNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);

			//triggers exception
			tnv.canReplaceChild(null, cn1);
		}

		[Test]
		[ExpectedException(typeof(exception.MethodParameterIsNullException))]
		public void canReplaceNullChildWithNewNode()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			
			//triggers exception
			tnv.canReplaceChild(null, cn1);
		}

		[Test]
		public void canReplaceChildWhoHasNoParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			Assert.IsFalse(tnv.canReplaceChild(cn1, cn2));
		}

		[Test]
		public void canReplaceChildWithNodeWhoAlreadyHasAParent()
		{
			Presentation pres = mProject.getPresentation();
			TreeNodeValidator tnv = makeTreeNodeValidator();
			CoreNode cn1 = pres.getCoreNodeFactory().createNode();
			CoreNode cn2 = pres.getCoreNodeFactory().createNode();

			pres.getRootNode().appendChild(cn1);
			pres.getRootNode().appendChild(cn2);

			Assert.IsFalse(tnv.canReplaceChild(cn1, cn2));
		}


		//this function is here to simplify a little bit
		private TreeNodeValidator makeTreeNodeValidator()
		{
			CoreNodeValidatorFactory fact = new CoreNodeValidatorFactory();
			return fact.createTreeNodeValidator();
		}

	}
}
