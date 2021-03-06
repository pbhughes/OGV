﻿using Infrastructure.ExtendedObjects;
using Infrastructure.Interfaces;
using Microsoft.Practices.Prism.Commands;
using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using forms = System.Windows.Forms;

namespace Infrastructure.Models
{
    public class Meeting : INotifyPropertyChanged, IMeeting
    {
        private ISession _sessionService;
        private IUser _user;
        private forms.TreeView _agendaTree;

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


        private bool _isLive;
        public bool IsLive
        {
            get
            {
                return _isLive;
            }

            set
            {
                _isLive = value;
                OnPropertyChanged("IsLive");
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
            set
            {
                _meetingName = value;
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
            get {
                return _meetingDate;
            }
            set {
                _meetingDate = value;
                OnPropertyChanged("MeetingDate");
            }
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
                {
                    _isLive = false;
                    return null;
                }

                _isLive = true;
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

        private long _startingHash;

        public long StartingHash
        {
            get
            {
                return _startingHash;
            }

            set
            {
                _startingHash = value;
                OnPropertyChanged("StartingHash");
            }
        }

        public long EndingHash
        {
            get
            {
                return this.ToString().GetHashCode();
            }
        }

        public bool HasChanged
        {
            get
            {
                return StartingHash != EndingHash;
            }
        }

        private void GetString(ExtendedTreeNode etn, ref string composite)
        {
           

            if (etn.AgendaItem != null)
            {
                composite += etn.AgendaItem.Title + etn.AgendaItem.Description;
            }

            foreach (ExtendedTreeNode node in etn.Nodes)
            {
                GetString(node, ref composite);
            }
        }

        public override string ToString()
        {
            string part = MeetingName + MeetingDate.ToString();

            if (_agendaTree != null)
            {
                foreach (ExtendedTreeNode etn in _agendaTree.Nodes)
                {
                    GetString(etn, ref part);

                }
            }

            return part;
        }

        private bool CanUpateSelectedItem()
        {
            return true;
        }

        private void ParseItems(XElement items, ref ExtendedTreeNode node)
        {
           
            if (items != null)
            {
                foreach (XElement item in items.Elements("item"))
                {
                    Item x = new Item();

                    x.Title = (item.Element("title") != null) ? item.Element("title").Value : null;
                    x.Description = (item.Element("description") != null) ? item.Element("description").Value : null;
                    x.TimeStamp = (item.Element("timestamp") != null) ? int.Parse(item.Element("timestamp").Value.ToString()) : 0;
                    x.UpdateHash();
                    string assingedText = (x.Title.Length < 150) ? x.Title : x.Title.Substring(0, 150);
                    ExtendedTreeNode xn = new ExtendedTreeNode() { Text = assingedText, ToolTipText = x.Title, AgendaItem = x };

                    if (xn.AgendaItem.TimeStamp != 0)
                    {

                        xn.MarkItemStamped(xn.AgendaItem.Title, x.TimeStamp, false);
                        _agendaTree.SelectedNode = xn;
                    }
                    else
                    {
                        xn.MarkItemUnstamped();
                    }
                    //tag tree node item with the IDa
                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref xn);
                    }
                    node.Nodes.Add(xn);

                }
            }
         
        }


        public Meeting(ISession sessionService, IUser user)
        {

            _sessionService = sessionService;
            _user = user;
            _sessionService.RaiseStamped += _sessionService_RaiseStamped;
            _clearStampsCommand = new DelegateCommand<forms.TreeView>(OnClearStamps, CanClearStamps);

            StartingHash = this.ToString().GetHashCode();
        }

        private bool CanClearStamps(forms.TreeView arg)
        {
            if (_agendaTree != null)
                if (_agendaTree.Nodes.Count > 0)
                    return true;

            return false;
        }

        private void OnClearStamps(forms.TreeView obj)
        {
            string caption = "Do you want to clear all stamps?";
            string content = "If you continue all stamps will be removed and set to zero.  Continue?";
            if (Xceed.Wpf.Toolkit.MessageBox.Show(content, caption, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Exclamation) == System.Windows.MessageBoxResult.Yes)
                ClearItemStamps(_agendaTree.Nodes);
        }

        private void ClearItemStamps(forms.TreeNodeCollection collection)
        {
            foreach (forms.TreeNode tn in collection)
            {
                if (tn.Nodes.Count > 0)
                    ClearItemStamps(tn.Nodes);
                else
                    ((ExtendedTreeNode)tn).SetTimeStamp(0);
            }

        }

        private string PushFile(string fileName)
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
            while (totalBytesToSend > 0)
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
            
            _agendaTree = agendaTree;
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
                ExtendedObjects.ExtendedTreeNode root = new ExtendedObjects.ExtendedTreeNode();

                ParseItems(items, ref root);

                foreach (ExtendedTreeNode x in root.Nodes)
                {
                    agendaTree.Nodes.Add(x);
                   
                }
                agendaTree.ShowPlusMinus = true;
                agendaTree.ShowLines = true;
                agendaTree.ExpandAll();
            }

