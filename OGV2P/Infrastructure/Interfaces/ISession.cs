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

        string LocalVideoFile { get; set; }

        TimeSpan CurrentVideoTime { get; set; }

        void Stamp();

        void LogOut();

        event MeetingNameSetEventHandler RaiseMeetingNameSet;

        event StampedEventHandler RaiseStamped;

        event LoggedOutEventHandler RaiseLoggedOut;

        DateTime InitializationTime { get; set; }


    }
}
