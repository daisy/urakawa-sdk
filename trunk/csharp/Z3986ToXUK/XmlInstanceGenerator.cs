using System;
using System.Xml;
using System.Xml.Xsl;

namespace Z3986ToXUK
{
  /// <summary>
  /// Event argument for progress events
  /// </summary>
  public class ProgressEventArgs : EventArgs
  {
    /// <summary>
    /// A <see cref="bool"/> indicating if the progress was cancelled during the event call
    /// </summary>
    /// <remarks>If one wants to cancel the process monitored by the event, 
    /// <see cref="Cancel"/> should be set to <c>true</c></remarks>
    public bool Cancel;

    /// <summary>
    /// The progress message 
    /// </summary>
    public string Message;

    /// <summary>
    /// Default constructor 
    /// </summary>
    public ProgressEventArgs(string msg) : base()
    {
      Message = msg;
      Cancel = false;
    }
  }

  /// <summary>
  /// Delegate for progress events
  /// </summary>
  public delegate void XmlInstanceGeneratorProgressEventDelegate(XmlInstanceGenerator o, ProgressEventArgs e);

	/// <summary>
	/// Provides functionality for generating an Urakawa instance xml document based on a Z39.86 DTB.
	/// </summary>
	/// <remarks>
	/// Currently only full text Z39.86 DTBs are supported
	/// </remarks>
	public class XmlInstanceGenerator
	{
		public static string XUK_NS = "http://www.daisy.org/urakawa/xuk/0.5";

    /// <summary>
    /// Event fired while generating the instance document, showing the progress of the generation
    /// </summary>
    public event XmlInstanceGeneratorProgressEventDelegate Progress;

    private bool FireProgress(string Message)
    {
      ProgressEventArgs e = new ProgressEventArgs(Message);
      if (Progress!=null) Progress(this, e);
      return e.Cancel;
    }

    /// <summary>
    /// The path of the XSLT doing the dtbook2InstanceXml transform
    /// </summary>
    public static string XSLT_PATH = System.IO.Path.Combine(
      System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
      "dtbookToXUK.xsl");

    private string mDTBOOKPath;

    /// <summary>
    /// The path of the dtbook file of the DTB
    /// </summary>
    public string DTBOOKPath
    {
      get
      {
        return mDTBOOKPath;
      }
    }

    /// <summary>
    /// Public constructor creating a generator based on a single DTBOOK file
    /// </summary>
    /// <param name="dtbook">The path of the DTBOOK file</param>
		public XmlInstanceGenerator(string dtbook)
		{
      Console.WriteLine("XSLT Path: {0}", XSLT_PATH);
      mDTBOOKPath = dtbook;
		}

    private static XmlElement[] XmlNodeListToXmlElementArray(XmlNodeList nodes)
    {
      XmlElement[] res = new XmlElement[nodes.Count];
      for (int i=0; i<nodes.Count; i++)
      {
        res[i] = (XmlElement)nodes[i];
      }
      return res;
    }

    /// <summary>
    /// Processes the clip time values, converting them from Z39.86 format to XUK format.
    /// </summary>
    /// <param name="instanceDoc">The instance <see cref="XmlDocument"/></param>
    public void ProcessClipValues(XmlDocument instanceDoc)
    {
			XmlNamespaceManager xukNsmgr = new XmlNamespaceManager(instanceDoc.NameTable);
			xukNsmgr.AddNamespace("xuk", XUK_NS);
      XmlNodeList clipAttributes = instanceDoc.SelectNodes("//xuk:AudioMedia|//xuk:VideoMedia", xukNsmgr);
      int count = 0;
      int nextFireCount = 0;
      string[] names = new string[] {"clipBegin", "clipEnd"};
      foreach (XmlNode nod in clipAttributes)
      {
        count++;
        if (count>nextFireCount)
        {
          FireProgress(String.Format(
            "Processing time clip values of Media {0:0}%", 
            (100*count)/clipAttributes.Count));
          nextFireCount += clipAttributes.Count/10;
        }
        XmlElement Media = (XmlElement)nod;
				XmlNode attrNode = Media.GetAttributeNode("id");
				if (Media.HasAttribute("id")) Media.RemoveAttribute("id");
        foreach (string clipAttrName in names)
        {
          string value = Media.GetAttribute(clipAttrName);
          if (value!=null && value!="")
          {
            TimeSpan timeVal = TimeSpanFromZ3986TimeValue(value);
            if (timeVal>=TimeSpan.Zero)
            {
              Media.SetAttribute(clipAttrName, timeVal.ToString());
            }
          }
        }
      }
    }

