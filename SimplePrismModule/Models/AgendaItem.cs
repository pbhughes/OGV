using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OGV.Admin.Models
{
    public class AgendaItem: INotifyPropertyChanged
    {
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

        ObservableCollection<AgendaItem> _items;
        public ObservableCollection<AgendaItem> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        private long _frame;
        public long Frame
        {
            get { return _frame; }
            set { _frame = value; OnPropertyChanged("Frame"); }
        }

        private AgendaItem _selectedItem;
        public AgendaItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; OnPropertyChanged("SelectedItem"); }
        }

        public AgendaItem()
        {
            _items = new ObservableCollection<AgendaItem>();
        }

        public override string ToString()
        {
            XDocument xdoc = XDocument.Parse("<item></item>");
            XElement title = new XElement("title", Title);
            XElement desc = new XElement("desc", Description);
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
