using System;
using Infrastructure.Models;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using System.Deployment;

namespace Infrastructure.Interfaces
{
    public delegate void MeetingSetEventHandler(object sender, EventArgs e);

    public interface IMeeting
    {
        Agenda MeetingAgenda { get; set; }
        string FileName { get; set; }
        bool IsBusy { get; set; }
        string ApplicationVersion { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> LoadAgendaFromFTP { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> LoadAgendaFromFile { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> CreateNewAgenda { get; set; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> SaveAgendaFile { get; set; }
        DateTime MeetingDate { get; set; }
        string MeetingName { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        Item SelectedItem { get; set; }
        Item FindItem(string title);
        string ClientPathLive { get; set; }
        string ClientPathLiveStream { get; set; }
        int VideoWidth { get; set; }
        int VideoHeight { get; set; }
        int FrameRate { get; set; }
        string LandingPage { get; set; }
        string PublishingPoint { get;  }
        string LocalFile { get; set; }
        void  AddNode(Item item);
        long WriteAgendaFile(forms.TreeView agendaTree);
        long BytesWritten { get; set; }
        string LocalAgendaFileName { get; set; }
        event MeetingSetEventHandler RaiseMeetingSetEvent;
    }
}
