using System;

namespace urakawa.xuk
{
	/// <summary>
	/// Summary description for Project.
	/// </summary>
	public class Project
	{
		urakawa.core.Presentation mPresentation;

		public Project()
		{
			
		}

		public bool openXUK(Uri fileUri)
		{
			mPresentation = new urakawa.core.Presentation();

			System.Xml.XmlTextReader source = new System.Xml.XmlTextReader(fileUri.ToString());
			source.WhitespaceHandling = System.Xml.WhitespaceHandling.Significant;

			//move to the Presentation element
			while (! (source.Name == "Presentation" && 
				source.NodeType == System.Xml.XmlNodeType.Element)
				&&
				source.EOF == false)
			{
				source.Read();
			}

			bool didItWork = mPresentation.XUKin(source);

			return didItWork;
		}

		public bool saveXUK(Uri fileUri)
		{
			//@todo
			//we should probably track the file encoding in the future
			System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(fileUri.LocalPath, System.Text.UnicodeEncoding.UTF8);

			writeBeginningOfFile(writer);
			writeFakeMetadata(writer);

			bool didItWork = false;
			
			if (mPresentation != null)
				didItWork = mPresentation.XUKout(writer);

			writeEndOfFile(writer);

			return didItWork;
		}

		public urakawa.core.Presentation getPresentation()
		{
			return mPresentation;
		}

		private void writeBeginningOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("XUK");
			writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", 
				"http://www.w3.org/2001/XMLSchema-instance", "xuk.xsd");
			
		}

		private void writeEndOfFile(System.Xml.XmlWriter writer)
		{
			writer.WriteEndElement();
			writer.WriteEndDocument();

			writer.Close();
		}

		private void writeFakeMetadata(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement("metaData");
			writer.WriteStartElement("key");
			writer.WriteAttributeString("value", "dc:Generator");
			writer.WriteAttributeString("name", "UrakawaToolkit build " + System.DateTime.Now.ToShortDateString());
			writer.WriteEndElement();
			writer.WriteEndElement();
		}
	}
}
