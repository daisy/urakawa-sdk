using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.property.channel;
using urakawa.media;
using urakawa.media.timing;
using urakawa.media.data.audio;

namespace urakawa.publish
{
	/// <summary>
	/// 
	/// </summary>
	public class PublishManagedAudioVisitor : ITreeNodeVisitor
	{
		private Channel mSourceChannel;
		private Channel mDestinationChannel;
		private Uri mDestinationDirectory;
		private string mAudioFileBaseNameFormat = "aud{0:0}.wav";
		private int mCurrentAudioFileNumber;
		private TreeNodeTestDelegate mTreeNodeGeneratesNewAudioFileTest;
		private PCMFormatInfo mCurrentAudioFilePCMFormat = null;
		private Stream mCurrentAudioFileStream = null;

		/// <summary>
		/// Gets the source <see cref="Channel"/> from which the <see cref="ManagedAudioMedia"/> to publish is retrieved
		/// </summary>
		/// <returns>The source channel</returns>
		public Channel getSourceChannel()
		{
			return mSourceChannel;
		}

		/// <summary>
		/// The destination channel to which the published audio is added as <see cref="ExternalAudioMedia"/>
		/// </summary>
		/// <returns>The destination channel</returns>
		public Channel getDestinationChannel()
		{
			return mDestinationChannel;
		}

		/// <summary>
		/// The directory in which the published audio files are created
		/// </summary>
		/// <returns></returns>
		public Uri getDestinationDirectory()
		{
			return mDestinationDirectory;
		}

		/// <summary>
		/// Gets the format of the name of the published audio files - format parameter 0 is the number of the audio file (1, 2, ...)
		/// </summary>
		/// <returns>The audio file name format</returns>
		public string getAudioFileNameFormat()
		{
			return mAudioFileBaseNameFormat;
		}

		/// <summary>
		/// Gets the number of the current audio file
		/// </summary>
		/// <returns>The current audio file number</returns>
		public int getCurrentAudioFileNumber()
		{
			return mCurrentAudioFileNumber;
		}

		/// <summary>
		/// Resets the audio file numbering, setting the current audio file number to 0. 
		/// </summary>
		public void resetAudioFileNumbering()
		{
			mCurrentAudioFileNumber = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool treeNodeGeneratesNewAudioFile(TreeNode node)
		{
			if (mTreeNodeGeneratesNewAudioFileTest != null)
			{
				return mTreeNodeGeneratesNewAudioFileTest(node);
			}
			return false;
		}
		public void writeCurrentAudioFile()
		{
			if (mCurrentAudioFileStream!=null && mCurrentAudioFilePCMFormat!=null)
			{
				Uri file = getCurrentAudioFileUri();
				FileStream fs = new FileStream(
					file.LocalPath,
					FileMode.Create, FileAccess.Write, FileShare.Read);
				try
				{
					PCMDataInfo pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
					pcmData.setDataLength((uint)mCurrentAudioFileStream.Length);
					pcmData.writeRiffWaveHeader(fs);
					mCurrentAudioFileStream.Position = 0;
					BinaryReader rd = new BinaryReader(mCurrentAudioFileStream);
					byte[] data = rd.ReadBytes((int)mCurrentAudioFileStream.Length);
					rd.Close();
					fs.Write(data, 0, data.Length);
					mCurrentAudioFileStream = null;
					mCurrentAudioFilePCMFormat = null;
				}
				finally
				{
					fs.Close();
				}
			}
		}
		public Uri getCurrentAudioFileUri()
		{
			Uri res = getDestinationDirectory();
			res = new Uri(res, String.Format(getAudioFileNameFormat(), getCurrentAudioFileNumber()));
			return res;
		}
		public void createNextAudioFile()
		{
			writeCurrentAudioFile();
			mCurrentAudioFileNumber++;
			mCurrentAudioFileStream = new MemoryStream();
		}

		public PublishManagedAudioVisitor(Channel sourceCh, Channel destCh, Uri destDir, TreeNodeTestDelegate newAudioFileTestDelegate)
		{
			if (sourceCh == null) throw new exception.MethodParameterIsNullException("The source Channel cannot be null");
			mSourceChannel = sourceCh;
			if (destCh == null) throw new exception.MethodParameterIsNullException("The destination Channel cannot be null");
			mDestinationChannel = destCh;
			if (destDir == null) throw new exception.MethodParameterIsNullException("The Uri of the destination directory cannot be null");
			destDir = new Uri(destDir, "./");
			mDestinationDirectory = destDir;
			if (!Directory.Exists(getDestinationDirectory().LocalPath))
			{
				Directory.CreateDirectory(getDestinationDirectory().LocalPath);
			}
			mTreeNodeGeneratesNewAudioFileTest = newAudioFileTestDelegate;
			resetAudioFileNumbering();
		}

		#region ITreeNodeVisitor Members

		public virtual bool preVisit(TreeNode node)
		{
			if (treeNodeGeneratesNewAudioFile(node)) createNextAudioFile();
			if (node.hasProperties(typeof(ChannelsProperty)))
			{
				ChannelsProperty chProp = node.getProperty<ChannelsProperty>();
				if (chProp.getMedia(getDestinationChannel())!=null) chProp.setMedia(getDestinationChannel(), null);
				ManagedAudioMedia mam = chProp.getMedia(getSourceChannel()) as ManagedAudioMedia;
				if (mam != null)
				{
					AudioMediaData amd = mam.getMediaData();
					if (mCurrentAudioFilePCMFormat == null)
					{
						mCurrentAudioFilePCMFormat = amd.getPCMFormat();
					}					
					if (mCurrentAudioFileStream==null || !mCurrentAudioFilePCMFormat.valueEquals(amd.getPCMFormat()))
					{
						createNextAudioFile();
						mCurrentAudioFilePCMFormat = amd.getPCMFormat();
					}
					BinaryReader rd = new BinaryReader(amd.getAudioData());
					Time clipBegin = Time.Zero.addTimeDelta(mCurrentAudioFilePCMFormat.getDuration((uint)mCurrentAudioFileStream.Position));
					Time clipEnd = clipBegin.addTimeDelta(amd.getAudioDuration());
					try
					{
						byte[] data = rd.ReadBytes(amd.getPCMLength());
						mCurrentAudioFileStream.Write(data, 0, data.Length);
					}
					finally
					{
						rd.Close();
					}
					ExternalAudioMedia eam = node.getPresentation().getMediaFactory().createMedia(
						typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS) as ExternalAudioMedia;
					if (eam == null)
					{
						throw new exception.FactoryCannotCreateTypeException(String.Format(
							"The media facotry cannot create a ExternalAudioMedia matching QName {1}:{0}",
							typeof(ExternalAudioMedia).Name, ToolkitSettings.XUK_NS));
					}
					eam.setLanguage(mam.getLanguage());
					eam.setSrc(node.getPresentation().getRootUri().MakeRelativeUri(getCurrentAudioFileUri()).ToString());
					eam.setClipBegin(clipBegin);
					eam.setClipEnd(clipEnd);
					chProp.setMedia(mDestinationChannel, eam);
				}
			}
			return true;
		}

		public virtual void postVisit(TreeNode node)
		{
			//Nothing is done in postVisit visit
		}

		#endregion
	}

	public delegate bool TreeNodeTestDelegate(TreeNode node);
}
