using System;
using Microsoft.Practices.Prism.Commands;

namespace OGV.Infrastructure.Interfaces
{
    public interface IAgendaItem
    {
        void AddItem(IAgendaItem item);
        bool CanReset();
        bool CanSave();
        event ChangedEventHandler ChangedEvent;
        string Description { get; set; }
        long Frame { get; set; }
        int IndexOf(IAgendaItem item);
        void InsertItem(IAgendaItem item, int indexAt);
        System.Collections.ObjectModel.ObservableCollection<IAgendaItem> Items { get; set; }
        void OnChanged();
        void OnReset();
        void OnSave();
        string OriginalText { get; set; }
        IParent Parent { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        void RemoveItem(IAgendaItem item);
        void Reset();
        DelegateCommand ResetCommand { get; set; }
        DelegateCommand SaveCommand { get; set; }
        bool SaveNeeded { get; }
        IAgendaItem SelectedItem { get; set; }
        TimeSpan TimeStamp { get; set; }
        string Title { get; set; }
        string ToString();
        string Segment { get; set; } 
    }
}
