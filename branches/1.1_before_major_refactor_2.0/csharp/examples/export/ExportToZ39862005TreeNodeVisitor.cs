using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using urakawa.core;
using urakawa.core.visitor;
using urakawa.media.timing;

namespace urakawa.examples.export
{
	public class ExportToZ39862005TreeNodeVisitor : ITreeNodeVisitor
	{
		public ExportToZ39862005TreeNodeVisitor()
		{
		}

		private string mOutputFolder;
		private urakawa.navigation.AbstractFilterNavigator mLevelNodeNavigator;
		private urakawa.navigation.AbstractFilterNavigator mPageNodeNavigator;
		private urakawa.properties.channel.Channel mAudioChannel;
		private urakawa.properties.channel.Channel mLevelTextChannel;
		private urakawa.properties.channel.Channel mLevelAudioChannel;
		private Dictionary<Uri, TreeNode> mTreeNodeSmilUris = new Dictionary<Uri, TreeNode>();
		private List<string> mSmilFileNames = new List<string>();
		private TimeDelta mTotalElapsedTime = new TimeDelta();
		private XmlWriter mSmilWriter;
		private string mDTBUID;
		private int mLatestSmilIdNo = 0;
		private List<string> mAudioFileNames = new List<string>();
		private MemoryStream mAudioMemoryStream;
		private urakawa.media.data.audio.PCMFormatInfo mPCMFormat;

		public string getDTBUID()
		{
			if (mDTBUID == null || mDTBUID.Trim() == "")
			{
				throw new exception.IsNotInitializedException(
					"The DTB UID of the ExportToZ39862005TreeNodeVisitor has not been initialized");
			}
			return mDTBUID;
		}

		public void setDTBUID(string newValue)
		{
			if (newValue == null)
			{
				throw new exception.MethodParameterIsNullException("The DTB UID can not be null");
			}
			if (newValue.Trim() == "")
			{
				throw new exception.MethodParameterIsEmptyStringException("The DTB UID (trimmed) can not be null");
			}
			mDTBUID = newValue;
		}

		public void clear()
		{
			mTreeNodeSmilUris.Clear();
			mSmilFileNames.Clear();
			mTotalElapsedTime = new TimeDelta();
			mSmilWriter = null;
			mAudioFileNames.Clear();
			mAudioMemoryStream = null;
			mLatestSmilIdNo = 0;
			mPCMFormat = null;
			DirectoryInfo di = new DirectoryInfo(getOutputFolder());
			if (!di.Exists) di.Create();
			foreach (FileSystemInfo fsi in di.GetFileSystemInfos())
			{
				fsi.Delete();
			}
		}

		public string getOutputFolder()
		{
			return mOutputFolder;
		}

		public void setOutputFolder(string newValue)
		{
			mOutputFolder = newValue;
		}

		public urakawa.navigation.AbstractFilterNavigator getLevelNodeNavigator()
		{
			if (mLevelNodeNavigator == null)
			{
				throw new exception.IsNotInitializedException(
					"The ExportToZ39862005TreeNodeVisitor has not been initialized with a LevelNodeNavigator");
			}
			return mLevelNodeNavigator;
		}

		public void setLevelNodeNavigator(urakawa.navigation.AbstractFilterNavigator newValue)
		{
			if (newValue == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The LevelNodeNavigator can not be null");
			}
			mLevelNodeNavigator = newValue;
		}

		public urakawa.navigation.AbstractFilterNavigator getPageNodeNavigator()
		{
			return mPageNodeNavigator;
		}

		public void setPageNodeNavigator(urakawa.navigation.AbstractFilterNavigator newValue)
		{
			mPageNodeNavigator = newValue;
		}

		public urakawa.properties.channel.Channel getAudioChannel()
		{
			if (mAudioChannel == null)
			{
				throw new exception.IsNotInitializedException(
					"The ExportToZ39862005TreeNodeVisitor has not been initialized with an AudioChannel");
			}
			return mAudioChannel;
		}

		public void setAudioChannel(urakawa.properties.channel.Channel newValue)
		{
			if (newValue == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The AudioChannel can not be null");
			}
			mAudioChannel = newValue;
		}

		public urakawa.properties.channel.Channel getLevelTextChannel()
		{
			if (mLevelTextChannel == null)
			{
				throw new exception.IsNotInitializedException(
					"The ExportToZ39862005TreeNodeVisitor has not been initialized with a LevelTextChannel");
			}
			return mLevelTextChannel;
		}

