using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;

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

        private int _timeStamp;

        public int TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                _timeStamp = value;

                Title = StampTitle(value);

                OnPropertyChanged("TimeStamp");
            }
        }

        private List<Item> _items;

        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        private long _startingHash = 0;

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
            if (Items != null)
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
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion INotifyPropertyChanged

        public string StampTitle(int seconds)
        {
            TimeSpan stamp = new TimeSpan(0, 0, seconds);
            _timeStamp = seconds;
            //see if there is a stamp heading
            string pattern = @"\[\d\d:\d\d:\d\d\]";
            string source = _title;
            Regex regx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = regx.Matches(source);

            if (matches.Count > 0)
            {
                //there is a match so this item has been stamped
                int start = source.IndexOf('[');
                int offSet = source.IndexOf(']');

                source = source.Substring(offSet + 1);
            }

            //the time has not been stamped
            if (stamp == TimeSpan.Zero)
                return source;
            else
                return string.Format("{0}{1}", stamp.ToAgendaTimeString(), source);
        }

        public void UpdateHash()
        {
            StartingHash = (Title + Description).GetHashCode();
            if (Items != null)
            {
                foreach (Item n in Items)
                {
                    n.UpdateHash();
                }
            }
        }

        public Item()
        {
            Items = new List<Item>();
            StartingHash = 0;
        }
    }
}