using System;
using urakawa.media;

namespace urakawa.core
{
	/// <summary>
	/// Summary description for ChannelsPropertyCoreNodeValidator.
	/// </summary>
	public class ChannelsPropertyCoreNodeValidator : ICoreNodeValidator
	{
		internal ChannelsPropertyCoreNodeValidator()
		{
    }

    internal static bool DetectMediaOfAncestors(IChannel ch, ICoreNode context)
    {
      ICoreNode parent = (ICoreNode)context.getParent();
      while (parent!=null)
      {
				IChannelsProperty chProp = (IChannelsProperty)parent.getProperty(typeof(ChannelsProperty));
				IMedia mediaOAOS = chProp.getMedia(ch);
				if (mediaOAOS!=null) return true;
				parent = (ICoreNode)parent.getParent();

				//TODO: LNN spotted something that looked like an infinite loop here, please confirm, old code below
				/* 
				IChannelsProperty chProp = (IChannelsProperty)context.getProperty(PropertyType.CHANNEL);
				IMedia mediaOAOS = chProp.getMedia(ch);
				if (mediaOAOS!=null) return true;
				parent = (ICoreNode)context.getParent();
				*/
			}
      return false;
    }

    internal static bool DetectMediaOfSelfOrDescendants(IChannel ch, ICoreNode context)
    {
      DetectMediaCoreNodeVisitor detVis = new DetectMediaCoreNodeVisitor(ch);
      context.acceptDepthFirst(detVis);
      return detVis.hasFoundMedia();
    }

    #region ICoreNodeValidator Members
    /// <summary>
    /// Determines if a given <see cref="IProperty"/> can be set for a given context <see cref="ICoreNode"/>.
    /// </summary>
    /// <param name="newProp">The given <see cref="IProperty"/></param>
    /// <param name="contextNode">The comntext <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if the <see cref="IProperty"/> can be set</returns>
    /// <remarks>Only <see cref="IProperty"/>s of type <see cref="PropertyType.CHANNEL"/>
    /// are tested. <c>true</c> is returned for all other property types.</remarks>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when one of <paramref name="newProp"/> or <paramref name="contextNode"/> are <c>null</c>
    /// </exception>
    public bool canSetProperty(IProperty newProp, ICoreNode contextNode)
    {
      if (newProp==null)
      {
        throw new exception.MethodParameterIsNullException("newProp parameter must not be null");
      }
      if (contextNode==null)
      {
        throw new exception.MethodParameterIsNullException("contextNode parameter must not be null");
      }
			if (newProp.GetType().IsAssignableFrom(typeof(ChannelsProperty)))
			{
        IChannelsProperty chProp = (IChannelsProperty)newProp;
        // Check each used channel for conflicts
        foreach (object oCh in chProp.getListOfUsedChannels())
        {
          IChannel ch = (IChannel)oCh;
          // If there is media attached 
          if (chProp.getMedia(ch)!=null)
          {
            if (DetectMediaOfAncestors(ch, contextNode)) return false;
            if (DetectMediaOfSelfOrDescendants(ch, contextNode)) return false;
          }
        }
      }
			return true;
    }

