using System;
using Microsoft.Practices.Prism.Commands;
namespace OGV.Infrastructure.Interfaces
{
    public delegate void AgendaSelectedDelegate( IAgenda selected );

    public interface IBoardList
    {
        DelegateCommand AddNodeCommand { get; }
        System.Collections.ObjectModel.ObservableCollection<IBoard> Boards { get; set; }
        event EventHandler CanExecuteChanged;
        DelegateCommand ChooseAgendaCommand { get; }
        bool IsBusy { get; set; }
        void Load();
        DelegateCommand LoadAgendaCommand { get; }
        DelegateCommand LogOutCommand { get; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        event AgendaSelectedDelegate AgendaSelectedEvent;
        void OnAgendaSelected(IAgenda sender);
        DelegateCommand<IAgendaItem> RemoveNodeCommand { get; }
        void SaveAgenda();
        IAgenda SelectedAgenda { get; set; }
        IBoard SelectedBoard { get; set; }
    }
}
