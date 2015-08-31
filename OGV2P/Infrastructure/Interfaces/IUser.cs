using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Infrastructure.Models;

namespace Infrastructure.Interfaces
{
    

    public interface IUser
    {
        string UserID { get; set; }

        string Password {get; set; }

        DelegateCommand LoginCommand { get; set; }

        event LoginEventHandler RaiseLoginEvent;

        string Message { get; set; }

        bool IsBusy { get; set; }
        
    }
}
