using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using OGV2P.AgendaModule.Interfaces;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using Infrastructure.Interfaces;


namespace OGV2P.AgendaModule.Models
{
    public class Meeting : INotifyPropertyChanged, IMeeting
    {

        private int _orginalHash;
        private ISession _sessionService;

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

        private DelegateCommand<TreeView> _loadAgenda;
        public DelegateCommand<TreeView> LoadAgenda
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

        private bool CanLoadAgenda(TreeView agendaTree)
        {
            return true;
        }

        private void OnLoadAgenda(TreeView agendaTree)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "JPEG Files (*.xml)|*.xml";

            //set the starting directory
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                FileName = dlg.FileName;

                //parse the agenda file
                string allText = File.ReadAllText(FileName);
                _orginalHash = allText.GetHashCode();

                XDocument xDoc = XDocument.Parse(allText);
                MeetingName = xDoc.Element("meeting").Element("meetingname").Value;
               

                MeetingDate = (xDoc.Element("meeting").Element("meetingdate") != null) ?
                    DateTime.Parse(xDoc.Element("meeting").Element("meetingdate").Value) :
                    DateTime.Now;
                

                XElement items = xDoc.Element("meeting").Element("agenda").Element("items");
                if (items != null)
                {
                    TreeNode root = new TreeNode();
                    Agenda a = new Agenda();
                    ParseItems(items, ref a, ref root);
                    foreach(TreeNode x in root.Nodes){
                        agendaTree.Nodes.Add(x);
                    }
                    agendaTree.ShowPlusMinus = false;
                    agendaTree.ShowLines = false;
                    agendaTree.ExpandAll();
                }
            }

        }

        private void ParseItems(XElement items, ref Agenda a, ref TreeNode node)
        {
            if (items != null)
            {
                foreach (XElement item in items.Elements("item"))
                {
                    Item x = new Item();
                    x.Title = (item.Element("title") != null) ? item.Element("title").Value : null;
                    x.Description = (item.Element("desc") != null) ? item.Element("desc").Value : null;
                    _agenda.Items.Add(x);
                    TreeNode xn = new TreeNode() { Text = x.Title };
                    
                    if (item.Element("items") == null || item.Element("items").Elements("item") != null)
                    {
                        ParseItems(item.Element("items"), ref a, ref xn);
                    }
                    node.Nodes.Add(xn);
                    a.Items.Add(x);
                }
            }
        }

        public void FindItem(int hashCode)
        {
            foreach (Item i in this.MeetingAgenda.Items)
            {
                if (i.Title.GetHashCode() == hashCode)
                    SelectedItem = i;
            }
        }


        public Meeting(ISession sessionService)
        {
            _sessionService = sessionService;
            _sessionService.RaiseStamped += _sessionService_RaiseStamped;
            _loadAgenda = new DelegateCommand<TreeView>(OnLoadAgenda, CanLoadAgenda);
            _agenda = new Agenda();
        }

        private void _sessionService_RaiseStamped(TimeSpan sessionTime)
        {
            _selectedItem.TimeStamp = sessionTime;
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