    private static TimeSpan TimeSpanFromTimeCountValue(string value)
    {
      try
      {
        long factor = TimeSpan.TicksPerSecond;
        if (value.EndsWith("h"))
        {
          value = value.Substring(0, value.Length-1);
          factor = TimeSpan.TicksPerHour;
        }
        else if (value.EndsWith("min"))
        {
          value = value.Substring(0, value.Length-3);
          factor = TimeSpan.TicksPerMinute;
        }
        else if (value.EndsWith("s"))
        {
          value = value.Substring(0, value.Length-1);
          factor = TimeSpan.TicksPerSecond;
        }
        else if (value.EndsWith("ms"))
        {
          value = value.Substring(0, value.Length-2);
          factor = TimeSpan.TicksPerMillisecond;
        }
        return new TimeSpan((long)(Double.Parse(value)*factor));
      }
      catch (Exception)
      {
        Console.WriteLine("Invalid Z39.86 timecount value '{0}'", value); 
        return TimeSpan.MinValue;
      }
    }

    private static TimeSpan TimeSpanFromZ3986TimeValue(string value)
    {
      if (value.StartsWith("npt=")) value = value.Substring(4);
      string[] parts = value.Split(':');
      long hours = 0;
      long mins;
      double secs;
      try
      {
        switch (parts.Length)
        {
          case 1:
            return TimeSpanFromTimeCountValue(parts[0]);
          case 2:
            mins = Int64.Parse(parts[0]);
            secs = Double.Parse(parts[1]);
            break;
          case 3:
            hours = Int64.Parse(parts[0]);
            mins = Int64.Parse(parts[1]);
            secs = Double.Parse(parts[2]);
            break;
          default:
            Console.WriteLine("Invalid Z39.86 time value '{0}'", value);
            return TimeSpan.MinValue;
        }
        if (hours<0 || mins<0 || secs<0)
        {
          Console.WriteLine("Invalid Z39.86 time value '{0}'", value); 
          return TimeSpan.MinValue;
        }
        long ticks = (hours*TimeSpan.TicksPerHour)+(mins*TimeSpan.TicksPerMinute)+(long)(secs*TimeSpan.TicksPerSecond);
        return new TimeSpan(ticks);
      }
      catch (Exception)
      {
        Console.WriteLine("Invalid Z39.86 time value '{0}'", value); 
        return TimeSpan.MinValue;
      }
    }

