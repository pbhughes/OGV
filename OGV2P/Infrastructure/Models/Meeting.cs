using System;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Interfaces;
using System.IO;
using System.Xml.Linq;
using System.Net;
using BuckSoft.Controls.FtpBrowseDialog;
using forms = System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Deployment.Application;


namespace Infrastructure.Models
{

    public class Meeting : INotifyPropertyChanged, IMeeting
    {
        private int _orginalHash;
        private ISession _sessionService;
        private IUser _user;

        private bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        private int _lastID;
        public int LastID
        {
            get
            {
                return _lastID;
            }

            set
            {
                _lastID = value;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();

                return "Debug";
            }
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        private DelegateCommand<forms.TreeView> _createNewAgenda;
        public DelegateCommand<forms.TreeView> CreateNewAgenda
        {
            get
            {
                return _createNewAgenda;
            }

            set
            {
                _createNewAgenda = value;
            }
        }

        private DelegateCommand<forms.TreeView> _loadAgendaFromFile;
        public DelegateCommand<forms.TreeView> LoadAgendaFromFile
        {
            get
            {
                return _loadAgendaFromFile;
            }

            set
            {
                _loadAgendaFromFile = value;
            }
        }

        private DelegateCommand<forms.TreeView> _loadAgendaFromFTP;
        public DelegateCommand<forms.TreeView> LoadAgendaFromFTP
        {
            get { return _loadAgendaFromFTP; }
            set { _loadAgendaFromFTP = value; }
        }

        private DelegateCommand<forms.TreeView> _saveAgendaFile;
        public DelegateCommand<forms.TreeView> SaveAgendaFile
        {
            get
            {
                return _saveAgendaFile;
            }

            set
            {
                _saveAgendaFile = value;
            }
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged("FileName"); }
        }

        private string _meetingName;
        public string MeetingName
        {
            get { return _meetingName; }
            set { 
                _meetingName = value; 
                _sessionService.MeetingName = value; 
                OnPropertyChanged("MeetingName"); 
            }
        }

        private string _localFile;
        public string LocalFile
        {
            get
            {
                return _localFile;
            }

            set
            {
                _localFile = value;
            }
        }

        private DateTime _meetingDate = DateTime.Now;

        public DateTime MeetingDate
        {
            get { return _meetingDate; }
            set { _meetingDate = value; OnPropertyChanged("MeetingDate"); }
        }

        private Agenda _agenda;

        public Agenda MeetingAgenda
        {
            get { return _agenda; }
            set { _agenda = value; OnPropertyChanged("MeetingAgenda"); }
        }

        private string _clientPathLive;
        public string ClientPathLive
        {
            get
            {
                return _clientPathLive;
            }

            set
            {
                _clientPathLive = value;
                OnPropertyChanged("ClientPathLive");
                OnPropertyChanged("PublishingPoint");
            }
        }

        private string _clientPathLiveStream;
        public string ClientPathLiveStream
        {
            get
            {
                return _clientPathLiveStream;
            }

            set
            {
                _clientPathLiveStream = value;
                OnPropertyChanged("ClientPathLiveStream");
                OnPropertyChanged("PublishingPoint");
            }
        }

        private int _videoWidth;
        public int VideoWidth
        {
            get
            {
                return _videoWidth;
            }

            set
            {
                _videoWidth = value;
                OnPropertyChanged("VideoWidth");
            }
        }

        private int _videoHeight;
        public int VideoHeight
        {
            get
            {
                return _videoHeight;
            }

            set
            {
                _videoHeight = value;
                OnPropertyChanged("VideoHeight");
            }
        }

        private int _frameRate;
        public int FrameRate
        {
            get
            {
                return _frameRate;
            }

            set
            {
                _frameRate = value;
                OnPropertyChanged("FrameRate");
            }
        }

        private string _landingPage;
        public string LandingPage
        {
            get
            {
                return _landingPage;
            }

            set
            {
                _landingPage = value;
                OnPropertyChanged("LandingPage");
            }
        }

        public string PublishingPoint
        {
            get
            {
                if (_clientPathLive == null || _clientPathLiveStream == null)
                    return string.Empty;

                UriBuilder urib = new UriBuilder(_clientPathLive);
                urib.Path += "/" + _clientPathLiveStream;
                return urib.ToString();
            }
            private set
            {
                ;
            }
        }

