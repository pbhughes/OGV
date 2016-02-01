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
using Xceed.Wpf.Toolkit;


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

     

        private DelegateCommand<forms.TreeView> _clearStampsCommand;
        public DelegateCommand<forms.TreeView> ClearStampsCommand
        {
            get { return _clearStampsCommand; }
            set { _clearStampsCommand = value; }
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
                    return null;

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

        private string _leftStatus;
        public string LeftStatus
        {
            get
            {
                return _leftStatus;
            }

            set
            {
                _leftStatus = value;
                OnPropertyChanged("LeftStatus");
            }
        }

        private string _rightStatus;
        public string RightStatus
        {
            get
            {
                return _rightStatus;
            }

            set
            {
                _rightStatus = value;
                OnPropertyChanged("RightStatus");
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
                    x.ItemChangedEvent += Item_ItemChangedEvent;
                    x.Title = (item.Element("title") != null) ? item.Element("title").Value : null;
                    x.Description = (item.Element("desc") != null) ? item.Element("desc").Value : null;
                    x.TimeStamp = (item.Element("timestamp") != null) ? TimeSpan.Parse(item.Element("timestamp").Value) : TimeSpan.Zero;
                    _agenda.Items.Add(x);
                    string assingedText = (x.Title.Length < 150) ? x.Title : x.Title.Substring(0, 150);
                    forms.TreeNode xn = new forms.TreeNode() { Text = assingedText , ToolTipText = x.Title };

                    //tag tree node item with the IDa
                    x.ID = NextID().ToString();
                    xn.Tag = x.ID;
                    xn.Name = x.ID;
                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref a, ref xn);
                    }
                    node.Nodes.Add(xn);
                    a.Items.Add(x);
                }
            }
        }

        private void Item_ItemChangedEvent(Item item)
        {
            string title = item.Title;
            RaiseMeetingItemChanged(item);
        }

        public Item FindItem(string id )
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
            _clearStampsCommand = new DelegateCommand<forms.TreeView>(OnClearStamps, CanClearStamps);
            
            _agenda = new Agenda();
          
        }

        private bool CanClearStamps(forms.TreeView arg)
        {
            if (MeetingAgenda != null)
                if (MeetingAgenda.Items.Count > 0)
                    return true;

            return false;
        }

        private void OnClearStamps(forms.TreeView obj)
        {
            string caption = "Do you want to clear all stamps?";
            string content = "If you continue all stamps will be removed and set to zero.  Continue?";
            if(Xceed.Wpf.Toolkit.MessageBox.Show(content, caption, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation) == System.Windows.MessageBoxResult.Yes)
                ClearItemStamps(MeetingAgenda?.Items);
        }

        private void ClearItemStamps( List<Item> collection)
        {
            foreach(Item i in collection)
            {
               i.TimeStamp = TimeSpan.Zero;
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
            //setup the FTP Request object
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

            ClearStampsCommand.RaiseCanExecuteChanged();
            OnRaiseMeetingSetEvent();
            
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

        public void AddNode(Item item, List<Item> collection)
        {
           
            if (collection == null)
                collection = new List<Item>();

            collection.Add(item);
            SelectedItem = item;
        }

        #endregion

        #region Meeting Item Changed Support

        public event MeeingItemChangedEventHandler RaiseMeetingItemChanged;

        private void OnMeetingItemChanged(Item item)
        {
            if (RaiseMeetingItemChanged != null)
                RaiseMeetingItemChanged(item);
        }
        #endregion

        public XDocument GetAgendaXmlDoc()
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

            return xdoc;
        }
        public string GetAgendaXML()
        {
            XDocument xdoc = GetAgendaXmlDoc();
            return xdoc.ToString();
        }
        public long WriteAgendaFile(forms.TreeView agendaTree)
        {
            XDocument xdoc = GetAgendaXmlDoc();

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

            Item agendaItem = FindItem(tn.Name);
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

        public void RemoveItem(string id)
        {
            var item = FindItem(id);
            if (item.Parent == null)
                MeetingAgenda.Items.Remove(item);
            else
                item.Parent.Items.Remove(item);
        }
    }
}
