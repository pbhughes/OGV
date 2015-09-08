using Infrastructure.Models;
using s = Infrastructure.Panopto.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISession
    {
        s.Session CurrentSession { get; set; }

        Guid RecorderID { get; set; }

        string MeetingName { get; set; }

        event MeetingNameSetEventHandler RaiseMeetingNameSet;
    }
}
