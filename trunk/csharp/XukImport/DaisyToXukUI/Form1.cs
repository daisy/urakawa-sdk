using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using XukImport;
using urakawa;
using urakawa.media.data;
using urakawa.progress;
using urakawa.property.channel;
using urakawa.xuk;

namespace DaisyToXukUI
{
    public class RenameChannelCommand : urakawa.command.Command
    {
        //public new const string XUK_NS = "http://www.test.org";

        public static Project mProject;
        private Channel m_Channel;
        private string m_newName;
        private string m_oldName;

        protected override void XukInAttributes(XmlReader source)
        {
            m_newName = source.GetAttribute(XukString_NewName);
            m_oldName = source.GetAttribute(XukString_OldName);
            m_Channel = Presentation.ChannelsManager.GetChannel(source.GetAttribute(XukString_ChannelId));
            base.XukInAttributes(source);
        }

        protected override void XukInChild(XmlReader source, ProgressHandler handler)
        {
            //nothing new here 
            base.XukInChild(source, handler);
        }

        protected override void XukOutAttributes(XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString(XukString_NewName, m_newName);
            destination.WriteAttributeString(XukString_OldName, m_oldName);
            destination.WriteAttributeString(XukString_ChannelId, Presentation.ChannelsManager.GetUidOfChannel(m_Channel));
            base.XukOutAttributes(destination, baseUri);
        }


        protected override void XukOutChildren(XmlWriter destination, Uri baseUri, ProgressHandler handler)
        {
            //nothing new here
            base.XukOutChildren(destination, baseUri, handler);
        }


        public static string XukString
        {
            get { return (mProject.IsPrettyFormat() ? "RenameChannelCommand" : "RenChCmd"); }
        }

        protected string XukString_NewName
        {
            get { return (Presentation.Project.IsPrettyFormat() ? "NewName" : "new"); }
        }
        protected string XukString_OldName
        {
            get { return (Presentation.Project.IsPrettyFormat() ? "OldName" : "old"); }
        }
        protected string XukString_ChannelId
        {
            get { return (Presentation.Project.IsPrettyFormat() ? "ChannelId" : "chId"); }
        }

        public RenameChannelCommand()
        {
            m_Channel = null;
            m_newName = null;
            m_oldName = null;
        }
        public void Init(Channel channel, string theNewChannelName)
        {
            m_Channel = channel;
            m_newName = theNewChannelName;
            m_oldName = channel.Name;
        }

        public override string GetTypeNameFormatted()
        {
            return (Presentation.Project.IsPrettyFormat() ? "RenameChannelCommand" : "RenChCmd");
        }

        public override bool CanExecute
        {
            get { return true; }
        }

        public override bool CanUnExecute
        {
            get { return true; }
        }

        public override string LongDescription
        {
            get { return "Change the name of the channel with the given string."; }
        }

        public override string ShortDescription
        {
            get { return "Rename channel"; }
        }

        public override void Execute()
        {
            m_Channel.Name = m_newName;
        }

        public override void UnExecute()
        {
            m_Channel.Name = m_oldName;
        }
    }

    public partial class Form1 : Form
    {
        private string m_ImportFile;  //variable to accept the file selected by the user
        private string m_SaveDir;     // path where the xuk files will be saved

        public Form1()
        {
            InitializeComponent();
        }

        //Select Import button to select the xml/opf file
        private void btnLoad_Click(object sender, EventArgs e)
        {
            txtBookName.Clear();
            txtSaveBookName.Clear();

            var open = new OpenFileDialog();
            //open.InitialDirectory = @"C:\";
            open.Filter = "XML Files (*.xml)|*.xml|OPF Files (*.opf)|*.opf|EPUB Files (*.epub)|*.epub|All files(*.*)|*.*";
            open.FilterIndex = 1;
            open.RestoreDirectory = true;
            if (open.ShowDialog(this) == DialogResult.OK)
            {
                txtBookName.Text = open.FileName;
                m_ImportFile = txtBookName.Text;
                btnSave.Enabled = true;
            }
        }//btnLoad

