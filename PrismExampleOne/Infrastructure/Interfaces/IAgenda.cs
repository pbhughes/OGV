using System;
namespace OGV.Infrastructure.Interfaces
{
    public delegate void ChangedEventHandler(object sender, EventArgs e);

    public interface IAgenda
    {
        void AddItem(IAgendaItem item);
        bool CanReset();
        bool CanSave();
        event ChangedEventHandler ChangedEvent;
        string FileName { get; set; }
        string FilePath { get; set; }
        int IndexOf(IAgendaItem item);
        void InsertItem(IAgendaItem item, int indexAt);
        System.Collections.ObjectModel.ObservableCollection<IAgendaItem> Items { get; set; }
        DateTime MeetingDate { get; set; }
        string Name { get; set; }
        void OnChanged();
        void OnReset();
        void OnSave();
        string OriginalText { get; set; }
        IAgenda ParseAgenda(System.IO.FileSystemInfo agenda);
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        void RemoveItem(IAgendaItem item);
        void Reset();
        Microsoft.Practices.Prism.Commands.DelegateCommand ResetCommand { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand SaveCommand { get; set; }
        bool SaveNeeded { get; }
        IAgendaItem SelectedItem { get; set; }
        string ToString();
        int TotalItems { get; }
        string VideoFileName { get; set; }
    }
}
