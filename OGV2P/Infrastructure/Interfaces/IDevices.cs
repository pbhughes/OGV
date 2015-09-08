using System;
using System.Collections;
using System.Collections.Generic;
using DirectX.Capture;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Panopto.RemoteRecorder;
using Infrastructure.Models;
using Infrastructure.Panopto.Session;

namespace Infrastructure.Interfaces
{
    public interface IDevices : IShowActivity
    {
        List<string> Cameras { get; set; }
        List<string> Microphones { get; set; }
        RemoteRecorder[] RemoteRecorders { get; set; }
        DelegateCommand GetFolders { get; }
        DelegateCommand GetRecorders { get;  }
        DelegateCommand StartRecording { get;  }
        DelegateCommand StopRecording { get;  }
        RemoteRecorder CurrentRecorder { get; set; }
        Folder[] FolderList { get; set; }
        Folder CurrentFolder { get; set; }
        ScheduledRecordingResult CurrentSession { get;  }
        Guid CurrentSessionGuid { get; }
    }
}
