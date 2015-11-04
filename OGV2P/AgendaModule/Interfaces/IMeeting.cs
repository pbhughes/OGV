using System;
using OGV2P.AgendaModule.Models;
using System.Windows.Forms;

namespace OGV2P.AgendaModule.Interfaces
{
    public interface IMeeting
    {
        Agenda MeetingAgenda { get; set; }
        string FileName { get; set; }
        bool IsBusy { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<TreeView> LoadAgenda { get; set; }
        DateTime MeetingDate { get; set; }
        string MeetingName { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        Item SelectedItem { get; set; }
        void FindItem(int hashCode);
    }
}
