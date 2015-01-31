using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Admin.Models
{
    public interface IChangeable
    {
        void OnChanged();

        event ChangedEventHandler ChangedEvent;

        DelegateCommand SaveAgendaCommand { get; set; }

        DelegateCommand ResetAgendaCommand { get; set; }

        bool CanSave();

        void OnSave();

        bool CanReset();

        void OnReset();

        bool SaveNeeded { get;  }

        void Reset();
    }
}
