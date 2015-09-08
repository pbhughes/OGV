using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Models;
using Infrastructure.Panopto.Session;
using r = Infrastructure.Panopto.RemoteRecorder;

namespace Infrastructure.Interfaces
{
    

    public interface IUser
    {
        bool IsReady { get; set; }

        string UserID { get; set; }

        string Password {get; set; }

        DelegateCommand LoginCommand { get; set; }

        event LoginEventHandler RaiseLoginEvent;

        string Message { get; set; }

        bool IsBusy { get; set; }

        AuthenticationInfo GetSessionAuthInfo();

        r.AuthenticationInfo GetRemoteRecorderAuthInfo();

        bool IsCameraServiceReady { get; set; }

        bool IsFilterServiceReady { get; set;  }
        
    }
}
