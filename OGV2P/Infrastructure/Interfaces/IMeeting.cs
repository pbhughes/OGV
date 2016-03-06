using System;
using Infrastructure.Models;
using System.Windows.Controls;
using forms = System.Windows.Forms;
using System.Deployment;
using System.Collections.Generic;

namespace Infrastructure.Interfaces
{
    public delegate void MeetingSetEventHandler(object sender, EventArgs e);
    public delegate void MeeingItemChangedEventHandler(IItem item);

    public interface IMeeting
    {
        string FileName { get; set; }
        bool IsBusy { get; set; }
        string ApplicationVersion { get; }
        Microsoft.Practices.Prism.Commands.DelegateCommand<forms.TreeView> ClearStampsCommand { get; set; }
        DateTime MeetingDate { get; set; }
        string MeetingName { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        Item SelectedItem { get; set; }
        string ClientPathLive { get; set; }
        string ClientPathLiveStream { get; set; }
        int VideoWidth { get; set; }
        int VideoHeight { get; set; }
        int FrameRate { get; set; }
        string LandingPage { get; set; }
        string PublishingPoint { get;  }
        string LocalFile { get; set; }
        long WriteAgendaFile(forms.TreeView agendaTree);
        long BytesWritten { get; set; }
        string LocalAgendaFileName { get; set; }
        event MeetingSetEventHandler RaiseMeetingSetEvent;
        event MeeingItemChangedEventHandler RaiseMeetingItemChanged;
        void ParseAgendaFile(forms.TreeView tree, string allText);
        string LeftStatus { get; set; }
        string RightStatus { get; set; }
        string GetAgendaXML();
        long StartingHash { get; set; }
        long EndingHash { get; }
        bool HasChanged { get; }
        bool IsLive { get; set; }
    }
}