    public void ProcessSmilrefNodes(XmlDocument instanceDoc, string baseUri, bool WriteInterrim)
    {
      System.Collections.Hashtable smilfiles = new System.Collections.Hashtable();
			XmlNamespaceManager xukNsmgr = new XmlNamespaceManager(instanceDoc.NameTable);
			xukNsmgr.AddNamespace("xuk", XUK_NS);
      XmlNodeList cnNodes = instanceDoc.SelectNodes(
				"//xuk:CoreNode[xuk:mProperties/xuk:XmlProperty/xuk:XmlAttribute[@name='smilref']]",
				xukNsmgr);
      int count = 0;
      int nextFireCount = 0;
      foreach (XmlNode nod in cnNodes)
      {
        count++;
        if (count>nextFireCount)
        {
          FireProgress(String.Format(
            "Handling smilrefs {0:0}%",
            (100*count)/cnNodes.Count));
          nextFireCount += cnNodes.Count/10;
        }
        XmlElement coreNode = (XmlElement)nod;
        XmlElement smilrefAttr = (XmlElement)coreNode.SelectSingleNode(
          "xuk:mProperties/xuk:XmlProperty/xuk:XmlAttribute[@name='smilref']", 
					xukNsmgr);
        string[] temps = smilrefAttr.InnerText.Split('#');
        if (temps.Length==2)
        {
          string id = "";
          XmlNode tempNode = coreNode.SelectSingleNode(
            "xuk:mProperties/xuk:XmlProperty/xuk:XmlAttribute[@name='id']",
						xukNsmgr);
          if (tempNode!=null) id = tempNode.InnerText.Trim();
          if (id.IndexOfAny(new char[] {'\'', '\"'})!=-1)
          {
            Console.WriteLine("Invalid id attribute value {0}", id);
          }
          string smilPath = temps[0];
          string smilAnchor = temps[1];
          if (smilAnchor.IndexOfAny(new char[] {'\'', '\"'})!=-1)
          {
            Console.WriteLine("Invalid smil anchor {0}", smilAnchor);
            continue;
          }
          XmlDocument smilDoc;
          if (smilfiles.ContainsKey(smilPath))
          {
            smilDoc = (XmlDocument)smilfiles[smilPath];
          }
          else
          {
            smilDoc = new XmlDocument();
            try
            {
              smilDoc.Load(System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(baseUri),
                smilPath));
            }
            catch (Exception e)
            {
              Console.WriteLine("Could not load smilfile {0}: {1}", smilPath, e.Message);
              continue;
            }
            smilfiles.Add(smilPath, smilDoc);
          }
          string smilPrefix = "";
          XmlNamespaceManager nsmgr = new XmlNamespaceManager(smilDoc.NameTable);
          if (smilDoc.DocumentElement.NamespaceURI!="")
          {
            smilPrefix = "smil:";
            nsmgr.AddNamespace("smil", smilDoc.DocumentElement.NamespaceURI);
          }
          tempNode = (XmlElement)smilDoc.GetElementById(smilAnchor);
          if (tempNode==null)
          {
            Console.WriteLine("Could not find smil time container {0}", smilrefAttr.
InnerText);
						continue;
          }
          XmlElement targetTimeContainer = (XmlElement)tempNode;
          string textLinkBackSrc = "";
          string xpath = String.Format(
            "descendant-or-self::{0}text[substring-after(@src, '#')='{2}']/@src",
            smilPrefix, smilAnchor, id);
          tempNode = targetTimeContainer.SelectSingleNode(xpath, nsmgr);
          if (tempNode!=null)
          {
            textLinkBackSrc = tempNode.Value.Trim();
          }
          if (textLinkBackSrc=="")
          {
            Console.WriteLine("No text linking back found for smilref {0}", smilrefAttr.Value);
            continue;
          }
          else if (textLinkBackSrc.IndexOfAny(new char[] {'\'', '\"'})!=-1)
          {
            Console.WriteLine("Invalid src attribute of text linking back: {0}", textLinkBackSrc);
            continue;
          }
          xpath = String.Format(".//{0}par[{0}text[@src='{1}']]", smilPrefix, textLinkBackSrc);
          XmlNodeList smilTimeContainers = targetTimeContainer.ParentNode.SelectNodes(xpath, nsmgr);
          if (smilTimeContainers.Count>1)
          {
            xpath = String.Format(
              ".//{0}par[{0}text[@src='{1}']] "
              +"| .//{0}par[(preceding::{0}text[@src='{1}']) and (following::{0}text[@src='{1}'])]",
              smilPrefix, textLinkBackSrc);
            smilTimeContainers = targetTimeContainer.ParentNode.SelectNodes(xpath, nsmgr);
          }
          XmlElement audioChannelMapping = instanceDoc.CreateElement("ChannelMapping", XUK_NS);
          XmlElement[] audioMedias = new XmlElement[0];
          audioChannelMapping.SetAttribute("channel", "audioChannel");
          xpath = String.Format(".//{0}audio", smilPrefix);
          for (int tcNo=0; tcNo<smilTimeContainers.Count; tcNo++)
          {
            XmlNodeList sans = smilTimeContainers[tcNo].SelectNodes(xpath, nsmgr);
            if (sans.Count>0)
            {
              XmlElement[] tempAM = audioMedias;
              audioMedias = new XmlElement[tempAM.Length+sans.Count];
              tempAM.CopyTo(audioMedias, 0);
              for (int i=0; i<sans.Count; i++)
              {
                XmlElement smilAudioElem = (XmlElement)sans[i];
                XmlElement audioMedia = instanceDoc.CreateElement("Media");
                audioMedia = instanceDoc.CreateElement("AudioMedia", XUK_NS);
                audioMedia.SetAttribute("clipBegin", smilAudioElem.GetAttribute("clipBegin"));
                audioMedia.SetAttribute("clipEnd", smilAudioElem.GetAttribute("clipEnd"));
                audioMedia.SetAttribute("id", smilAudioElem.GetAttribute("id"));
                audioMedia.SetAttribute("src", smilAudioElem.GetAttribute("src"));
                audioMedias[i+tempAM.Length] = audioMedia;
              }
            }
          }
          switch (audioMedias.Length)
          {
            case 0:
              break;
            case 1:
              audioChannelMapping.AppendChild(audioMedias[0]);
              break;
            default:
              XmlElement seqMedia = instanceDoc.CreateElement("SequenceMedia", XUK_NS);
              foreach (XmlElement am in audioMedias)
              {
                seqMedia.AppendChild(am);
              }
              audioChannelMapping.AppendChild(seqMedia);
              break;
          }
          if (audioMedias.Length>0)
          {
            if (coreNode.SelectNodes("CoreNode[.//XmlAttribute[@name='smilref']]").Count>0)
            {
              XmlElement NonDistributedAudios = instanceDoc.CreateElement("NonDistributedAudios", XUK_NS);
              NonDistributedAudios.SetAttribute("smilref", smilrefAttr.Value);
              NonDistributedAudios.AppendChild(audioChannelMapping);
              coreNode.AppendChild(NonDistributedAudios);
            }
            else
            {
              coreNode.SelectSingleNode("xuk:mProperties/xuk:ChannelsProperty", xukNsmgr).AppendChild(audioChannelMapping);
            }
            smilrefAttr.ParentNode.RemoveChild(smilrefAttr);
          }
        }

      }
      if (WriteInterrim)
      {
        string interrimPath = 
          System.IO.Path.Combine(
          System.IO.Path.GetDirectoryName(DTBOOKPath),
          System.IO.Path.GetFileNameWithoutExtension(DTBOOKPath))
          +".interrim.xuk";
        try
        {
          XmlTextWriter wr = new XmlTextWriter(
            interrimPath, System.Text.Encoding.UTF8);
          wr.Indentation = 1;
          wr.Formatting = Formatting.Indented;
          instanceDoc.WriteTo(wr);
          wr.Close();
        }
        catch (Exception e)
        {
          throw new ApplicationException(
            String.Format("Could not write interrim XUK: {0}", e.Message),
            e);
        }

      }
      ProcessNestedSmilrefNodes(instanceDoc);
      foreach (XmlNode mediaNod in instanceDoc.SelectNodes(".//Media[@id]"))
      {
        XmlElement media = (XmlElement)mediaNod;
        media.Attributes.RemoveNamedItem("id");
      }
    }


