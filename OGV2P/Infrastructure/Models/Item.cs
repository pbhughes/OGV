using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Infrastructure.Interfaces;

namespace Infrastructure.Models
{
    public class Item : INotifyPropertyChanged, IItem
    {
        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged("ID"); }
        }

        private Item _parent;

        public Item Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged("Parent"); }
        }

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
            set {
                _timeStamp = value;
                string formatted = string.Format("{0}:{1}:{2}", 
                    value.Hours.ToString().PadLeft(2,'0'), 
                    value.Minutes.ToString().PadLeft(2, '0'),
                    value.Seconds.ToString().PadLeft(2, '0'));
                Title = StampTitle(formatted);
                OnItemChanged();
                OnPropertyChanged("TimeStamp");
            }
        }

        private List<Item> _items;

        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        long _startingHash = 0;
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

        public bool HasChanges
        {
            get
            {
                long currentHash = (Title + Description).GetHashCode();
                return (currentHash == StartingHash);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(Title + Description);
            if(Items != null)
            {
                foreach (Item n in Items)
                {
                    sb.Append(n.ToString());
                }
            }
            

            return sb.ToString();
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;


       
        private void OnPropertyChanged(string name)
        {

            OnItemChanged();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

       

        #endregion

        #region Item Changed Event Support

        public event ItemChangedEventHandler ItemChangedEvent;

        private void OnItemChanged()
        {
            if (ItemChangedEvent != null)
                ItemChangedEvent(this);
        }
        #endregion

        private string StampTitle(string stamp)
        {
            //see if there is a stamp heading
            string pattern = @"\[\d\d:\d\d:\d\d\]";
            string source = _title;
            Regex regx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regx.Matches(source);
            if(matches.Count > 0)
            {
                //there is a match so this item has been stamped
                int start = source.IndexOf('[');
                int offset = source.IndexOf(']');
                source = source.Substring(offset + 1);
            }

            return string.Format("[{0}]{1}", stamp, source);
        }

        public void UpdateHash()
        {
            StartingHash = (Title + Description).GetHashCode();
            if(Items != null)
            {
                foreach(Item n in Items)
                {
                    n.UpdateHash();
                }
            }
        }

        public Item()
        {
            StartingHash = 0;
        }
    }
}
