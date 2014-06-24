using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using urakawa.command;
using urakawa.data;
using urakawa.events.progress;
using urakawa.ExternalFiles;
using urakawa.progress;

namespace urakawa.xuk
{
    ///<summary>
    ///  Action that serializes a xuk data stream from a <see cref="XukAble"/>
    ///</summary>
    public class SaveXukAction : ProgressAction
    {
        public override void DoWork()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Execute();
            stopWatch.Stop();
            Console.WriteLine(@"......XUK-out milliseconds: " + stopWatch.ElapsedMilliseconds);
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

            bool pretty = mSourceXukAble.PrettyFormat;

            XmlWriterSettings settings = XmlReaderWriterHelper.GetDefaultXmlWriterConfiguration(pretty);

            mXmlWriter = XmlWriter.Create(mDestStream, settings);

            if (pretty && mXmlWriter is XmlTextWriter)
            {
                ((XmlTextWriter)mXmlWriter).Formatting = Formatting.Indented;
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="destUri">The <see cref="Uri"/> of the destination (cannot be null)</param>
        /// <param name="xukAble">The source <see cref="IXukAble"/>(cannot be null)</param>
        public SaveXukAction(Project proj, IXukAble xukAble, Uri destUri, bool skipBackup)
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
            //Finished += (sender, e) =>
            //{
            //Progress -= progressing;
            //};

            //dotnet2
            Finished += delegate(object sender, FinishedEventArgs e)
            {
                Progress -= progressing;
            };
            //Cancelled += (sender, e) =>
            //{
            //Progress -= progressing;
            //};

            //dotnet2
            Cancelled += delegate(object sender, CancelledEventArgs e)
            {
                Progress -= progressing;
            };

            string path = mDestUri.LocalPath;
            string parentdir = Path.GetDirectoryName(path);
            if (!Directory.Exists(parentdir))
            {
                FileDataProvider.CreateDirectory(parentdir);
            }

            if (!skipBackup)
            {
                Backup(path);
            }

            mDestStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

            bool pretty = mSourceXukAble.PrettyFormat;

            XmlWriterSettings settings = XmlReaderWriterHelper.GetDefaultXmlWriterConfiguration(pretty);

            mXmlWriter = XmlWriter.Create(mDestStream, settings);

            if (pretty && mXmlWriter is XmlTextWriter)
            {
                ((XmlTextWriter)mXmlWriter).Formatting = Formatting.Indented;
            }
        }

        public static void Backup(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string parentdir = Path.GetDirectoryName(path);

                    string fileName = Path.GetFileName(path);

                    string pathBackup = path;
                    do
                    {
                        //pathBackup = Path.ChangeExtension(Path.GetRandomFileName(), ".BAK");
                        string timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH:mm:ss_K", CultureInfo.InvariantCulture);
                        timeStamp = FileDataProvider.EliminateForbiddenFileNameCharacters(timeStamp).Replace(' ', '_');
                        pathBackup = Path.Combine(parentdir, fileName + "_" + timeStamp);
                    } while (File.Exists(pathBackup));

                    File.Copy(path, pathBackup);
                    try
                    {
                        File.SetAttributes(pathBackup, FileAttributes.Normal);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void closeOutput()
        {
            if (mXmlWriter != null)
            {
                mXmlWriter.Close();
                mXmlWriter = null;
            }
            if (mDestStream != null)
            {
                mDestStream.Close();
                mDestStream.Dispose();
                mDestStream = null;
            }
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
                mXmlWriter.WriteStartElement(m_Project.PrettyFormat ? "Xuk" : "xuk", XukAble.XUK_NS);
                if (!string.IsNullOrEmpty(XukAble.XUK_XSD_PATH))
                {
                    if (string.IsNullOrEmpty(m_Project.GetXukNamespace()))
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
                                          m_Project.GetXukNamespace()
                                          + (m_Project.GetXukNamespace().EndsWith("/") ? "" : "/"),
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
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                throw new Exception("SaveXuk", ex);
            }
            finally
            {
                //Progress -= SaveXukAction_progress;

                closeOutput();

                if (canceled) NotifyCancelled();
                else NotifyFinished();
            }
        }

        public static void GenerateXukSchemas(Project currentProject)
        {
            //<?oxygen RNGSchema="file.rnc" type="compact"?>
            //<?oxygen SCHSchema="file.sch"?>
            try
            {
                GenerateXukSchema(true);
                GenerateXukSchema(false);
            }
            finally
            {
                // XukStrings maintains a pointer to the last-created Project instance!
                //XukStrings.RelocateProjectReference(currentProject);
            }
        }


        private static void GenerateXukSchema(bool isPrettyFormat)
        {
            const string SCHEMA_FILENAME_PREFIX = @"Tobi_Schema_";
            const string SCHEMA_FILENAME_EXTENSION = @"rnc";

            string schema_PrettyXuk_FilePath = Path.Combine(ExternalFilesDataManager.STORAGE_FOLDER_PATH, SCHEMA_FILENAME_PREFIX + "PrettyXuk" + "." + SCHEMA_FILENAME_EXTENSION);
            string schema_CompressedXuk_FilePath = Path.Combine(ExternalFilesDataManager.STORAGE_FOLDER_PATH, SCHEMA_FILENAME_PREFIX + "CompressedXuk" + "." + SCHEMA_FILENAME_EXTENSION);

            //xsi:noNamespaceSchemaLocation ===> XukAble.XUK_NS + "/" + XukAble.XUK_XSD_PATH

            Project project = new Project();
            project.PrettyFormat = isPrettyFormat;

            StreamWriter streamWriter = new StreamWriter(isPrettyFormat ? schema_PrettyXuk_FilePath : schema_CompressedXuk_FilePath, false, Encoding.UTF8);
            try
            {
                streamWriter.WriteLine("default namespace = \"" + XukAble.XUK_NS + "\"");
                //streamWriter.WriteLine("namespace xuk2 = \""+XukAble.XUK_NS+"\"");
                //streamWriter.WriteLine("namespace xuk1 = \"http://www.daisy.org/urakawa/xuk/1.0\"");
                //streamWriter.WriteLine("namespace obi = \"http://www.daisy.org/urakawa/obi\"");
                //streamWriter.WriteLine("namespace xsi = \"http://www.w3.org/2001/XMLSchema-instance\"");

                //streamWriter.WriteLine("start = element " + XukStrings.Xuk + " { " + XukStrings.Xuk + ".attlist & " + XukStrings.Xuk + ".content }");
                //streamWriter.WriteLine(XukStrings.Xuk + ".attlist = " + XukStrings.Xuk + ".NOOP.attr");

                streamWriter.WriteLine("start = element " + (isPrettyFormat ? "Xuk" : "xuk")
                    + " { " +
                    XukAble.GetXukName(typeof(Project)).z(isPrettyFormat)
                    + " }");

                streamWriter.Write(Project.GetXukSchema(isPrettyFormat));
            }
            finally
            {
                streamWriter.Close();
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