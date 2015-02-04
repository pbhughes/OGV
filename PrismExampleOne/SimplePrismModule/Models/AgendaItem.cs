using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OGV.Infrastructure.Interfaces;

namespace OGV.Admin.Models
{
    public delegate void AgendaItemChangedEventHandler(object sender, EventArgs e);

    public class AgendaItem: INotifyPropertyChanged, IParent, IChangeable, IAgendaItem
    {
        public string OriginalText { get; set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged("Title"); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; OnPropertyChanged("Description"); }
        }

        private TimeSpan _timeStamp;
        public TimeSpan TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; OnPropertyChanged("TimeStamp"); }
        }

        ObservableCollection<IAgendaItem> _items;
        public ObservableCollection<IAgendaItem> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        private IParent _parent;
        public IParent Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); OnChanged(); }
        }

        private long _frame;
        public long Frame
        {
            get { return _frame; }
            set { _frame = value; OnPropertyChanged("Frame"); }
        }

        private IAgendaItem _selectedItem;
        public IAgendaItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        public AgendaItem()
        {
            this.SaveCommand = new DelegateCommand(OnSave, CanSave);
            this.ResetCommand = new DelegateCommand(OnReset, CanReset);
            _items = new ObservableCollection<IAgendaItem>();
        }

        void ItemChanged_Event(object sender, EventArgs e)
        {
            OnChanged();
        }

        public override string ToString()
        {
            XDocument xdoc = XDocument.Parse("<item></item>");
            XElement title = new XElement("title", Title);
            XElement desc = new XElement("desc", Description);
            XElement segment = new XElement("segment", Segment);
            XElement timeStamp = new XElement("timestamp", TimeStamp.ToString());
            XElement items = new XElement("items");
            
            xdoc.Element("item").Add(title);
            xdoc.Element("item").Add(desc);
            xdoc.Element("item").Add(timeStamp);
            foreach (var item in Items)
            {
                XElement itemElement = XElement.Parse(item.ToString());
                items.Add(itemElement);
            }

            xdoc.Element("item").Add(items);
            string content = xdoc.ToString();
            return content;
        }

        private void ResetButtons()
        {
            SaveCommand.RaiseCanExecuteChanged();
            ResetCommand.RaiseCanExecuteChanged();
        }

        private void ParseAgendaItem(string text)
        {
            XDocument xDoc = XDocument.Parse(text);


            Title = (string)xDoc.Element("item").Element("title") ?? "";
            Description = (string)xDoc.Element("item").Element("desc") ?? "";
            Frame = long.Parse((string)xDoc.Element("item").Element("frame") ?? "0");
            TimeStamp = TimeSpan.Parse((string)xDoc.Element("item").Element("timestamp") ?? (new TimeSpan(0, 0, 0)).ToString());
            Segment = (string)xDoc.Element("item").Element("segment") ?? "";

            if (xDoc.Element("item").Element("timestamp") != null)
                TimeStamp = TimeSpan.Parse(xDoc.Element("item").Element("timestamp").Value);

            foreach(IAgendaItem item in _items){
                item.Reset();
            }
            
        }

        private string _segment;
        public string Segment
        {
            get
            {
                return _segment;
            }
            set
            {
                if (value != _segment)
                {
                    
                    _segment = value;
                    OnPropertyChanged("Segment");
                    OnChanged();
                }

            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));

            OnChanged();
        }

        #endregion

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

            item.Parent = this;
            item.OriginalText = item.ToString();
            _items.Add(item);

          
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



        #region IChangeble

        public DelegateCommand SaveCommand { get; set; }

        public DelegateCommand ResetCommand { get; set; }

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
                throw new InvalidOperationException("Agenda has not been loaded");

            string allText = this.ToString();
            OriginalText = allText;

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

        public bool SaveNeeded
        {
            get
            {
                if (!string.IsNullOrEmpty(OriginalText))
                {
                    int orignalHash = OriginalText.GetHashCode();
                    string current = this.ToString();
                    int currenHash = current.GetHashCode();

                    return orignalHash != currenHash;
                }

                return false;
                
            }
        }

        public void Reset()
        {
            ParseAgendaItem(this.OriginalText);
        }

        public void OnChanged()
        {
            if (ChangedEvent != null)
                ChangedEvent(this, new EventArgs());

            ResetButtons();
        }

        #endregion





       
    }
}
