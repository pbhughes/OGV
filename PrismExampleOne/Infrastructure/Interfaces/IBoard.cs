using System;
namespace OGV.Infrastructure.Interfaces
{
    public interface IBoard
    {
        System.Collections.ObjectModel.ObservableCollection<IAgenda> Agendas { get; set; }
        bool IsBusy { get; set; }
        string Name { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