        private long _bytesWritten;
        public long BytesWritten
        {
            get
            {
                return _bytesWritten;
            }

            set
            {
                _bytesWritten = value;
                OnPropertyChanged("BytesWritten");
            }
        }

        private string _localAgendaFileName;
        public string LocalAgendaFileName
        {
            get
            {
                return _localAgendaFileName;
            }

            set
            {
                _localAgendaFileName = value;
                OnPropertyChanged("LocalAgendaFileName");
            }
        }


        public event MeetingSetEventHandler RaiseMeetingSetEvent;

        private void OnRaiseMeetingSetEvent()
        {
            if(RaiseMeetingSetEvent != null)
            {
                RaiseMeetingSetEvent(this, new EventArgs());
            }
        }

        private bool CanUpateSelectedItem()
        {
            return true;
        }

        private void OnUpateSelectedItem()
        {
            int x = 0;
        }

        private void ParseItems(XElement items, ref Agenda a, ref forms.TreeNode node)
        {
            if (items != null)
            {
                foreach (XElement item in items.Elements("item"))
                {
                    Item x = new Item();
                    x.Title = (item.Element("title") != null) ? item.Element("title").Value : null;
                    x.Description = (item.Element("desc") != null) ? item.Element("desc").Value : null;
                    x.TimeStamp = (item.Element("timestamp") != null) ? TimeSpan.Parse(item.Element("timestamp").Value) : TimeSpan.Zero;
                    _agenda.Items.Add(x);
                    string assingedText = (x.Title.Length < 150) ? x.Title : x.Title.Substring(0, 150);
                    forms.TreeNode xn = new forms.TreeNode() { Text = assingedText , ToolTipText = x.Title };

                    //tag treenode item with the ID
                    x.ID = NextID();
                    xn.Tag = x.ID;
                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref a, ref xn);
                    }
                    node.Nodes.Add(xn);
                    a.Items.Add(x);
                }
            }
        }

        public Item FindItem(int id )
        {
            
            foreach (Item i in this.MeetingAgenda.Items)
            {
                if (i.ID == id)
                {
                    return i;
                }
                    
            }

            return null;
        }

        public Meeting(ISession sessionService, IUser user)
        {
            if (user.SelectedBoard != null)
            {
                _meetingDate = DateTime.Now;
                _meetingName = string.Format("{0}_{1}", (user.SelectedBoard == null) ? "select_a_board" : user.SelectedBoard.Name, _meetingDate.ToString("mm-dd-yyyy"));
            }
            _sessionService = sessionService;
            _user = user;
            _sessionService.RaiseStamped += _sessionService_RaiseStamped;
            _loadAgendaFromFTP = new DelegateCommand<forms.TreeView>(OnLoadAgendaFromFTP, CanLoadAgendaFromFTP);
            _loadAgendaFromFile = new DelegateCommand<System.Windows.Forms.TreeView>(OnLoadAgendaFromFile, CanLoadAgendaFromFile);
            _createNewAgenda = new DelegateCommand<System.Windows.Forms.TreeView>(OnCreateNewAgenda, CanCreateNewAgenda);
            _saveAgendaFile = new DelegateCommand<System.Windows.Forms.TreeView>(OnSaveAgendaFile, CanSaveAgendaFile);
            _agenda = new Agenda();
          
        }

        private bool CanSaveAgendaFile(forms.TreeView arg)
        {
            return MeetingAgenda != null && MeetingAgenda.Items.Count > 0;
        }

        private void OnSaveAgendaFile(forms.TreeView obj)
        {
            try
            {
                long totalBytes = WriteAgendaFile(obj);

                Task t = new Task( (  ) => {
                    string msg = PushFile(LocalAgendaFileName);
                });

                t.Start();

            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
            finally
            {

            }
        
        }

        private string PushFile( string fileName )
        {
            long totalBytesToSend;
            int bytes;

            byte[] buffer = new byte[4097];
            FileInfo fi = new FileInfo(fileName);
            totalBytesToSend = fi.Length;
            bytes = 0;
            //setup the ftp Request object
            Uri uri = new Uri(string.Format("ftp://{0}/{1}/{2}", _user.SelectedBoard.FtpServer, _user.SelectedBoard.FtpPath, fi.Name));
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(uri.ToString());
            NetworkCredential creds = new NetworkCredential(_user.UserID, _user.Password);
            req.Credentials = creds;
            req.Method = WebRequestMethods.Ftp.UploadFile;
            req.UseBinary = true;
            req.KeepAlive = true;
            req.ContentLength = fi.Length;

            //read the file to send 4097 bytes at a time
            FileStream fs = fi.OpenRead();
            Stream ss = req.GetRequestStream();
            while(totalBytesToSend > 0)
            {
                bytes = fs.Read(buffer, 0, buffer.Length);
                ss.Write(buffer, 0, bytes);
                totalBytesToSend = totalBytesToSend - bytes;
                BytesWritten += bytes;
            }

            //get a write stream from the request and write the content
            fs.Close();
            ss.Close();
            
            FtpWebResponse response = (FtpWebResponse)req.GetResponse();
            string status = response.StatusDescription;
            response.Close();
            return status;
        }

        private bool CanCreateNewAgenda(forms.TreeView arg)
        {
            return ! IsBusy;
        }

        private void OnCreateNewAgenda(forms.TreeView obj)
        {
           if(MeetingAgenda != null && MeetingAgenda.Items.Count > 0)
            {
                if (Xceed.Wpf.Toolkit.MessageBox.Show("Save current Agenda?", "Save Current Agenda", System.Windows.MessageBoxButton.YesNo) ==
                    System.Windows.MessageBoxResult.Yes)
                {

                }

            }
            this.MeetingAgenda = new Agenda();
            if(MeetingAgenda.Items == null)
            {
                MeetingAgenda.Items = new List<Item>();
            }

            MeetingName = "Please enter a meeting name...";
            MeetingDate = DateTime.Now;

            Item newItem = new Item() { Title = "Please add a new title..." };
            MeetingAgenda.Items.Add(newItem);

            forms.TreeNode x = new forms.TreeNode() { Text = newItem.Title, ToolTipText = newItem.Title };
            obj.Nodes.Add(x);

            OnRaiseMeetingSetEvent();
            ReevaluateCommands();
        }

        private bool CanLoadAgendaFromFile(forms.TreeView arg)
        {
            return !IsBusy;
        }

        private void OnLoadAgendaFromFile(forms.TreeView obj)
        {
            try
            {
                forms.OpenFileDialog dg = new forms.OpenFileDialog();
                dg.DefaultExt = ".xml";
                dg.AddExtension = true;
                dg.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase");
                if (!Directory.Exists(dg.InitialDirectory))
                    Directory.CreateDirectory(dg.InitialDirectory);

           
                if(dg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _localAgendaFileName = dg.FileName;
                    string allXml = File.ReadAllText(dg.FileName);
                    ParseAgendaFile(obj, allXml);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
          


        }

        private bool CanLoadAgendaFromFTP(forms.TreeView agendaTree)
        {
            return ! IsBusy;
        }

        private void OnLoadAgendaFromFTP(forms.TreeView agendaTree)
        {
            string fileName = string.Empty;
            try
            {


                // Create OpenFileDialog 
                FtpBrowseDialog dlg = new FtpBrowseDialog(_user.SelectedBoard.FtpServer, _user.SelectedBoard.FtpPath, 21, _user.UserID, _user.Password, true);

                var result = dlg.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == forms.DialogResult.OK)
                {
                    // Open document 
                    fileName = dlg.SelectedFile;


                    string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ClerkBase");
                    if (!File.Exists(dir))
                        Directory.CreateDirectory(dir);

                    _localAgendaFileName = Path.Combine(dir, dlg.SelectedFileName);

                    //setup the ftp request then parse the agenda file 
                    FtpWebRequest req = (FtpWebRequest)WebRequest.Create(fileName);
                    NetworkCredential creds = new NetworkCredential(_user.UserID, _user.Password);
                    req.Credentials = creds;
                    req.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());

                    string allText = sr.ReadToEnd();
                    _orginalHash = allText.GetHashCode();

                    ParseAgendaFile(agendaTree, allText);

                }
            }
            catch (COMException cex)
            {
                ; //ignore it

            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(string.Format("Unable to download file :{0} verify the filename is correct.", fileName));

            }


        }

        public void ParseAgendaFile(forms.TreeView agendaTree, string allText)
        {
            XDocument xDoc = XDocument.Parse(allText);
            MeetingName = xDoc.Element("meeting").Element("meetingname").Value;

            ClientPathLive = xDoc.Element("meeting").Element("clientpathlive").Value;
            ClientPathLiveStream = xDoc.Element("meeting").Element("clientpathlivestream").Value;

            MeetingDate = (xDoc.Element("meeting").Element("meetingdate") != null) ?
                DateTime.Parse(xDoc.Element("meeting").Element("meetingdate").Value) :
                DateTime.Now;

            VideoHeight = int.Parse(xDoc.Element("meeting").Element("videoheight").Value);
            VideoWidth = int.Parse(xDoc.Element("meeting").Element("videowidth").Value);
            FrameRate = int.Parse(xDoc.Element("meeting").Element("framerate").Value);
            LandingPage = xDoc.Element("meeting").Element("landingpage").Value;

            this.LocalFile = string.Format("{0}-{1}-{2}_{3}.mp4", this.MeetingDate.Day, this.MeetingDate.Month, this.MeetingDate.Year, this.MeetingName);
            XElement items = xDoc.Element("meeting").Element("agenda").Element("items");
            if (items != null)
            {
                forms.TreeNode root = new forms.TreeNode();
                Agenda a = new Agenda();

                ParseItems(items, ref a, ref root);

                foreach (forms.TreeNode x in root.Nodes)
                {
                    agendaTree.Nodes.Add(x);
                }
                agendaTree.ShowPlusMinus = true;
                agendaTree.ShowLines = true;
                agendaTree.ExpandAll();
            }

            OnRaiseMeetingSetEvent();
            ReevaluateCommands();
        }

        private void ReevaluateCommands()
        {
            SaveAgendaFile.RaiseCanExecuteChanged();
        }

        private void _sessionService_RaiseStamped(TimeSpan sessionTime)
        {
            if (_selectedItem != null)
            {
                _selectedItem.TimeStamp = sessionTime;
                
            }
               
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void AddNode(Item item)
        {
            List<Item> collection;
            if(SelectedItem == null)
            {
                collection = SelectedItem.Items;
            }
            else
            {
                collection = MeetingAgenda.Items;
            }

            if (collection == null)
                collection = new List<Item>();

            collection.Add(item);
            SelectedItem = item;
        }





        #endregion

        public long WriteAgendaFile(forms.TreeView agendaTree)
        {
            XDocument xdoc = new XDocument(
                    new XElement("meeting",
                        new XElement("clientpathlive", ClientPathLive),
                        new XElement("clientpathlivestream", ClientPathLiveStream),
                        new XElement("meetingname", MeetingName),
                        new XElement("meetingdate", MeetingDate.ToShortDateString()),
                        new XElement("videoheight", VideoHeight.ToString()),
                        new XElement("videowidth", VideoWidth.ToString()),
                        new XElement("framerate", FrameRate.ToString()),
                        new XElement("landingpage", LandingPage),
                        new XElement("agenda", new XElement("items"))
                    )
            );

            foreach(forms.TreeNode tn in agendaTree.Nodes)
            {
                XElement root = ProcessNodes(tn);
                xdoc.Element("meeting").Element("agenda").Element("items").Add(root);
            }
            xdoc.Save(_localAgendaFileName);
            FileInfo fInfo = new FileInfo(_localAgendaFileName);
            return fInfo.Length;
        }

        private XElement ProcessNodes(forms.TreeNode tn)
        {
            XElement item = CreateAnItem(tn);
            if(tn.Nodes.Count > 0)
            {
                foreach(forms.TreeNode subNode in tn.Nodes)
                {
                    XElement sub = ProcessNodes(subNode);
                    item.Element("items").Add(sub);
                    
                }
            }

            return item;
            
        }

        private XElement CreateAnItem(forms.TreeNode tn)
        {

            Item agendaItem = FindItem((int)tn.Tag);
            XElement item = new XElement("item",
                                         new XElement("title", agendaItem.Title),
                                         new XElement("desc", agendaItem.Description),
                                         new XElement("timestamp", agendaItem.TimeStamp.ToString()),
                                         new XElement("items"));

            return item;
        }

        public int NextID()
        {
            return _lastID++;
        }
    }
}