		public void setLevelTextChannel(urakawa.properties.channel.Channel newValue)
		{
			if (newValue == null)
			{
				throw new exception.MethodParameterIsNullException(
					"The LevelTextChannel can not be null");
			}
			mLevelTextChannel = newValue;
		}

		public urakawa.properties.channel.Channel getLevelAudioChannel()
		{
			return mLevelAudioChannel;
		}

		public void setLevelAudioChannel(urakawa.properties.channel.Channel newValue)
		{
			mLevelAudioChannel = newValue;
		}

		public Dictionary<Uri, TreeNode> getTreeNodeSmilUriDictionary()
		{
			return mTreeNodeSmilUris;
		}

		public List<string> getSmilFileNames()
		{
			return mSmilFileNames;
		}

		public TimeDelta getTotalElapsedTime()
		{
			return mTotalElapsedTime;
		}

		private XmlWriter getSmilWriter()
		{
			return mSmilWriter;
		}

		public static string SMIL_NS = "http://www.w3.org/2001/SMIL20/";
		public static string SMIL_PUBLIC = "-//NISO//DTD dtbsmil 2005-1//EN";
		public static string SMIL_DTD_URI = "http://www.daisy.org/z3986/2005/dtbsmil-2005-1.dtd";
		public static string BASE_NAME_FORMAT = "SM{0:00000}";

		private static string timeDeltaToSmilString(TimeDelta time)
		{
			TimeSpan val = time.getTimeDeltaAsTimeSpan();
			return String.Format(
				"{0:00}:{1:00}:{2:00}.{3:000}",
				val.Hours, val.Minutes % 60, val.Seconds % 60, val.Milliseconds % 1000);
		}

		private XmlWriter getNextSmilWriter(string basename)
		{
			XmlWriter curWr = getSmilWriter();
			if (curWr != null)
			{
				curWr.WriteEndDocument();
				curWr.Close();
			}
			getNextAudioStream(basename);
			mLatestSmilIdNo = 0;
			string fileName = String.Format("{0}.smil", basename);
			getSmilFileNames().Add(fileName);
			mSmilWriter = XmlWriter.Create(Path.Combine(getOutputFolder(), fileName));
			curWr = getSmilWriter();
			curWr.WriteStartDocument(true);
			curWr.WriteDocType(
				"smil",
				SMIL_PUBLIC,
				SMIL_DTD_URI,
				null);
			curWr.WriteStartElement("smil", SMIL_NS);

			curWr.WriteStartElement("head", SMIL_NS);

			curWr.WriteStartElement("meta", SMIL_NS);
			curWr.WriteAttributeString("name", "dtb:uid");
			curWr.WriteAttributeString("value", getDTBUID());
			curWr.WriteEndElement();

			System.Reflection.AssemblyName exeAssName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
			curWr.WriteStartElement("meta", SMIL_NS);
			curWr.WriteAttributeString("name", "dtb:generator");
			curWr.WriteAttributeString(
				"value",
				String.Format("{1} v{2} ({0})", GetType().FullName, exeAssName.Name, exeAssName.Version));
			curWr.WriteEndElement();

			curWr.WriteStartElement("meta", SMIL_NS);
			curWr.WriteAttributeString("name", "dtb:totalElapsedTime");
			curWr.WriteAttributeString("value", timeDeltaToSmilString(getTotalElapsedTime()));
			curWr.WriteEndElement();


			curWr.WriteEndElement();

			curWr.WriteStartElement("body", SMIL_NS);
			return curWr;
		}

		private string getCurrentAudioFileName()
		{
			if (mAudioFileNames.Count > 0) return mAudioFileNames[mAudioFileNames.Count - 1];
			return null;
		}

		private urakawa.media.data.audio.PCMFormatInfo getCurrentPCMFormat()
		{
			return mPCMFormat;
		}

		private Stream getNextAudioStream(string basename)
		{
			if (getAudioStream() != null && getCurrentPCMFormat()!=null)
			{
				FileStream fs = new FileStream(Path.Combine(getOutputFolder(), getCurrentAudioFileName()), FileMode.Create, FileAccess.Write);
				urakawa.media.data.audio.PCMDataInfo dataInfo = new urakawa.media.data.audio.PCMDataInfo(getCurrentPCMFormat());
				Stream curAS = getAudioStream();
				curAS.Position = 0;
				dataInfo.setDataLength((uint)curAS.Length);
				dataInfo.writeRiffWaveHeader(fs);
				copyData(curAS, fs, dataInfo.getDataLength());
				fs.Close();
			}
			mAudioMemoryStream = new MemoryStream();
			mPCMFormat = null;
			return getAudioStream();
		}