    /// <summary>
    /// Processes <c>CoreNode</c>s with nested smilref attributes in instance document
    /// </summary>
    /// <param name="instanceDoc">The instance <see cref="XmlDocument"/></param>
    public void ProcessNestedSmilrefNodes(XmlDocument instanceDoc)
    {
			XmlNamespaceManager xukNsmgr = new XmlNamespaceManager(instanceDoc.NameTable);
			xukNsmgr.AddNamespace("xuk", XUK_NS);
			int count = 0;
      XmlNodeList smilrefNodes = instanceDoc.SelectNodes("//xuk:CoreNode/xuk:NonDistributedAudios[@smilref]", xukNsmgr);
      foreach (XmlNode nod in smilrefNodes)
      {
        count++;
        XmlElement NonDistributedAudios = (XmlElement)nod;
        XmlElement parentCoreNode = (XmlElement)NonDistributedAudios.ParentNode;
        FireProgress(String.Format(
          "Handling nested smilref {0} ({1:0}/{2:0})", 
          NonDistributedAudios.GetAttribute("smilref"), count, smilrefNodes.Count));
        XmlElement[] AUDIOMedias = XmlNodeListToXmlElementArray(
          NonDistributedAudios.SelectNodes(".//xuk:AudioMedia", xukNsmgr));
        parentCoreNode.RemoveChild(NonDistributedAudios);
        XmlElement[] CoreNodes = XmlNodeListToXmlElementArray(parentCoreNode.SelectNodes("xuk:CoreNode", xukNsmgr));
        int aoIndex = 0;
        int nIndex = 0;
        while (nIndex<CoreNodes.Length && aoIndex<AUDIOMedias.Length)
        {
					if (CoreNodes[nIndex].SelectNodes(".//xuk:AudioMedia", xukNsmgr).Count > 0)
          {
            //Advance aoIndex to point at the first audio Media 
            //after the ones used by CoreNodes[nIndex] and descendants
            while (aoIndex<AUDIOMedias.Length)
            {
              bool used = false;
							foreach (XmlNode AMNod in CoreNodes[nIndex].SelectNodes(".//xuk:AudioMedia[@id]", xukNsmgr))
              {
                XmlElement AMElem = (XmlElement)AMNod;
                if (AUDIOMedias[aoIndex].GetAttribute("id")==AMElem.GetAttribute("id"))
                {
                  AMElem.SetAttribute("id", "");
                  used = true;
                  break;
                }
              }
              if (!used) break;
              aoIndex++;
            }
            nIndex++;
          }
          else
          {
            int nSubIndex = nIndex;
            XmlElement nextDescendantAO = null;
            //advance nSubIndex to point at the next CoreNode with a descendant audio Media
            while (nSubIndex<CoreNodes.Length)
            {
							XmlNode AMNod = CoreNodes[nSubIndex].SelectSingleNode(".//xuk:AudioMedia", xukNsmgr);
              if (AMNod!=null) 
              {
                nextDescendantAO = (XmlElement)AMNod;
                break;
              }
              nSubIndex++;
            }
            //The recipient CoreNode for audio Media. 
            //Can be an existing child of parentCoreNode or an inserted wrapper CoreNode
            XmlElement recipientCoreNode, recipientChannelMapping;
            if (nSubIndex==(nIndex+1))
            {
              //Only a single Node, no need to wrap
              recipientCoreNode = CoreNodes[nIndex];
              recipientChannelMapping = (XmlElement)recipientCoreNode.SelectSingleNode(
                "xuk:mProperties/xuk:ChannelsProperty", xukNsmgr).AppendChild(instanceDoc.CreateElement("ChannelMapping", XUK_NS));
            }
            else
            {
              //Wrap CoreNodes[nIndex]-CoreNodes[nSubIndex] in an new created recipientNode
              recipientCoreNode = instanceDoc.CreateElement("CoreNode", XUK_NS);
							recipientChannelMapping = instanceDoc.CreateElement("ChannelMapping", XUK_NS);
							XmlNode temp = recipientCoreNode.AppendChild(instanceDoc.CreateElement("mProperties", XUK_NS));
							temp = temp.AppendChild(instanceDoc.CreateElement("ChannelsProperty", XUK_NS));
              temp.AppendChild(recipientChannelMapping);
              parentCoreNode.InsertBefore(recipientCoreNode, CoreNodes[nIndex]);
              for (int i=nIndex; i<nSubIndex; i++)
              {
                recipientCoreNode.AppendChild(parentCoreNode.RemoveChild(CoreNodes[i]));
              }
            }
            recipientChannelMapping.SetAttribute("channel", "audioChannel");
            nIndex = nSubIndex;
            int aoSubIndex = aoIndex;
            //Advance aoSubIndex to point immeadtly after the last AudioObject to add to recipientNode
            if (nextDescendantAO==null) aoSubIndex=AUDIOMedias.Length;
            while (aoSubIndex<AUDIOMedias.Length)
            {
              if (AUDIOMedias[aoSubIndex].GetAttribute("id")==nextDescendantAO.GetAttribute("id"))
              {
                break;
              }
              aoSubIndex++;
            }
            if (aoSubIndex==(aoIndex+1))
            {
              recipientChannelMapping.AppendChild(AUDIOMedias[aoIndex]);
            }
            else
            {
							XmlElement SequenceMedia = instanceDoc.CreateElement("SequenceMedia", XUK_NS);
              SequenceMedia.SetAttribute("type", "AUDIO");
              for (int i=aoIndex; i<aoSubIndex; i++)
              {
                SequenceMedia.AppendChild(AUDIOMedias[i]);
              }
              recipientChannelMapping.AppendChild(SequenceMedia);
            }
            aoIndex = aoSubIndex;
          }
        }
      }
    }