    /// <summary>
    /// Determines if a given child <see cref="ICoreNode"/> can be removed it's parent
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The given child <see cref="ICoreNode"/></param>
    /// <returns>Always <c>true</c>, since removing a child will never violate
    /// <see cref="IChannelsProperty"/> rules</returns>
    public bool canRemoveChild(ICoreNode node)
    {
      return true;
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted as a child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to insert</param>
    /// <param name="index">The index at which to insert</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a child of <paramref name="contextNode"/> 
    /// at index <paramref name="index"/></returns>
    /// <remarks>The result is independant of the <paramref name="index"/></remarks>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when <paramref name="node"/> or <paramref name="contextNode"/> is null
    /// </exception>
    public bool canInsert(ICoreNode node, int index, ICoreNode contextNode)
    {
      if (node==null)
      {
        throw new exception.MethodParameterIsNullException("node parameter must not be null");
      }
      if (contextNode==null)
      {
        throw new exception.MethodParameterIsNullException("contextNode parameter must not be null");
      }
      foreach (object oCh in contextNode.getPresentation().getChannelsManager().getListOfChannels())
      {
        if (DetectMediaOfAncestors((IChannel)oCh, contextNode))
        {
          if (DetectMediaOfSelfOrDescendants((IChannel)oCh, node))
          {
            return false;
          }
        }
      }
      return true;
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted before a given anchor <see cref="ICoreNode"/>
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling before <paramref name="anchorNode"/>
    /// </returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when one of <paramref name="node"/> or <paramref name="anchorNode"/> are <c>null</c>
    /// </exception>
    public bool canInsertBefore(ICoreNode node, ICoreNode anchorNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return canInsert(node, 0, (ICoreNode)anchorNode.getParent());
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be inserted after a given anchor <see cref="ICoreNode"/>
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The given <see cref="ICoreNode"/> to be tested for insertion</param>
    /// <param name="anchorNode">The anchor <see cref="ICoreNode"/></param>
    /// <returns>
    /// A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can be inserted as a sibling after <paramref name="anchorNode"/>
    /// </returns>
    /// <exception cref="exception.MethodParameterIsNullException">
    /// Thrown when one of <paramref name="node"/> or <paramref name="anchorNode"/> are <c>null</c>
    /// </exception>
    public bool canInsertAfter(ICoreNode node, ICoreNode anchorNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return canInsert(node, 0, (ICoreNode)anchorNode.getParent());
    }

    /// <summary>
    /// Determines if a <see cref="ICoreNode"/> can replace another <see cref="ICoreNode"/>
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to replace with</param>
    /// <param name="oldNode">The <see cref="ICoreNode"/> to be replaced</param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> 
    /// can replace <paramref name="oldNode"/> in the list of children 
    /// of the parent of <paramref name="oldNode"/></returns>
    public bool canReplaceChild(ICoreNode node, ICoreNode oldNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return canInsert(node, 0, (ICoreNode)oldNode.getParent());
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can replace the child 
    /// of a given context <see cref="ICoreNode"/> at a given index
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> with which to replace</param>
    /// <param name="index">The index of the child to replace</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indicating if <paramref name="node"/> can replace 
    /// the child of <paramref name="context"/> at index <paramref name="index"/></returns>
    public bool canReplaceChild(ICoreNode node, int index, ICoreNode contextNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return canInsert(node, index, contextNode);
    }

    /// <summary>
    /// Determines if the child at a given index can be removed from a given context <see cref="ICoreNode"/>
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="index">The given index</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>Always <c>true</c>, since removing a child will never violate
    /// <see cref="IChannelsProperty"/> rules</returns>
    public bool canRemoveChild(int index, ICoreNode contextNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return true;
    }

    /// <summary>
    /// Determines if a given <see cref="ICoreNode"/> can be appended to a given context <see cref="ICoreNode"/>
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="node">The <see cref="ICoreNode"/> to append</param>
    /// <param name="contextNode">The context <see cref="ICoreNode"/></param>
    /// <returns>A <see cref="bool"/> indocating if <paramref name="node"/> can be appended to 
    /// the list of children of <paramref name="contextNode"/></returns>
    public bool canAppendChild(ICoreNode node, ICoreNode contextNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return canInsert(node, contextNode.getChildCount(), contextNode);
    }

    /// <summary>
    /// Determines if a given context <see cref="ICoreNode"/> can be detached from it's parent
    /// without violating any <see cref="IChannelsProperty"/> rules
    /// </summary>
    /// <param name="contextNode">The content <see cref="ICoreNode"/></param>
    /// <returns>
    /// Always <c>true</c> since detaching a <see cref="ICoreNode"/> 
    /// will never violate <see cref="IChannelsProperty"/> rules
    /// </returns>
    public bool canDetach(ICoreNode contextNode)
    {
      //TODO: Check that the proper exceptions are thrown
      return true;
    }

    #endregion
  }
}
