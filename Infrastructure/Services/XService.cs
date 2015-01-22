using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OGV.Infrastructure.Services
{
    public class XService: IXService
    {

        public event EventHandler Close;

        public void RequestClose()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Save;

        public void RequestSave()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Back;

        public void RequestBack()
        {
            throw new NotImplementedException();
        }
    }
}
