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
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

      

        private DelegateCommand<forms.TreeView> _loadAgenda;
        public DelegateCommand<forms.TreeView> LoadAgenda
        {
            get { return _loadAgenda; }
            set { _loadAgenda = value; }
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

      

        public event MeetingSetEventHandler RaiseMeetingSetEvent;

        private void OnRaiseMeetingSetEvent()
        {
            if(RaiseMeetingSetEvent != null)
            {
                RaiseMeetingSetEvent(this, new EventArgs());
            }
        }

        private bool CanLoadAgenda(forms.TreeView agendaTree)
        {
            return true;
        }

        private void OnLoadAgenda(forms.TreeView agendaTree)
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
                        agendaTree.ShowPlusMinus = false;
                        agendaTree.ShowLines = false;
                        agendaTree.ExpandAll();
                    }

                    OnRaiseMeetingSetEvent();


                }
            }
            catch ( COMException cex)
            {
                ; //ignore it

            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(string.Format("Unable to download file :{0} verify the filename is correct.", fileName));

            }


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
                    _agenda.Items.Add(x);
                    forms.TreeNode xn = new forms.TreeNode() { Text = x.Title };
                    
                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref a, ref xn);
                    }
                    node.Nodes.Add(xn);
                    a.Items.Add(x);
                }
            }
        }

        public Item FindItem(int hashCode)
        {
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
            _loadAgenda = new DelegateCommand<forms.TreeView>(OnLoadAgenda, CanLoadAgenda);
            _agenda = new Agenda();
          
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

        #endregion
    }
}
