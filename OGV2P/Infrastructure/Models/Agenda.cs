using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Infrastructure.Models
{
    public class Agenda : INotifyPropertyChanged
    {

        private List<Item> _items;

        public List<Item> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public void UpdateHash()
        {
            if(Items != null)
            {
                foreach(Item n in Items)
                {
                    n.UpdateHash();
                }
            }
        }

        public Agenda()
        {
            _items = new List<Item>();
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
