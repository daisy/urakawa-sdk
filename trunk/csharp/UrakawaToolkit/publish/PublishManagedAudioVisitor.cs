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
		public Channel getSourceChannel()
		{
			return mSourceChannel;
		}
		public Channel getDestinationChannel()
		{
			return mDestinationChannel;
		}
		public Uri getDestinationDirectory()
		{
			return mDestinationDirectory;
		}
		public string getAudioFileBaseNameFormat()
		{
			return mAudioFileBaseNameFormat;
		}
		public int getCurrentAudioFileNumber()
		{
			return mCurrentAudioFileNumber;
		}
		public void resetAudioFileNumbering()
		{
			mCurrentAudioFileNumber = 0;
		}
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
			res = new Uri(res, String.Format(getAudioFileBaseNameFormat(), getCurrentAudioFileNumber()));
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

		public bool preVisit(TreeNode node)
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
					eam.setSrc(node.getPresentation().getBaseUri().MakeRelative(getCurrentAudioFileUri()).ToString());
					eam.setClipBegin(clipBegin);
					eam.setClipEnd(clipEnd);
					chProp.setMedia(mDestinationChannel, eam);
				}
			}
			return true;
		}

		public void postVisit(TreeNode node)
		{
			//Nothing is done in postVisit visit
		}

		#endregion
	}

	public delegate bool TreeNodeTestDelegate(TreeNode node);
}
