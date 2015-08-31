using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISession
    {
        Guid CurrentSession { get; set; }

        Guid RecorderID { get; set; }
    }
}
