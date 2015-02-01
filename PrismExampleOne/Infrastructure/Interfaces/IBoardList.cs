using System;
namespace OGV.Infrastructure.Interfaces
{
    public interface IBoardList
    {
        Microsoft.Practices.Prism.Commands.DelegateCommand AddNodeCommand { get; }
        System.Collections.ObjectModel.ObservableCollection<IBoard> Boards { get; set; }
        event EventHandler CanExecuteChanged;
        Microsoft.Practices.Prism.Commands.DelegateCommand ChooseAgendaCommand { get; }
        bool IsBusy { get; set; }
        void Load();
        Microsoft.Practices.Prism.Commands.DelegateCommand LoadAgendaCommand { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand LogOutCommand { get; }
        IAgenda ParseAgenda(System.IO.FileSystemInfo agenda);
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        Microsoft.Practices.Prism.Commands.DelegateCommand<IAgendaItem> RemoveNodeCommand { get; }
        void SaveAgenda();
        IAgenda SelectedAgenda { get; set; }
        IBoard SelectedBoard { get; set; }
    }
}
