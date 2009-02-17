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
	/// An abstract <see cref="ITreeNodeVisitor"/> that publishes <see cref="ManagedAudioMedia"/> 
	/// from a source <see cref="Channel"/> to a destination <see cref="Channel"/> as <see cref="ExternalAudioMedia"/>.
	/// In concrete implementations of the abstract visitor, 
	/// methods <see cref="treeNodeTriggersNewAudioFile"/> and <see cref="treeNodeMustBeSkipped"/> 
	/// must be implemented to control which <see cref="TreeNode"/>s trigger the generation of a new audio file
	/// and which <see cref="TreeNode"/>s are skipped.
	/// After visitation the <see cref="writeCurrentAudioFile"/> method must be called to ensure that
	/// the current audio file is written to disk.
	/// </summary>
	public abstract class AbstractPublishManagedAudioVisitor : ITreeNodeVisitor
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		protected AbstractPublishManagedAudioVisitor()
		{
			resetAudioFileNumbering();
		}

		private Channel mSourceChannel;
		private Channel mDestinationChannel;
		private Uri mDestinationDirectory;
		private string mAudioFileBaseNameFormat = "aud{0:0}.wav";
		private int mCurrentAudioFileNumber;
		private PCMFormatInfo mCurrentAudioFilePCMFormat = null;
		private Stream mCurrentAudioFileStream = null;
	    private uint mCurrentAudioFileStreamRiffWaveHeaderLength = 0;

		/// <summary>
		/// Gets the source <see cref="Channel"/> from which the <see cref="ManagedAudioMedia"/> to publish is retrieved
		/// </summary>
		/// <returns>The source channel</returns>
		public Channel getSourceChannel()
		{
			if (mSourceChannel == null)
			{
				throw new exception.IsNotInitializedException(
					"The AbstractPublishManagedAudioVisitor has not been inistalized with a source Channel");
			}
			return mSourceChannel;
		}

		/// <summary>
		/// Sets the source <see cref="Channel"/> from which the <see cref="ManagedAudioMedia"/> to publish is retrieved
		/// </summary>
		/// <param name="ch">The new source <see cref="Channel"/></param>
		public void setSourceChannel(Channel ch)
		{
			if (ch == null) throw new exception.MethodParameterIsNullException("The source Channel can not be null");
			mSourceChannel = ch;
		}

		/// <summary>
		/// Gets the destination <see cref="Channel"/> to which the published audio is added as <see cref="ExternalAudioMedia"/>
		/// </summary>
		/// <returns>The destination channel</returns>
		public Channel getDestinationChannel()
		{
			if (mDestinationChannel == null)
			{
				throw new exception.IsNotInitializedException(
					"The AbstractPublishManagedAudioVisitor has not been inistalized with a destination Channel");
			}
			return mDestinationChannel;
		}

		/// <summary>
		/// Sets the destination <see cref="Channel"/> to which the published audio is added as <see cref="ExternalAudioMedia"/>
		/// </summary>
		/// <param name="ch">The new destination <see cref="Channel"/></param>
		public void setDestinationChannel(Channel ch)
		{
			if (ch == null) throw new exception.MethodParameterIsNullException("The destination Channel can not be null");
			mDestinationChannel = ch;
		}

		/// <summary>
		/// Gets the <see cref="Uri"/> of the destination directory in which the published audio files are created
		/// </summary>
		/// <returns>The destination directory <see cref="Uri"/></returns>
		public Uri getDestinationDirectory()
		{
			if (mDestinationChannel == null)
			{
				throw new exception.IsNotInitializedException(
					"The AbstractPublishManagedAudioVisitor has not ben initialized with a destination directory Uri");
			}
			return mDestinationDirectory;
		}

		/// <summary>
		/// Sets the <see cref="Uri"/> of the destination directory in which the published audio files are created
		/// </summary>
		/// <param name="destDir">The <see cref="Uri"/> of the new destination directory</param>
		public void setDestinationDirectory(Uri destDir)
		{
			if (destDir == null)
			{
				throw new exception.MethodParameterIsNullException("The Uri of the destination directory can not be null");
			}
			mDestinationDirectory = destDir;
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
		/// Controls when new audio files are created. In concrete implementations,
		/// if this method returns <c>true</c> for a given <see cref="TreeNode"/>, 
		/// this <see cref="TreeNode"/> triggers the creation of a new audio file
		/// </summary>
		/// <param name="node">The given node</param>
		/// <returns>A <see cref="bool"/> indicating if the given node triggers a new audio file</returns>
		public abstract bool treeNodeTriggersNewAudioFile(TreeNode node);

		/// <summary>
		/// Controls what <see cref="TreeNode"/> are skipped during publish visitation
		/// </summary>
		/// <param name="node">A <see cref="TreeNode"/> to test</param>
		/// <returns>A <see cref="bool"/> indicating if the <see cref="TreeNode"/> should be skipped</returns>
		public abstract bool treeNodeMustBeSkipped(TreeNode node);


		/// <summary>
		/// Writes the curently active audio file to disk.
		/// </summary>
		public void writeCurrentAudioFile()
		{
			if (mCurrentAudioFileStream != null && mCurrentAudioFilePCMFormat != null)
			{
                PCMDataInfo pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
                pcmData.setDataLength((uint)mCurrentAudioFileStream.Length - mCurrentAudioFileStreamRiffWaveHeaderLength);

                mCurrentAudioFileStream.Position = 0;
                mCurrentAudioFileStream.Seek(0, SeekOrigin.Begin);

                pcmData.writeRiffWaveHeader(mCurrentAudioFileStream);

                mCurrentAudioFileStream.Close();

                mCurrentAudioFileStream = null;
                mCurrentAudioFilePCMFormat = null;
                mCurrentAudioFileStreamRiffWaveHeaderLength = 0;

                /*

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
                */
			}
		}

		private Uri getCurrentAudioFileUri()
		{
			Uri res = getDestinationDirectory();
			res = new Uri(res, String.Format(getAudioFileNameFormat(), getCurrentAudioFileNumber()));
			return res;
		}

		private void createNextAudioFile()
		{
			writeCurrentAudioFile();

            mCurrentAudioFileNumber++;
			//mCurrentAudioFileStream = new MemoryStream();
            
            Uri file = getCurrentAudioFileUri();
            mCurrentAudioFileStream = new FileStream(
                file.LocalPath,
                FileMode.Create, FileAccess.Write, FileShare.Read);
		}

		#region ITreeNodeVisitor Members

		/// <summary>
		/// The pre-visit method does the business logic of publishing the managed audio 
		/// from the source to the destination <see cref="Channel"/>
		/// </summary>
		/// <param name="node">The node being visited</param>
		/// <returns>A <see cref="bool"/> indicating if the children of <paramref name="node"/> should be visited as well</returns>
		public virtual bool preVisit(TreeNode node)
		{
			if (treeNodeMustBeSkipped(node)) return false;
			if (treeNodeTriggersNewAudioFile(node)) createNextAudioFile();
			if (node.hasProperties(typeof(ChannelsProperty)))
			{
				ChannelsProperty chProp = node.getProperty<ChannelsProperty>();
				if (chProp.getMedia(getDestinationChannel())!=null) chProp.setMedia(getDestinationChannel(), null);
				ManagedAudioMedia mam = chProp.getMedia(getSourceChannel()) as ManagedAudioMedia;
				if (mam != null)
				{
					AudioMediaData amd = mam.getMediaData();
                    if (mCurrentAudioFileStream != null && mCurrentAudioFilePCMFormat == null)
                    {
                        mCurrentAudioFilePCMFormat = amd.getPCMFormat();
                        PCMDataInfo pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
                        //pcmData.setDataLength((uint)mCurrentAudioFileStream.Length);
                        pcmData.setDataLength(0);
                        mCurrentAudioFileStreamRiffWaveHeaderLength =
                            (uint) pcmData.writeRiffWaveHeader(mCurrentAudioFileStream);
                    }
				    if (mCurrentAudioFileStream == null || !mCurrentAudioFilePCMFormat.valueEquals(amd.getPCMFormat()))
					{
						createNextAudioFile();
						mCurrentAudioFilePCMFormat = amd.getPCMFormat();
                        PCMDataInfo pcmData = new PCMDataInfo(mCurrentAudioFilePCMFormat);
                        //pcmData.setDataLength((uint)mCurrentAudioFileStream.Length);
                        pcmData.setDataLength(0);
                        mCurrentAudioFileStreamRiffWaveHeaderLength = (uint) pcmData.writeRiffWaveHeader(mCurrentAudioFileStream);
					}
					BinaryReader rd = new BinaryReader(amd.getAudioData());
                    Time clipBegin = Time.Zero.addTimeDelta(mCurrentAudioFilePCMFormat.getDuration((uint)mCurrentAudioFileStream.Position - mCurrentAudioFileStreamRiffWaveHeaderLength));
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

		/// <summary>
		/// Nothing is done in in post-visit
		/// </summary>
		/// <param name="node">The node</param>
		public virtual void postVisit(TreeNode node)
		{
			//Nothing is done in postVisit visit
		}

		#endregion
	}
}
