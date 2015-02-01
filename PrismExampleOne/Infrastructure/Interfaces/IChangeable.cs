using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Infrastructure.Interfaces
{
    public interface IChangeable
    {
        void OnChanged();

        event ChangedEventHandler ChangedEvent;

        DelegateCommand SaveCommand { get; set; }

        DelegateCommand ResetCommand { get; set; }

        bool CanSave();

        void OnSave();

        bool CanReset();

        void OnReset();

        bool SaveNeeded { get;  }

        void Reset();

       
    }
}
