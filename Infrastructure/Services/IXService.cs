using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Infrastructure.Services
{
    public interface IXService
    {
        event EventHandler Close;
        void RequestClose();
        event EventHandler Save;
        void RequestSave();
        event EventHandler Back;
        void RequestBack();

        

    }
}
