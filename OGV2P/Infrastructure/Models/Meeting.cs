using System;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Interfaces;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Windows.Controls;
using BuckSoft.Controls.FtpBrowseDialog;
using forms = System.Windows.Forms;
using System.Collections.Specialized;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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
            set {
                _isBusy = value;
                OnPropertyChanged("IsBusy");
                CreateNewAgenda.RaiseCanExecuteChanged();
                LoadAgendaFromFile.RaiseCanExecuteChanged();
                LoadAgendaFromFTP.RaiseCanExecuteChanged();
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

        private DateTime _meetingDate;

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
                    x.TimeStamp = (item.Element("timestamp") != null) ? TimeSpan.Parse(item.Element("timespan").Value) : TimeSpan.Zero;
                    _agenda.Items.Add(x);
                    string assingedText = (x.Title.Length < 150) ? x.Title : x.Title.Substring(0, 150);
                    forms.TreeNode xn = new forms.TreeNode() { Text = assingedText , ToolTipText = x.Title };

                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref a, ref xn);
                    }
                    node.Nodes.Add(xn);
                    a.Items.Add(x);
                }
            }
        }

        public Item FindItem(string title )
        {
            int hashCode = title.GetHashCode();
            foreach (Item i in this.MeetingAgenda.Items)
            {
                if (i.Title.GetHashCode() == hashCode)
                {
                    return i;
                }
                    
            }

            return null;
        }

        public Meeting(ISession sessionService, IUser user)
        {
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
                long bytes = WriteAgendaFile(obj, @"c:\agendaTest.xml");
                BytesWritten = bytes;
            }
            catch (Exception ex)
            {

                string msg = ex.Message;
            }
        
        }

        private bool CanCreateNewAgenda(forms.TreeView arg)
        {
            return !IsBusy;
        }

        private void OnCreateNewAgenda(forms.TreeView obj)
        {
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
            throw new NotImplementedException();
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
                FtpBrowseDialog dlg = new FtpBrowseDialog("ftp.coreyware.com", "test", 21, _user.UserID, _user.Password, true);

                var result = dlg.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == forms.DialogResult.OK)
                {
                    // Open document 
                    fileName = dlg.SelectedFile;

                    //parse the agenda file
                    FtpWebRequest req = (FtpWebRequest)WebRequest.Create(fileName);
                    NetworkCredential creds = new NetworkCredential(_user.UserID, _user.Password);
                    req.Credentials = creds;
                    req.Method = WebRequestMethods.Ftp.DownloadFile;

                    FtpWebResponse response = (FtpWebResponse)req.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());

                    string allText = sr.ReadToEnd();
                    _orginalHash = allText.GetHashCode();


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

        public long WriteAgendaFile(forms.TreeView agendaTree, string location)
        {
            XDocument xdoc = new XDocument(
                    new XElement("meeting",
                        new XElement("clientpathlive", ClientPathLive),
                        new XElement("clientpathlivestream", ClientPathLiveStream),
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
            xdoc.Save(location);
            FileInfo fInfo = new FileInfo(location);
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

            Item agendaItem = FindItem(tn.Text);
            XElement item = new XElement("item",
                                         new XElement("title", agendaItem.Title),
                                         new XElement("desc", agendaItem.Description),
                                         new XElement("timestamp", agendaItem.TimeStamp.ToString()),
                                         new XElement("items"));

            return item;
        }
    }
}