            ClearStampsCommand.RaiseCanExecuteChanged();
            StartingHash = this.ToString().GetHashCode();
            OnRaiseMeetingSetEvent();
            
        }

        private void _sessionService_RaiseStamped(TimeSpan sessionTime)
        {
            if (_selectedItem != null)
            {
                _selectedItem.TimeStamp = (int)sessionTime.TotalSeconds;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        #region Event Support

        public event MeeingItemChangedEventHandler RaiseMeetingItemChanged;
        public event MeetingSetEventHandler RaiseMeetingSetEvent;

        private void OnMeetingItemChanged(Item item)
        {
            if (RaiseMeetingItemChanged != null)
                RaiseMeetingItemChanged(item);
        }

        private void OnRaiseMeetingSetEvent()
        {
            if (RaiseMeetingSetEvent != null)
            {
                RaiseMeetingSetEvent(this, new EventArgs());
            }
        }

       

        #endregion Meeting Item Changed Support

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

        public long WriteAgendaFile(ExtendedTreeView agendaTree)
        {
            try
            {
                _agendaTree = agendaTree;
                string fileName = LocalAgendaFileName;
                StringBuilder content = new StringBuilder();

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.ConformanceLevel = ConformanceLevel.Fragment;
                settings.OmitXmlDeclaration = true;
                settings.CheckCharacters = true;
                settings.Indent = true;
                settings.CloseOutput = true;
                settings.IndentChars = "\t";
                using (var r = XmlWriter.Create(content, settings))
                {
                    r.WriteStartElement("meeting");
                    r.WriteElementString("clientpathlive", ClientPathLive);
                    r.WriteElementString("clientpathlivestream", ClientPathLiveStream);
                    r.WriteElementString("meetingname", MeetingName);
                    r.WriteElementString("meetingdate", MeetingDate.ToShortDateString() );
                    r.WriteElementString("videoheight", VideoHeight.ToString());
                    r.WriteElementString("videowidth", VideoWidth.ToString());
                    r.WriteElementString("framerate", FrameRate.ToString());
                    r.WriteElementString("landingpage", LandingPage);
                    r.WriteStartElement("agenda");
                    r.WriteStartElement("items");
                    r.WriteRaw(agendaTree.ToString());
                    r.WriteEndElement();
                    r.WriteEndElement();
                    r.WriteEndElement();
                }
                string allText = content.ToString();
                File.WriteAllText(_localAgendaFileName, allText);
                FileInfo f = new FileInfo(_localAgendaFileName);

                return f.Length;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private XElement ProcessNodes(ExtendedTreeNode tn)
        {
            XElement item = CreateAnItem(tn);
            if (tn.Nodes.Count > 0)
            {
                foreach (ExtendedTreeNode subNode in tn.Nodes)
                {
                    XElement sub = ProcessNodes(subNode);
                    item.Element("items").Add(sub);
                }
            }

            return item;
        }

        private XElement CreateAnItem(ExtendedTreeNode tn)
        {
            Item agendaItem = tn.AgendaItem as Item;
            XElement item = new XElement("item",
                                         new XElement("title", agendaItem.Title),
                                         new XElement("desc", agendaItem.Description),
                                         new XElement("timestamp", agendaItem.TimeStamp.ToString()),
                                         new XElement("items"));

            return item;
        }

        private bool _showSettings;
        public bool ShowSettings
        {
            get
            {
                return _showSettings;
            }

            set
            {
                _showSettings = value;
                OnPropertyChanged("ShowSettings");
            }
        }
    }
}