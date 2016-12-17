using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class BoardList : IBoardList, INotifyPropertyChanged
    {
        

        private List<IBoard> _boards;
        public List<IBoard> Boards
        {
            get
            {
                return _boards;
            }

            set
            {
                _boards = value;
                OnPropertyChanged("Boards");
            }
        }

        public void AddBoard(IBoard board)
        {
            if (_boards == null)
                _boards = new List<IBoard>();

            _boards.Add(board);
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
