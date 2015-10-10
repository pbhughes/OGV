using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ISession
    {

        Guid RecorderID { get; set; }

        string MeetingName { get; set; }

        string LocalVideoFile { get; set; }

        event MeetingNameSetEventHandler RaiseMeetingNameSet;
    }
}
