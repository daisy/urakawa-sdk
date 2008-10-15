using System;
using System.IO;
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

        private static Stream GetStreamFromUri(Uri src)
        {
            FileStream fs = new FileStream(src.LocalPath, FileMode.Create, FileAccess.Write);
            return fs;
        }

        private void initializeXmlWriter(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = " ";
            mXmlWriter = XmlWriter.Create(stream, settings);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (can be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="writer">The destination <see cref="XmlWriter"/> (cannot be null)</param>
        public SaveXukAction(IXukAble xukAble, Uri destUri, XmlWriter writer)
        {
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
        public SaveXukAction(IXukAble xukAble, Uri destUri, Stream destStream)
        {
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
        public SaveXukAction(IXukAble xukAble, Uri destUri)
        {
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
            mDestStream = null;
        }

        #region Overrides of ProgressAction

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected override void getCurrentProgress(out long cur, out long tot)
        {
            if (mDestStream!=null)
            {
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
        public override bool canExecute()
        {
            return mXmlWriter != null;
        }


        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public override string getLongDescription()
        {
            return "Serializes a XUK fragment";
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void execute()
        {
            mHasCancelBeenRequested = false;
            progress += SaveXukAction_progress;

            bool canceled = false;
            try
            {
// ReSharper disable PossibleNullReferenceException
                    mXmlWriter.WriteStartDocument();
// ReSharper restore PossibleNullReferenceException
                    mXmlWriter.WriteStartElement("Xuk", urakawa.ToolkitSettings.XUK_NS);
                    if (urakawa.ToolkitSettings.XUK_XSD_PATH != String.Empty)
                    {
                        if (urakawa.ToolkitSettings.XUK_NS == String.Empty)
                        {
                            mXmlWriter.WriteAttributeString(
                                "xsi", "noNamespaceSchemaLocation",
                                "http://www.w3.org/2001/XMLSchema-instance",
                                urakawa.ToolkitSettings.XUK_XSD_PATH);
                        }
                        else
                        {
                            mXmlWriter.WriteAttributeString(
                                "xsi",
                                "noNamespaceSchemaLocation",
                                "http://www.w3.org/2001/XMLSchema-instance",
                                String.Format("{0} {1}", urakawa.ToolkitSettings.XUK_NS, urakawa.ToolkitSettings.XUK_XSD_PATH));
                        }
                    }
                    mSourceXukAble.xukOut(mXmlWriter, mDestUri, this);
                    mXmlWriter.WriteEndElement();
                    mXmlWriter.WriteEndDocument();
            }
            catch (exception.ProgressCancelledException)
            {
                canceled = true;
            }
            finally
            {
                progress -= SaveXukAction_progress;

                closeOutput();

                if (canceled) notifyCancelled();
                else notifyFinished();
            }

        }

        private void SaveXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        {
            if (mHasCancelBeenRequested) e.Cancel();
        }

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string getShortDescription()
        {
            return "Save Xuk";
        }

        #endregion
    }
}
