using System;
using System.IO;
using System.Text;
using System.Xml;
using urakawa.command;
using urakawa.progress;

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that serializes a xuk data stream from a <see cref="XukAble"/>
    ///</summary>
    public class SaveXukAction : ProgressAction
    {
        private Uri mDestUri;
        private Stream mDestStream;
        private XmlWriter mXmlWriter;
        private readonly IXukAble mSourceXukAble;
        private Project m_Project;

        private static Stream GetStreamFromUri(Uri src)
        {
            FileStream fs = new FileStream(src.LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);
            return fs;
        }

        private void initializeXmlWriter(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;

            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineChars = Environment.NewLine;

            if (!mSourceXukAble.IsPrettyFormat())
            {
                settings.Indent = false;
                settings.NewLineOnAttributes = false;
            }
            else
            {
                settings.Indent = true;
                settings.IndentChars = "\t";
                settings.NewLineOnAttributes = true;
            }
            mXmlWriter = XmlWriter.Create(stream, settings);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (can be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="writer">The destination <see cref="XmlWriter"/> (cannot be null)</param>
        public SaveXukAction(Project proj, IXukAble xukAble, Uri destUri, XmlWriter writer)
        {
            m_Project = proj;
            if (writer == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination Writer of the SaveXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The source XukAble of the SaveXukAction cannot be null");
            mDestUri = destUri;
            mSourceXukAble = xukAble;
            mXmlWriter = writer;
            XmlTextWriter txtWriter = writer as XmlTextWriter;
            if (txtWriter != null)
            {
                mDestStream = txtWriter.BaseStream;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (can be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="destStream">The destination <see cref="Stream"/> (cannot be null)</param>
        public SaveXukAction(Project proj, IXukAble xukAble, Uri destUri, Stream destStream)
        {
            m_Project = proj;
            if (destStream == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination Stream of the SaveXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The source XukAble of the SaveXukAction cannot be null");
            mDestUri = destUri;
            mSourceXukAble = xukAble;
            mDestStream = destStream;
            initializeXmlWriter(mDestStream);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (cannot be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/>(cannot be null)</param>
        public SaveXukAction(Project proj, IXukAble xukAble, Uri destUri)
        {
            m_Project = proj;
            if (destUri == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination URI of the SaveXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The source XukAble of the SaveXukAction cannot be null");

            mDestUri = destUri;
            mSourceXukAble = xukAble;
            mDestStream = GetStreamFromUri(mDestUri);
            initializeXmlWriter(mDestStream);
        }

        private void closeOutput()
        {
            mXmlWriter.Close();
            mXmlWriter = null;
            mDestStream.Close();
            mDestStream.Dispose();
            mDestStream = null;
        }

        #region Overrides of ProgressAction

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected override void GetCurrentProgress(out long cur, out long tot)
        {
            if (mDestStream != null)
            {
                // TODO: these progress values are always equal, as the stream is being created !!
                //mSourceXukAble.NumberOfElements ??
                cur = mDestStream.Position;
                tot = mDestStream.Length;
            }
            else
            {
                cur = 0;
                tot = 0;
            }
        }

        /// <summary>
        /// Gets a <c>bool</c> indicating if the <see cref="IAction"/> can execute
        /// </summary>
        /// <returns>The <c>bool</c></returns>
        public override bool CanExecute
        {
            get { return mXmlWriter != null; }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void Execute()
        {
            mHasCancelBeenRequested = false;
            Progress += SaveXukAction_progress;
            bool canceled = false;
            try
            {
                mXmlWriter.WriteStartDocument();
                mXmlWriter.WriteStartElement(XukStrings.Xuk, m_Project.XukNamespaceUri);
                if (XukAble.XUK_XSD_PATH != String.Empty)
                {
                    if (m_Project.XukNamespaceUri == String.Empty)
                    {
                        mXmlWriter.WriteAttributeString(
                            "xsi", "noNamespaceSchemaLocation",
                            "http://www.w3.org/2001/XMLSchema-instance",
                            XukAble.XUK_XSD_PATH);
                    }
                    else
                    {
                        mXmlWriter.WriteAttributeString(
                            "xsi",
                            "noNamespaceSchemaLocation",
                            "http://www.w3.org/2001/XMLSchema-instance",
                            String.Format("{0} {1}", m_Project.XukNamespaceUri + (m_Project.XukNamespaceUri.EndsWith("/") ? "" : "/"),
                                          XukAble.XUK_XSD_PATH));
                    }
                }
                mSourceXukAble.XukOut(mXmlWriter, mDestUri, this);
                mXmlWriter.WriteEndElement();
                mXmlWriter.WriteEndDocument();
            }
            catch (exception.ProgressCancelledException)
            {
                canceled = true;
            }
            finally
            {
                Progress -= SaveXukAction_progress;

                closeOutput();

                if (canceled) NotifyCancelled();
                else NotifyFinished();
            }
        }

        private void SaveXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        {
            if (mHasCancelBeenRequested) e.Cancel();
        }

        private string m_ShortDescription = "Serializing XUK...";
        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string ShortDescription
        {
            get { return m_ShortDescription; }
            set { m_ShortDescription = value; }
        }

        private string m_LongDescription = "Serializing the Urakawa SDK data model into a XUK XML file...";
        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public override string LongDescription
        {
            get { return m_LongDescription; }
            set { m_LongDescription = value; }
        }

        #endregion
    }
}