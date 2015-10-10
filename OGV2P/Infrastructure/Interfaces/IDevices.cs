using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    public interface IDevices : IShowActivity
    {
        List<string> Cameras { get; set; }
        List<string> Microphones { get; set; }
        DelegateCommand GetFolders { get; }
        DelegateCommand GetRecorders { get;  }
        DelegateCommand StartRecording { get;  }
        DelegateCommand StopRecording { get;  }
        Guid CurrentSessionGuid { get; }
    }
}
