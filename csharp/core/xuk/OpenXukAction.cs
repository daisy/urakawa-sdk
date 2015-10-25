using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Xml;
using AudioLib;
using urakawa.command;
using urakawa.events.progress;
using urakawa.ExternalFiles;
using urakawa.progress;

#if USE_ISOLATED_STORAGE
using System.IO.IsolatedStorage;
#endif //USE_ISOLATED_STORAGE

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that deserializes a xuk data stream into a <see cref="XukAble"/>
    ///</summary>
    public class OpenXukAction : ProgressAction
    {
        public const string XUK_EXTENSION = @".xuk";
        public const string XUK_SPINE_EXTENSION = @".xukspine";

        public override void DoWork()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Execute();
            stopWatch.Stop();
            Console.WriteLine(@"......XUK-in milliseconds: " + stopWatch.ElapsedMilliseconds);
        }

        private Uri mSourceUri;
        private Stream mSourceStream;
        private XmlReader mXmlReader;
        private readonly IXukAble mDestXukAble;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source (can be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="sourceStream">The source <see cref="Stream"/> (cannot be null)</param>
        //public OpenXukAction(IXukAble xukAble, Uri sourceUri, Stream sourceStream)
        //{
        //    if (sourceStream == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The source Stream of the OpenXukAction cannot be null");
        //    if (xukAble == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The destination IXukAble of the OpenXukAction cannot be null");
        //    mSourceUri = sourceUri;
        //    mDestXukAble = xukAble;
        //    mSourceStream = sourceStream;
        //    initializeXmlReader(mSourceStream);
        //}

        /// <summary>
        /// Constructor (DO NOT USE ! THE STREAM IS NULL. USE THE STREAM-BASED CTOR instead)
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source (can be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        /// <param name="reader">The source <see cref="XmlReader"/> (cannot be null)</param>
        //public OpenXukAction(IXukAble xukAble, Uri sourceUri, XmlReader reader)
        //{
        //    if (reader == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The source XmlReader of the OpenXukAction cannot be null");
        //    if (xukAble == null)
        //        throw new exception.MethodParameterIsNullException(
        //            "The destination IXukAble of the OpenXukAction cannot be null");
        //    mSourceUri = sourceUri;
        //    mDestXukAble = xukAble;
        //    mXmlReader = reader;
        //    XmlTextReader txtReader = reader as XmlTextReader;
        //    if (txtReader != null)
        //    {
        //        //TODO: where can we get the underlying stream ??
        //        //mSourceStream = txtReader.BaseStream;
        //    }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceUri">The <see cref="Uri"/> of the source file (cannot be null)</param>
        /// <param name="xukAble">The destination <see cref="IXukAble"/> (cannot be null)</param>
        public OpenXukAction(IXukAble xukAble, Uri sourceUri)
        {
            if (sourceUri == null)
                throw new exception.MethodParameterIsNullException(
                    "The source URI of the OpenXukAction cannot be null");
            if (xukAble == null)
                throw new exception.MethodParameterIsNullException(
                    "The destination IXukAble of the OpenXukAction cannot be null");

            mSourceUri = sourceUri;
            mDestXukAble = xukAble;

            int currentPercentage = 0;

            EventHandler<ProgressEventArgs> progressing = delegate(object sender, ProgressEventArgs e)
            {
                double val = e.Current;
                double max = e.Total;

                int percent = (int)((val / max) * 100);

                if (percent != currentPercentage)
                {
                    currentPercentage = percent;
                    reportProgress_Throttle(currentPercentage, val + " / " + max);
                    //backWorker.ReportProgress(currentPercentage);
                }
                if (RequestCancellation)
                {
                    e.Cancel();
                }
            };

            Progress += progressing;
            
            Finished += delegate(object sender, FinishedEventArgs e)
            {
                Progress -= progressing;
            };
            
            Cancelled += delegate(object sender, CancelledEventArgs e)
            {
                Progress -= progressing;
            };
            if (!mSourceUri.IsFile)
                throw new exception.XukException("The XUK URI must point to a local file!");

            mSourceStream = new FileStream(mSourceUri.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            XmlReaderSettings settings = XmlReaderWriterHelper.GetDefaultXmlReaderConfiguration(false, false, false);

            if (!mDestXukAble.PrettyFormat)
            {
                //
            }
            else
            {
                //
            }

            mXmlReader = XmlReader.Create(mSourceStream, settings, mSourceUri.ToString());
        }

        private void closeInput()
        {
            mXmlReader.Close();
            mXmlReader = null;
            mSourceStream.Close();
            mSourceStream.Dispose();
            mSourceStream = null;
        }


        #region Overrides of ProgressAction

        /// <summary>
        /// Gets the current and estimated total progress values
        /// </summary>
        /// <param name="cur">A <see cref="long"/> in which the current progress is returned</param>
        /// <param name="tot">A <see cref="long"/> in which the estimated total progress is returned</param>
        protected override void GetCurrentProgress(out long cur, out long tot)
        {
            if (mSourceStream != null)
            {
                cur = mSourceStream.Position;
                tot = mSourceStream.Length;
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
            get { return mXmlReader != null; }
        }

        /// <summary>
        /// Execute the command.
        /// </summary>
        /// <exception cref="urakawa.exception.CannotExecuteException">Thrown when the command cannot be reversed.</exception>
        public override void Execute()
        {
            //mHasCancelBeenRequested = false;
            //Progress += OpenXukAction_progress;

            bool canceled = false;
            try
            {
                bool foundRoot = false;
                while (mXmlReader.Read())
                {
                    if (mXmlReader.NodeType == XmlNodeType.Element)
                    {
                        DebugFix.Assert(mDestXukAble is Project);

                        if (mXmlReader.LocalName == "Xuk")
                        {
                            mDestXukAble.PrettyFormat = true;
                            foundRoot = true;
                            break;
                        }
                        else if (mXmlReader.LocalName == "xuk")
                        {
                            mDestXukAble.PrettyFormat = false;
                            foundRoot = true;
                            break;
                        }
                    }
                    else if (mXmlReader.NodeType == XmlNodeType.EndElement)
                    {
                        break;
                    }
                    if (mXmlReader.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                }
                if (!foundRoot)
                {
                    throw new exception.XukException("Could not find Xuk element in XukAble fragment");
                }

                bool foundXukAble = false;
                if (!mXmlReader.IsEmptyElement)
                {
                    while (mXmlReader.Read())
                    {
                        if (mXmlReader.NodeType == XmlNodeType.Element)
                        {
                            //If the element QName matches the Xuk QName equivalent of this, Xuk it in using this.XukIn
                            if (mXmlReader.NamespaceURI == mDestXukAble.GetXukNamespace()
                                && mDestXukAble.GetXukName_().Match(mXmlReader.LocalName))
                            {
                                foundXukAble = true;
                                mDestXukAble.XukIn(mXmlReader, this);
                            }
                            else if (!mXmlReader.IsEmptyElement)
                            {
                                mXmlReader.ReadSubtree().Close();
                            }
                        }
                        else if (mXmlReader.NodeType == XmlNodeType.EndElement)
                        {
                            break;
                        }

                        if (mXmlReader.EOF) throw new exception.XukException("Unexpectedly reached EOF");
                    }
                }
                if (!foundXukAble)
                {
                    throw new exception.XukException("Found no required XukAble in Xuk file");
                }
            }
            catch (exception.ProgressCancelledException)
            {
                canceled = true;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("OpenXuk", ex);
            }
            finally
            {
                //Progress -= OpenXukAction_progress;

                closeInput();

                if (canceled) NotifyCancelled();
                else NotifyFinished();
            }
        }

        //private void OpenXukAction_progress(object sender, urakawa.events.progress.ProgressEventArgs e)
        //{
        //    if (mHasCancelBeenRequested) e.Cancel();
        //}

        private string m_ShortDescription = "Parsing XUK...";
        /// <summary>
        /// Gets a short humanly readable description of the command
        /// </summary>
        public override string ShortDescription
        {
            get { return m_ShortDescription; }
            set { m_ShortDescription = value; }
        }

        private string m_LongDescription = "Parsing  a XUK XML file into an instance of the Urakawa SDK data model...";
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