		private XmlWriter getNextSmilWriter()
		{
			return getNextSmilWriter(String.Format(BASE_NAME_FORMAT, mSmilFileNames.Count));
		}

		private string getNextSmilId()
		{
			mLatestSmilIdNo++;
			return String.Format("smilid{0}", mLatestSmilIdNo);
		}

		private Stream getAudioStream()
		{
			return mAudioMemoryStream;
		}

		private TimeDelta getElapsenInCurrentAudio()
		{
			if (getAudioStream() != null && getCurrentPCMFormat()!=null)
			{
				return getCurrentPCMFormat().getDuration((uint)getAudioStream().Length);
			}
			return new TimeDelta();
		}

		private void copyData(Stream source, Stream dest, uint count)
		{
			uint bytesCopied = 0;
			byte[] buf = new byte[1024];
			while (bytesCopied < count)
			{
				int cpLen = buf.Length;
				if (((uint)cpLen) > count - bytesCopied) cpLen = (int)(count - bytesCopied);
				if (source.Read(buf, 0, cpLen) != cpLen)
				{
					throw new exception.OperationNotValidException(
						"Unexpectedly readhed end of source file");
				}
				dest.Write(buf, 0, cpLen);
			}
		}

		private List<media.data.audio.AudioMediaData> getAudioMediaDataFromSequenceMedia(media.SequenceMedia sequence)
		{
			List<media.data.audio.AudioMediaData> res = new List<urakawa.media.data.audio.AudioMediaData>();
			foreach (media.IMedia m in sequence.getListOfItems())
			{
				if (m is media.data.ManagedAudioMedia)
				{
					res.Add(((media.data.ManagedAudioMedia)m).getMediaData());
				}
				else if (m is media.SequenceMedia)
				{
					res.AddRange(getAudioMediaDataFromSequenceMedia((media.SequenceMedia)m));
				}
			}
			return res;
		}

		#region ITreeNodeVisitor Members


		public bool preVisit(TreeNode node)
		{
			XmlWriter curWr;
			if (getLevelNodeNavigator().isIncluded(node))
			{
				curWr = getNextSmilWriter();
			}
			else
			{
				curWr = getSmilWriter();
			}
			curWr.WriteStartElement("seq", SMIL_NS);
			curWr.WriteAttributeString("id", getNextSmilId());
			properties.channel.ChannelsProperty chProp 
				= node.getProperty(typeof(properties.channel.ChannelsProperty)) as properties.channel.ChannelsProperty;
			if (chProp != null)
			{
				media.IMedia audChMedia = chProp.getMedia(getAudioChannel());
				List<media.data.audio.AudioMediaData> audioMediaData = new List<urakawa.media.data.audio.AudioMediaData>();
				if (audChMedia is media.data.ManagedAudioMedia)
				{
					audioMediaData.Add(((media.data.ManagedAudioMedia)audChMedia).getMediaData());
				}
				else if (audChMedia is media.SequenceMedia)
				{
					audioMediaData.AddRange(getAudioMediaDataFromSequenceMedia((media.SequenceMedia)audChMedia));
				}
				if (audioMediaData.Count > 0)
				{
					curWr.WriteStartElement("par", SMIL_NS);
					if (audioMediaData.Count > 1) curWr.WriteStartElement("seq", SMIL_NS);
					foreach (media.data.audio.AudioMediaData amd in audioMediaData)
					{
						TimeDelta clipBegin = getElapsenInCurrentAudio();
						if (getCurrentPCMFormat() == null)
						{
							mPCMFormat = new urakawa.media.data.audio.PCMFormatInfo(amd.getPCMFormat());
						}
						else if (!getCurrentPCMFormat().ValueEquals(amd.getPCMFormat()))
						{
							throw new exception.InvalidDataFormatException(
								"Can not export since the PCM format differs within a single destination file");
						}
						copyData(amd.getAudioData(), getAudioStream(), (uint)amd.getPCMLength());
						TimeDelta clipEnd = getElapsenInCurrentAudio();
						curWr.WriteStartElement("audio", SMIL_NS);
						curWr.WriteAttributeString("src", getCurrentAudioFileName());
						curWr.WriteAttributeString("clipBegin", timeDeltaToSmilString(clipBegin));
						curWr.WriteAttributeString("clipEnd", timeDeltaToSmilString(clipEnd));
					}
					if (audioMediaData.Count > 1) curWr.WriteEndElement();
					curWr.WriteEndElement();
				}
			}
			return true;
		}

		public void postVisit(TreeNode node)
		{
			XmlWriter curWr = getSmilWriter();
			if (curWr!=null) curWr.WriteEndElement();
		}

		#endregion
	}
}
