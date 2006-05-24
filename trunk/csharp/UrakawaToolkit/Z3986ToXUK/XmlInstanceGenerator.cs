using System;
using System.Xml;

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
    /// Processes <c>CoreNode</c>s with nested smilref attributes in instance document
    /// </summary>
    /// <param name="instanceDoc">The instance <see cref="XmlDocument"/></param>
    public void ProcessNestedSmilrefNodes(XmlDocument instanceDoc)
    {
      int count = 0;
      XmlNodeList smilrefNodes = instanceDoc.SelectNodes("//CoreNode/NonDistributedAudios[@smilref]");
      foreach (XmlNode nod in smilrefNodes)
      {
        count++;
        XmlElement NonDistributedAudios = (XmlElement)nod;
        XmlElement parentCoreNode = (XmlElement)NonDistributedAudios.ParentNode;
        FireProgress(String.Format(
          "Handling smilref {0} ({1:0}/{2:0})", 
          NonDistributedAudios.GetAttribute("smilref"), count, smilrefNodes.Count));
        XmlElement[] AUDIOMedias = XmlNodeListToXmlElementArray(
          NonDistributedAudios.SelectNodes(".//Media[@type='AUDIO']"));
        parentCoreNode.RemoveChild(NonDistributedAudios);
        XmlElement[] CoreNodes = XmlNodeListToXmlElementArray(parentCoreNode.SelectNodes("CoreNode"));
        int aoIndex = 0;
        int nIndex = 0;
        while (nIndex<CoreNodes.Length && aoIndex<AUDIOMedias.Length)
        {
          if (CoreNodes[nIndex].SelectNodes(".//Media[@type='AUDIO']").Count>0)
          {
            //Advance aoIndex to point at the first audio Media 
            //after the ones used by CoreNodes[nIndex] and descendants
            while (aoIndex<AUDIOMedias.Length)
            {
              bool used = false;
              foreach (XmlNode AMNod in CoreNodes[nIndex].SelectNodes(".//Media[@type='AUDIO'][@id]"))
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
              XmlNode AMNod = CoreNodes[nSubIndex].SelectSingleNode(".//Media[@type='AUDIO']");
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
                "mProperties/ChannelsProperty").AppendChild(instanceDoc.CreateElement("ChannelMapping"));
            }
            else
            {
              //Wrap CoreNodes[nIndex]-CoreNodes[nSubIndex] in an new created recipientNode
              recipientCoreNode = instanceDoc.CreateElement("CoreNode");
              recipientChannelMapping = instanceDoc.CreateElement("ChannelMapping");
              XmlNode temp = recipientCoreNode.AppendChild(instanceDoc.CreateElement("mProperties"));
              temp = temp.AppendChild(instanceDoc.CreateElement("ChannelsProperty"));
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
              XmlElement SequenceMedia = instanceDoc.CreateElement("SequenceMedia");
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
      foreach (XmlNode mediaNod in instanceDoc.SelectNodes(".//Media[@id]"))
      {
        XmlElement media = (XmlElement)mediaNod;
        media.Attributes.RemoveNamedItem("id");
      }
    }


    /// <summary>
    /// Generates the instance xml document
    /// </summary>
    /// <returns>The instance <see cref="XmlDocument"/></returns>
    public XmlDocument GenerateInstanceXml(bool ProcessNestedSmilrefs)
    {
      if (!System.IO.File.Exists(DTBOOKPath))
      {
        throw new ApplicationException(String.Format(
          "Could not find input dtbook file {0}", DTBOOKPath));
      }
      System.IO.Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(DTBOOKPath));
      XslTransform trans = new XslTransform(XSLT_PATH, DTBOOKPath);
      XmlDocument instanceDoc = new XmlDocument();
      //string dtbookDir = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(DTBOOKPath));
      try
      {
        instanceDoc.LoadXml(trans.Transform());
      }
      catch (XmlException e)
      {
        throw new ApplicationException(
          String.Format("Could not perform XSLT transform: {0}", e.Message),
          e);
      }
      FireProgress("Generated instance document");
      if (ProcessNestedSmilrefs)
      {
        ProcessNestedSmilrefNodes(instanceDoc);
        FireProgress("Processed nested smilrefs in instance document");
      }
      return instanceDoc;
    }
  }
}
