using System;
using Microsoft.Practices.Prism.Commands;

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
        string PublishingPoint { get; set; }
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
        IAgenda ParseAgenda(string filePath);
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        void RemoveItem(IAgendaItem item);
        void Reset();
        DelegateCommand ResetCommand { get; set; }
        DelegateCommand SaveCommand { get; set; }
        DelegateCommand AssociateVideoCommand { get; set; }
        TimeSpan VideoTime { get; set; }
        bool SaveNeeded { get; }
        IAgendaItem SelectedItem { get; set; }
        string ToString();
        int TotalItems { get; }
        string VideoFileName { get; set; }
        string VideoFilePath { get; set; }
        string CurrentSegment { get; set; }
        bool IsRecording { get; set; }
    }
}
