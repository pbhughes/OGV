using System;
using Infrastructure.Models;
using System.Windows.Controls;
using forms = System.Windows.Forms;

namespace Infrastructure.Interfaces
{
    public interface IMeeting
    {
        Agenda MeetingAgenda { get; set; }
        string FileName { get; set; }
        bool IsBusy { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> LoadAgenda { get; set; }
        DateTime MeetingDate { get; set; }
        string MeetingName { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        Item SelectedItem { get; set; }
        void FindItem(int hashCode);
        string ClientPathLive { get; set; }
        string ClientPathLiveStream { get; set; }
    }
}