    /// <summary>
    /// Generates the instance xml document
    /// </summary>
    /// <param name="ProcessSmilrefs">A <see cref="bool"/> indicating if smilrefs attrubutes should be 
    /// processed</param>
    /// <returns>The instance <see cref="XmlDocument"/></returns>
    public XmlDocument GenerateInstanceXml(bool ProcessSmilrefs, bool WriteInterrim)
    {
      if (!System.IO.File.Exists(DTBOOKPath))
      {
        throw new ApplicationException(String.Format(
          "Could not find input dtbook file {0}", DTBOOKPath));
      }
      System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(DTBOOKPath));
			XslCompiledTransform trans = new XslCompiledTransform();
			XsltSettings setXsl = new XsltSettings();
			setXsl.EnableDocumentFunction = true;
			trans.Load(XSLT_PATH, setXsl, null);
      XmlDocument instanceDoc = new XmlDocument();
      try
      {
				System.IO.MemoryStream memStream = new System.IO.MemoryStream();
				XmlWriterSettings setWr = new XmlWriterSettings();
				setWr.CloseOutput = false;
				XmlWriter wr = XmlWriter.Create(memStream, setWr);
				XmlReaderSettings setRd = new XmlReaderSettings();
				setRd.ProhibitDtd = false;
				setRd.IgnoreWhitespace = true;
				XmlReader rd = XmlReader.Create(DTBOOKPath, setRd);
				trans.Transform(rd, wr);
				wr.Close();
				rd.Close();
				memStream.Position = 0;
				rd = XmlReader.Create(memStream);
				instanceDoc.Load(rd);
				wr.Close();
      }
      catch (XmlException e)
      {
        throw new ApplicationException(
          String.Format("Could not perform XSLT transform: {0}", e.Message),
          e);
      }
      FireProgress("Generated instance document");
      if (ProcessSmilrefs)
      {
        ProcessSmilrefNodes(instanceDoc, DTBOOKPath, WriteInterrim);
        FireProgress("Processed nested smilrefs in instance document");
				ProcessClipValues(instanceDoc);
				FireProgress("Processed time clip values");
      }

      return instanceDoc;
    }
  }
}
