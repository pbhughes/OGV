using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISaveAgendaViewModel
    {
        IMeeting Meeting { get; set; }
        IUser User { get; set; }
        bool IsBusy { get; set; }
        void SaveAgenda();
    }
}
