using System;

namespace urakawa.core
{
	/// <summary>
	/// <see cref="ICoreNodeVisitor"/> for detecting <see cref="IMedia"/> in a <see cref="IChannel"/>
	/// </summary>
	public class DetectMediaCoreNodeVisitor : ICoreNodeVisitor
	{
    private bool mHasFoundMedia = false;

    /// <summary>
    /// Determines is the <see cref="DetectMediaCoreNodeVisitor"/> has detected
    /// </summary>
    /// <returns>
    /// A <see cref="bool"/> indicating if the <see cref="DetectMediaCoreNodeVisitor"/>
    /// has dected any media in <see cref="IChannel"/> <see cref="getChannelFromWhichMediaIsDetected"/>()
    /// </returns>
    public bool hasFoundMedia()
    {
      return mHasFoundMedia;
    }

    private IChannel mChannel;

    /// <summary>
    /// Gets the <see cref="IChannel"/> in which <see cref="IMedia"/> is detected
    /// </summary>
    /// <returns>The <see cref="IChannel"/></returns>
    public IChannel getChannelFromWhichMediaIsDetected()
    {
      return mChannel;
    }

    /// <summary>
    /// Constructor setting the <see cref="IChannel"/> in which the <see cref="DetectMediaCoreNodeVisitor"/> 
    /// detects <see cref="IMedia"/>
    /// </summary>
    /// <param name="channelFromWhichToExtract">The <see cref="IChannel"/></param>
		public DetectMediaCoreNodeVisitor(IChannel channelInWhichToDetect)
    {
      mChannel = channelInWhichToDetect;
    }
    #region ICoreNodeVisitor Members

    public bool preVisit(ICoreNode node)
    {
      // If media has already been detected, do nothing more
      if (mHasFoundMedia) return false;
      IProperty prop = node.getProperty(PropertyType.CHANNEL);
      if (prop!=null)
      {
        IChannelsProperty chProp = (IChannelsProperty)prop;
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

    public void postVisit(ICoreNode node)
    {
      // Nothing is done in post visit which is OK
    }

    #endregion
  }
}
