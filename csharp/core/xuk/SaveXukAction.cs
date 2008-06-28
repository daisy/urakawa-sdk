using System;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.progress;

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that opens a xuk file and loads it into a <see cref="Project"/>
    ///</summary>
    public class SaveXukAction : ProgressAction
    {
        /// <summary>
        /// Constructor explicitly setting the source <see cref="XmlReader"/> and the destination <see cref="Project"/>
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the source file</param>
        /// <param name="sourceProj"></param>
        /// <param name="destStream">The source <see cref="Stream"/></param>
        public SaveXukAction(Uri destUri, Project sourceProj, Stream destStream)
        {
            if (destStream == null)
                throw new exception.MethodParameterIsNullException(
                    "The source Stream of the SaveXukAction cannot be null");
            if (sourceProj == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination Project of the SaveXukAction cannot be null");
            mDestUri = destUri;
            mSourceProject = sourceProj;
            mDestStream = destStream;
        }

        private static Stream getStreamFromUri(Uri src)
        {
            if (src==null) throw new exception.MethodParameterIsNullException("The Uri source is null");
            FileStream fs = new FileStream(src.LocalPath, FileMode.Create, FileAccess.Write);
            return fs;
        }

        /// <summary>
        /// Constructor explicitly setting the source of the read and the destination <see cref="Project"/>
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the source file</param>
        /// <param name="sourceProj"></param>
        public SaveXukAction(Uri destUri, Project sourceProj)
            : this(destUri, sourceProj, getStreamFromUri(destUri))
        {
            
        }

        private Uri mDestUri;
        private Stream mDestStream;
        private readonly Project mSourceProject;

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
        public override bool CanExecute
        {
            get { return true; }
        }

        /// <summary>
        /// Get a long uman-readable description of the command
        /// </summary>
        public override string LongDescription
        {
            get { return "Saves a xuk project file"; }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void Execute()
        {
            mHasCancelBeenRequested = false;
            progress += SaveXukAction_progress;
            try
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.IndentChars = " ";
                XmlWriter writer = XmlWriter.Create(mDestStream, settings);
                try
                {
// ReSharper disable PossibleNullReferenceException
                    writer.WriteStartDocument();
// ReSharper restore PossibleNullReferenceException
                    writer.WriteStartElement("Xuk", urakawa.ToolkitSettings.XUK_NS);
                    if (urakawa.ToolkitSettings.XUK_XSD_PATH != String.Empty)
                    {
                        if (urakawa.ToolkitSettings.XUK_NS == String.Empty)
                        {
                            writer.WriteAttributeString(
                                "xsi", "noNamespaceSchemaLocation",
                                "http://www.w3.org/2001/XMLSchema-instance",
                                urakawa.ToolkitSettings.XUK_XSD_PATH);
                        }
                        else
                        {
                            writer.WriteAttributeString(
                                "xsi",
                                "noNamespaceSchemaLocation",
                                "http://www.w3.org/2001/XMLSchema-instance",
                                String.Format("{0} {1}", urakawa.ToolkitSettings.XUK_NS, urakawa.ToolkitSettings.XUK_XSD_PATH));
                        }
                    }
                    mSourceProject.xukOut(writer, mDestUri, this);
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                finally
                {
                    writer.Close();
                }
                notifyFinished();
            }
            catch (exception.ProgressCancelledException)
            {
                notifyCancelled();
            }
            finally
            {
                progress -= SaveXukAction_progress;
            }

        }

        private void SaveXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        {
            if (mHasCancelBeenRequested) e.Cancel();
        }

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string ShortDescription
        {
            get { return "Save Xuk"; }
        }

        #endregion
    }
}
