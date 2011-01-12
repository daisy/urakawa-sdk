using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using urakawa.command;
using urakawa.events.progress;
using urakawa.progress;

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that serializes a xuk data stream from a <see cref="XukAble"/>
    ///</summary>
    public class SaveXukAction : ProgressAction
    {
        public static XmlWriterSettings GetDefaultXmlWriterConfiguration(bool pretty)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;

            settings.NewLineHandling = NewLineHandling.Replace;
            settings.NewLineChars = Environment.NewLine;

            if (!pretty)
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

            return settings;
        }

        public static void WriteXmlDocument(XmlDocument xmlDoc, string path)
        {
            const bool pretty = true;

            xmlDoc.PreserveWhitespace = false;
            xmlDoc.XmlResolver = null;

            XmlWriterSettings settings = GetDefaultXmlWriterConfiguration(pretty);

            using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
            {
                if (pretty && xmlWriter is XmlTextWriter)
                {
                    ((XmlTextWriter)xmlWriter).Formatting = Formatting.Indented;
                }

                try
                {
                    xmlDoc.Save(xmlWriter);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    // No message box: use debugging instead (inspect stack trace, watch values)
                    //MessageBox.Show(e.ToString());

                    // The Fail() method is better:
                    //System.Diagnostics.Debug.Fail(e.Message);

                    //Or you can explicitely break:
#if DEBUG
                    Debugger.Break();
#endif
                }
                finally
                {
                    xmlWriter.Close();
                }
            }
        }

        public override void DoWork()
        {
            Execute();
        }

        private Uri mDestUri;
        private Stream mDestStream;
        private XmlWriter mXmlWriter;
        private readonly IXukAble mSourceXukAble;
        private Project m_Project;


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

            XmlWriterSettings settings = GetDefaultXmlWriterConfiguration(mSourceXukAble.IsPrettyFormat());
            mXmlWriter = XmlWriter.Create(mDestStream, settings);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (cannot be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/>(cannot be null)</param>
        public SaveXukAction(Project proj, IXukAble xukAble, Uri destUri)
        {
            if (proj == null)
                throw new exception.MethodParameterIsNullException(
                    "The source Project of the SaveXukAction cannot be null");
            if (destUri == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination URI of the SaveXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The source XukAble of the SaveXukAction cannot be null");

            m_Project = proj;
            mDestUri = destUri;
            mSourceXukAble = xukAble;

            int currentPercentage = 0;
            /*
            EventHandler<ProgressEventArgs> progressing = (sender, e) =>
            {
                double val = e.Current;
                double max = e.Total;
                var percent = (int)((val / max) * 100);

                if (percent != currentPercentage)
                {
                    currentPercentage = percent;
                    reportProgress(currentPercentage, val + " / " + max);
                    //backWorker.ReportProgress(currentPercentage);
                }

                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };
             */
            //dotnet2
            EventHandler<ProgressEventArgs> progressing = delegate (object sender,ProgressEventArgs e) 
            {
                double val = e.Current;
                double max = e.Total;
                //var percent = (int)((val / max) * 100);
                int percent = (int)((val / max) * 100);

                if (percent != currentPercentage)
                {
                    currentPercentage = percent;
                    reportProgress(currentPercentage, val + " / " + max);
                    //backWorker.ReportProgress(currentPercentage);
                }

                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };
            Progress += progressing;
            //Finished += (sender, e) =>
            //{
                //Progress -= progressing;
            //};

            //dotnet2
            Finished += delegate (object sender,FinishedEventArgs e) 
            {
                Progress -= progressing;
            };
            //Cancelled += (sender, e) =>
            //{
                //Progress -= progressing;
            //};

            //dotnet2
            Cancelled += delegate (object sender,CancelledEventArgs e) 
            {
                Progress -= progressing;
            };

            mDestStream = new FileStream(mDestUri.LocalPath, FileMode.Create, FileAccess.Write, FileShare.None);

            XmlWriterSettings settings = GetDefaultXmlWriterConfiguration(mSourceXukAble.IsPrettyFormat());
            mXmlWriter = XmlWriter.Create(mDestStream, settings);
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
            //mHasCancelBeenRequested = false;
            //Progress += SaveXukAction_progress;
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
                            String.Format("{0}{1}",
                            m_Project.XukNamespaceUri
                            + (m_Project.XukNamespaceUri.EndsWith("/") ? "" : "/"),
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
                //Progress -= SaveXukAction_progress;

                closeOutput();

                if (canceled) NotifyCancelled();
                else NotifyFinished();
            }
        }

        //private void SaveXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        //{
        //    if (mHasCancelBeenRequested) e.Cancel();
        //}

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