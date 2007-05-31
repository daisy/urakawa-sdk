using System;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.media;
using urakawa.properties.channel;

namespace urakawa.examples
{
	/// <summary>
	/// Example visitor that collects all <see cref="IMedia"/> in a given channel
	/// </summary>
	/// <example>
	/// Thre following example will 
	/// <code>
	/// using urakawa.core;
	/// using urakawa.examples;
	/// using urakawa.project;
	/// 
	/// public class Program
	/// {
	/// 	[STAThread]
	/// 	static void Main(string[] args)
	/// 	{
	/// 		Project proj = new Project();
	/// 		proj.openXUK(new Uri(System.IO.Path.Combine(
	/// 				System.IO.Directory.GetCurrentDirectory(),
	/// 				args[0])));
	/// 		foreach (Channel ch in proj.getPresentation().getChannelsManager().getListOfChannels())
	/// 		{
	/// 			CollectMediaFromChannelCoreNodeVisitor visitor = new CollectMediaFromChannelCoreNodeVisitor(ch);
	/// 			proj.getPresentation().getRootNode().acceptDepthFirst(visitor);
	/// 			Console.WriteLine(
	/// 					"Channel {0} contains {1:0} media objects",
	/// 					ch.getLocalName(), visitor.CollectedMedia.Length);
	/// 		}
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class CollectMediaFromChannelCoreNodeVisitor : ICoreNodeVisitor
	{
		/// <summary>
		/// An integer that indicates the number of <see cref="ICoreNode"/>s visited
		/// </summary>
		public ulong VisitCount = 0;
		private System.Collections.ArrayList mCollectedMedia;

		/// <summary>
		/// Gets an array of the collected <see cref="IMedia"/>
		/// </summary>
		public IMedia[] CollectedMedia
		{
			get
			{
				return (IMedia[])mCollectedMedia.ToArray(typeof(IMedia));
			}
		}
		private Channel mChannel;

		/// <summary>
		/// Gets the <see cref="Channel"/> from which <see cref="IMedia"/> is collected
		/// </summary>
		public Channel CollectorChannel
		{
			get
			{
				return mChannel;
			}
		}

		/// <summary>
		/// Constructor setting the <see cref="Channel"/> from which media is collected
		/// </summary>
		/// <param name="ch"></param>
		public CollectMediaFromChannelCoreNodeVisitor(Channel ch)
		{
			mCollectedMedia = new System.Collections.ArrayList();
			mChannel = ch;
		}
		#region ICoreNodeVisitor Members

		/// <summary>
		/// Pre-visit action:
		/// If <see cref="IMedia"/> is present in <see cref="Channel"/> <see cref="CollectorChannel"/>,
		/// this is added to <see cref="CollectedMedia"/> and the child <see cref="ICoreNode"/>s are not visited
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		/// <returns>
		/// <c>true</c> is no <see cref="IMedia"/> is present in <see cref="Channel"/> <see cref="CollectorChannel"/>,
		/// <c>false</c> else
		/// </returns>
		public bool preVisit(ICoreNode node)
		{
			bool foundMedia = false;
			ChannelsProperty chProp = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
			if (chProp!=null)
			{
				if (chProp.getMedia(CollectorChannel)!=null)
				{
					foundMedia = true;
					mCollectedMedia.Add(chProp.getMedia(CollectorChannel));
				}
			}
			VisitCount++;
			return !foundMedia;
		}

		/// <summary>
		/// Post-visit action: Nothing is done here
		/// </summary>
		/// <param name="node">The <see cref="ICoreNode"/> to visit</param>
		public void postVisit(ICoreNode node)
		{
			// Nothing is done!!!
		}

		#endregion
	}
}