        //Start Import button to strat the conversion process
        private void btnStartImport_Click(object sender, EventArgs e)
        {
            var converter = new XukImport.DaisyToXuk(m_ImportFile, m_SaveDir);
            Channel channelText = null;
            Channel channelAudio = null;
            List<Channel> listCh = converter.Project.GetPresentation(0).ChannelsManager.ListOfChannels;
            foreach (Channel ch in listCh)
            {
                if (ch is TextChannel)
                {
                    channelText = ch;
                }
                else if (ch is AudioChannel)
                {
                    channelAudio = ch;
                }
            }

            RenameChannelCommand.mProject = converter.Project;

            RenameChannelCommand cmd1 = null;
            if (channelText != null)
            {
                cmd1 = converter.Project.GetPresentation(0).CommandFactory.Create<RenameChannelCommand>();
                cmd1.Init(channelText, "The new TEXT Channel Name");
            }

            RenameChannelCommand cmd2 = null;
            if (channelAudio != null)
            {
                cmd2 = converter.Project.GetPresentation(0).CommandFactory.Create<RenameChannelCommand>();
                cmd2.Init(channelAudio, "The new AUDIO Channel Name");
            }

            RenameChannelCommand cmd3 = null;
            if (channelText != null)
            {
                cmd3 = converter.Project.GetPresentation(0).CommandFactory.Create<RenameChannelCommand>();
                cmd3.Init(channelText, "NEW TEXT Channel Name");
            }

            /*
            DeleteChannelCommand cmd2 = null;
            if (channelAudio != null)
            {
                cmd2 = new DeleteChannelCommand(channelAudio);
            }
             * */


            converter.Project.GetPresentation(0).UndoRedoManager.StartTransaction("rename transaction", null);

            if (cmd1 != null)
            {
                converter.Project.GetPresentation(0).UndoRedoManager.Execute(cmd1);
            }

            if (cmd2 != null)
            {
                converter.Project.GetPresentation(0).UndoRedoManager.Execute(cmd2);
            }

            converter.Project.GetPresentation(0).UndoRedoManager.EndTransaction();


            if (cmd3 != null)
            {
                converter.Project.GetPresentation(0).UndoRedoManager.Execute(cmd3);
            }

            converter.Project.GetPresentation(0).UndoRedoManager.Undo();

            Uri uriComp = new Uri(m_SaveDir + ".COMPRESSED.xuk");

            converter.Project.SetPrettyFormat(false);

            {
                SaveXukAction actionSave = new SaveXukAction(converter.Project, converter.Project, uriComp);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            Uri uriPretty = new Uri(m_SaveDir + ".PRETTY.xuk");

            converter.Project.SetPrettyFormat(true);

            {
                SaveXukAction actionSave = new SaveXukAction(converter.Project, converter.Project, uriPretty);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }

            /////
            //// Make sure we don't create concurrent access to WAV files while opening the same XUK file in several projects.
            converter.Project.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
            /////
            Project projectPretty = new Project();
            {
                OpenXukAction actionOpen = new OpenXukAction(projectPretty, uriPretty);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }
            projectPretty.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
            System.Diagnostics.Debug.Assert(converter.Project.ValueEquals(projectPretty));
            
            Project projectComp = new Project();
            {
                OpenXukAction actionOpen = new OpenXukAction(projectComp, uriComp);
                bool openWasCancelled;
                Progress.ExecuteProgressAction(actionOpen, out openWasCancelled);
                if (openWasCancelled)
                {
                    return;
                }
            }
            projectComp.GetPresentation(0).DataProviderManager.CompareByteStreamsDuringValueEqual = false;
            System.Diagnostics.Debug.Assert(converter.Project.ValueEquals(projectComp));

            //// Make sure we don't create concurrent access to WAV files while opening the same XUK file in several projects.
            System.Diagnostics.Debug.Assert(projectComp.ValueEquals(projectPretty));

            converter.Project.GetPresentation(0).UndoRedoManager.Redo();

            System.Diagnostics.Debug.Assert(!converter.Project.ValueEquals(projectPretty));


            //TODO: there is a bug in the Urakawa SDK due to the static sharing of the XukString !
            // In other words, the Factories will not refresh their QNames in the following call to SetPrettyFormat(),
            // because converter.Project was already set to PRETTY a while ago (see code above). Problem:
            // the "projectComp" instance created above has set the Factory QNames to COMPRESSED
            // and due to the static sharing of XukStrings, converter.Project is affected. As a result, the PRETTY2 file
            // is nicely indented but with compressed q-names.

            Uri uriPretty2 = new Uri(m_SaveDir + ".PRETTY2.xuk");

            converter.Project.SetPrettyFormat(true);

            {
                SaveXukAction actionSave = new SaveXukAction(converter.Project, converter.Project, uriPretty2);
                bool saveWasCancelled;
                Progress.ExecuteProgressAction(actionSave, out saveWasCancelled);
                if (saveWasCancelled)
                {
                    return;
                }
            }
        }//btnStartImport

        //Save button to select the directory where the xuk files will be saved.
        private void btnSave_Click(object sender, EventArgs e)
        {
            var saveFile = new FolderBrowserDialog();
            saveFile.SelectedPath = txtSaveBookName.Text;
            saveFile.ShowNewFolderButton = true;
            if (saveFile.ShowDialog(this) == DialogResult.OK)
            {
                btnStartImport.Enabled = true;            //start import button enabled when user selects the save path directory.
                txtSaveBookName.Text = saveFile.SelectedPath;
                string r = Path.GetFileNameWithoutExtension(m_ImportFile);
                m_SaveDir = Path.Combine(txtSaveBookName.Text, r);
            }
        }//btnSave

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSaveBookName.Clear();
            txtBookName.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

    }//class
}//namespace
