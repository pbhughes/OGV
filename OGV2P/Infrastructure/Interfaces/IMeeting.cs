using System;
using Infrastructure.Models;
using System.Windows.Controls;
using forms = System.Windows.Forms;

namespace Infrastructure.Interfaces
{
    public delegate void MeetingSetEventHandler(object sender, EventArgs e);

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
        Item FindItem(int hashCode);
        string ClientPathLive { get; set; }
        string ClientPathLiveStream { get; set; }
        int VideoWidth { get; set; }
        int VideoHeight { get; set; }
        int FrameRate { get; set; }
        string LandingPage { get; set; }
        string PublishingPoint { get;  }
        string LocalFile { get; set; }


        event MeetingSetEventHandler RaiseMeetingSetEvent;
    }
}
