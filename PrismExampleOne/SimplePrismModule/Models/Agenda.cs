using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using OGV.Infrastructure.Interfaces;
using OGV.Admin.Models;

namespace OGV.Infrastructure.Interfaces
{

    public class Agenda : INotifyPropertyChanged, IParent, IChangeable, IAgenda
    {
        public int TotalItems
        {
            get
            {
                int total = 0;
                foreach (var agItem in Items)
                {
                    total++;
                    if (agItem.Items.Count > 0)
                        AddToTotal(agItem, ref total);
                }
                return total;
            }
        }

        private void AddToTotal(IAgendaItem agItem, ref int total)
        {
            foreach (var item in agItem.Items)
            {
                total++;
                if (item.Items.Count > 0)
                    AddToTotal(item, ref total);
            }
        }

        private string _name = @"Agenda Name.oga";

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged("FileName"); OnChanged(); }
        }

        private DateTime _meetingDate;

        public DateTime MeetingDate
        {
            get { return _meetingDate; }
            set { _meetingDate = value; OnPropertyChanged("MeetingDate"); OnChanged(); }
        }

        private string _videoFileName;

        public string VideoFileName
        {
            get { return _videoFileName; }
            set { _videoFileName = value; OnPropertyChanged("VideoFileName"); }
        }

        private string _orignalText;

        public string OriginalText
        {
            get { return _orignalText; }
            set { _orignalText = value; OnPropertyChanged("AllText"); }
        }

        private string _filePath;

        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = value; OnPropertyChanged("FilePath"); }
        }

        public bool SaveNeeded {
            get
            {
                int orignalHash = OriginalText.GetHashCode();
                string current = this.ToString();
                int currenHash = current.GetHashCode();

                return orignalHash != currenHash;
            }
        }

        private ObservableCollection<IAgendaItem> _items;

        public ObservableCollection<IAgendaItem> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        private IAgendaItem _selectedItem;
        public IAgendaItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); OnChanged(); StampCommand.RaiseCanExecuteChanged(); }
        }

        public Agenda()
        {
            this.SaveCommand = new DelegateCommand(OnSave, CanSave);
            this.ResetCommand = new DelegateCommand(OnReset, CanReset);
            this.StampCommand = new DelegateCommand(OnStamp, CanStamp);

            _items = new ObservableCollection<IAgendaItem>();
            AgendaItem level1 = new AgendaItem() { Title = "Top 1" };
            AgendaItem level2 = new AgendaItem() { Title = "Top 2" };
            level1.Items.Add(level2);
        }

        public void OnChanged()
        {
            if (ChangedEvent != null)
                ChangedEvent(this, new EventArgs());

           

        }

        public void Reset()
        {
            try
            {

                XDocument xDoc = XDocument.Parse(OriginalText);
                _items.Clear();
                var allAgendaItems = xDoc.Element("meeting").Element("agenda").Element("items").Elements("item");
                foreach (var itemElement in allAgendaItems)
                {

                    AgendaItem ai = ParseAgendaItem(itemElement);
                    ai.Parent = this;
                    AddItem(ai);

                }

                
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public IAgenda ParseAgenda(FileSystemInfo agenda)
        {
            try
            {
                string allText = File.ReadAllText(agenda.FullName);
                string filePath = agenda.FullName;
                Agenda a = new Agenda() { OriginalText = allText, FilePath = filePath };
                XDocument xDoc = XDocument.Parse(a.OriginalText);
                a.MeetingDate = DateTime.Parse(xDoc.Element("meeting").Element("meetingdate").Value);
                a.Name = agenda.Name;
                var allAgendaItems = xDoc.Element("meeting").Element("agenda").Element("items").Elements("item");
                foreach (var itemElement in allAgendaItems)
                {

                    AgendaItem ai = ParseAgendaItem(itemElement);
                    a.AddItem(ai);
                   
                }

                return a;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static AgendaItem ParseAgendaItem(XElement itemElement)
        {
            AgendaItem ai = new AgendaItem()
            {
                Title = (string)itemElement.Element("title") ?? "",
                Description = (string)itemElement.Element("desc") ?? "",
                Frame = long.Parse((string)itemElement.Element("frame") ?? "0"),
                TimeStamp = TimeSpan.Parse((string)itemElement.Element("timestamp") ?? (new TimeSpan(0, 0, 0)).ToString())
            };

            if (itemElement.Element("timestamp") != null)
                ai.TimeStamp = TimeSpan.Parse(itemElement.Element("timestamp").Value);

            if (itemElement.Element("items") != null)
            {
                foreach (var subItem in itemElement.Element("items").Elements("item"))
                {
                    var subAgendaItem = ParseAgendaItem(subItem);
                    ai.Items.Add(subAgendaItem);
                }
            }
            return ai;
        }

        public override string ToString()
        {
            XDocument xdoc = XDocument.Parse("<meeting></meeting>");
            
            XElement meetingDate = new XElement("meetingdate", MeetingDate.ToString("G"));
            XElement agenda = new XElement("agenda");
            XElement items = new XElement("items");
            XElement videoFilName = new XElement("filename", VideoFileName);
            xdoc.Element("meeting").Add(videoFilName);
            xdoc.Element("meeting").Add(meetingDate);
            xdoc.Element("meeting").Add(agenda);
            
            foreach (var item in Items)
            {
                XElement itemElement = XElement.Parse(item.ToString());
                items.Add(itemElement);
            }
            xdoc.Element("meeting").Element("agenda").Add(items);
            string content = xdoc.ToString();
            return content;            
        }

        void ItemChanged_Event(object sender, EventArgs e)
        {
            OnChanged();
        }

        private TimeSpan _videoTime;
        public TimeSpan VideoTime
        {
            get { return _videoTime; }
            set { _videoTime = value; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));

            OnChanged();
        }

        #endregion INotifyPropertyChanged

        #region IParent Interface 


        public void RemoveItem(IAgendaItem item)
        {
            if (_items.Contains(item))
                _items.Remove(item);
        }

        public void AddItem(IAgendaItem item)
        {
            if (_items == null)
                _items = new ObservableCollection<IAgendaItem>();

            _items.Add(item);
            item.Parent = this;

            item.ChangedEvent += ItemChanged_Event;

            OnChanged();
        }

        public void InsertItem(IAgendaItem item, int indexAt)
        {
            if (_items == null)
            {
                indexAt = 0;
                _items = new ObservableCollection<IAgendaItem>();
            }
            item.Parent = this;

            if (indexAt > _items.Count)
                _items.Add(item);
            else
            _items.Insert(indexAt, item);
            OnChanged();
        }

        public int IndexOf(IAgendaItem item)
        {
            if (_items.Contains(item))
                return _items.IndexOf(item);

            return -1;
        }

        #endregion

        #region IChangable

        public DelegateCommand SaveCommand { get;  set; }

        public DelegateCommand ResetCommand { get;  set; }

        public DelegateCommand StampCommand { get; set; }

        public event ChangedEventHandler ChangedEvent;

        public bool CanSave()
        {
            if (this != null)
            {
                return SaveNeeded;
            }

            return false;
        }

        public void OnSave()
        {
            if (this == null)
                throw new InvalidOperationException("No agenda has been loaded");

            if (string.IsNullOrEmpty(this.FilePath))
                throw new InvalidOperationException("File name has not been set");

            string allText = this.ToString();
            File.WriteAllText(this.FilePath, allText);
            this.OriginalText = allText;

            OnChanged();
        }

        public bool CanReset()
        {
            if (this != null)
            {
                return SaveNeeded;
            }

            return false;
        }

        public void OnReset()
        {
            Reset();
        }

        public bool CanStamp()
        {
            return (VideoTime != null && SelectedItem != null);
        }

        public void OnStamp()
        {
            SelectedItem.TimeStamp = VideoTime;
        }

        #endregion


     

        
    }
}