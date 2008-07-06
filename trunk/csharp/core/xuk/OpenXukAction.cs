using System;
using System.IO;
using System.Xml;
using urakawa.command;
using urakawa.events.command;
using urakawa.progress;

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that opens a xuk file and loads it into a <see cref="Project"/>
    ///</summary>
    public class OpenXukAction : ProgressAction
    {
        /// <summary>
        /// Constructor explicitly setting the source <see cref="XmlReader"/> and the destination <see cref="Project"/>
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source file</param>
        /// <param name="destProj"></param>
        /// <param name="sourceStream">The source <see cref="Stream"/></param>
        public OpenXukAction(Uri sourceUri, Project destProj, Stream sourceStream)
        {
            if (sourceStream == null)
                throw new exception.MethodParameterIsNullException(
                    "The source Stream of the OpenXukAction cannot be null");
            if (destProj == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination Project of the OpenXukAction cannot be null");
            mSourceUri = sourceUri;
            mDestProject = destProj;
            mDestStream = sourceStream;
        }

        private static Stream getStreamFromUri(Uri src)
        {
            if (src == null) throw new exception.MethodParameterIsNullException("The Uri source is null");
            return new FileStream(src.LocalPath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Constructor explicitly setting the source of the read and the destination <see cref="Project"/>
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source file</param>
        /// <param name="destProj"></param>
        public OpenXukAction(Uri sourceUri, Project destProj)
            : this(sourceUri, destProj, getStreamFromUri(sourceUri))
        {
        }

        private Uri mSourceUri;
        private Stream mDestStream;
        private readonly Project mDestProject;

        #region Overrides of ProgressAction

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected override void getCurrentProgress(out long cur, out long tot)
        {
            if (mDestStream != null)
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
            get { return "Opens a xuk project file"; }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void Execute()
        {
            mHasCancelBeenRequested = false;
            Progress += OpenXukAction_progress;
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = false;
                XmlReader rd = XmlReader.Create(mDestStream, settings, mSourceUri.ToString());
                try
                {
                    if (!rd.ReadToFollowing("Xuk", urakawa.ToolkitSettings.XUK_NS))
                    {
                        throw new exception.XukException("Could not find Xuk element in Project Xuk file");
                    }
                    bool foundProject = false;
                    if (!rd.IsEmptyElement)
                    {
                        while (rd.Read())
                        {
                            if (rd.NodeType == XmlNodeType.Element)
                            {
                                //If the element QName matches the Xuk QName equivalent of this, Xuk it in using this.XukIn
                                if (rd.LocalName == mDestProject.XukLocalName &&
                                    rd.NamespaceURI == mDestProject.XukNamespaceUri)
                                {
                                    foundProject = true;
                                    mDestProject.XukIn(rd, this);
                                }
                                else if (!rd.IsEmptyElement)
                                {
                                    rd.ReadSubtree().Close();
                                }
                            }
                            else if (rd.NodeType == XmlNodeType.EndElement)
                            {
                                break;
                            }

                            if (rd.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                        }
                    }
                    if (!foundProject)
                    {
                        throw new exception.XukException("Found no Project in Xuk file");
                    }
                }
                finally
                {
                    rd.Close();
                }
                NotifyFinished();
            }
            catch (exception.ProgressCancelledException)
            {
                NotifyCancelled();
            }
            finally
            {
                Progress -= OpenXukAction_progress;
            }
        }

        private void OpenXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        {
            if (mHasCancelBeenRequested) e.Cancel();
        }

        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string ShortDescription
        {
            get { return "Open Xuk"; }
        }

        #endregion
    }
}