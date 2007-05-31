using System;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;
using urakawa.core.property;
using urakawa.properties.channel;


namespace urakawa.examples
{
	/// <summary>
	/// <see cref="ICoreNodeVisitor"/> for detecting <see cref="IMedia"/> in a <see cref="Channel"/>
	/// </summary>
	public class DetectMediaCoreNodeVisitor : ICoreNodeVisitor
	{
    private bool mHasFoundMedia = false;

    /// <summary>
    /// Resets the visitor so that it can be re-used
    /// </summary>
    public void reset()
    {
      mHasFoundMedia = false;
    }

    /// <summary>
    /// Determines is the <see cref="DetectMediaCoreNodeVisitor"/> has detected
    /// </summary>
    /// <returns>
    /// A <see cref="bool"/> indicating if the <see cref="DetectMediaCoreNodeVisitor"/>
    /// has dected any media in <see cref="Channel"/> <see cref="getChannelFromWhichMediaIsDetected"/>()
    /// </returns>
    public bool hasFoundMedia()
    {
      return mHasFoundMedia;
    }

    private Channel mChannel;

    /// <summary>
    /// Gets the <see cref="Channel"/> in which <see cref="IMedia"/> is detected
    /// </summary>
    /// <returns>The <see cref="Channel"/></returns>
    public Channel getChannelFromWhichMediaIsDetected()
    {
      return mChannel;
    }

    /// <summary>
    /// Constructor setting the <see cref="Channel"/> in which the <see cref="DetectMediaCoreNodeVisitor"/> 
    /// detects <see cref="IMedia"/>
    /// </summary>
    /// <param name="channelInWhichToDetect">The <see cref="Channel"/></param>
	public DetectMediaCoreNodeVisitor(Channel channelInWhichToDetect)
    {
      mChannel = channelInWhichToDetect;
    }
    #region ICoreNodeVisitor Members

    /// <summary>
    /// Called before visiting children in in depth first traversal.
    /// If there is a <see cref="IMedia"/> associated with <paramref localName="node"/>
    /// via a <see cref="ChannelsProperty"/>, the <see cref="DetectMediaCoreNodeVisitor"/>
    /// is flagged as having found a <see cref="IMedia"/> in the given channel 
    /// and the traversal is ended
    /// </summary>
    /// <param name="node">The <see cref="CoreNode"/> to visit</param>
    /// <returns>A <see cref="bool"/> indicating if the traversal should 
    /// continue after the current visit</returns>
    public bool preVisit(CoreNode node)
    {
      // If media has already been detected, do nothing more
      if (mHasFoundMedia) return false;
      Property prop = node.getProperty(typeof(ChannelsProperty));
      if (prop!=null)
      {
        ChannelsProperty chProp = (ChannelsProperty)prop;
        IMedia m = chProp.getMedia(mChannel);
        // If media is present in mChannel, flag that media is detected in mChannel 
        // and retrun false to avoid searching the subtree of node
        if (m!=null)
        {
          mHasFoundMedia = true;
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Called after visiting the children in depth first traversal 
    /// - does nothing in the present visitor
    /// </summary>
    /// <param name="node">The <see cref="CoreNode"/> being visited</param>
    public void postVisit(CoreNode node)
    {
      // Nothing is done in post visit which is OK
    }

    #endregion
  }
}
