using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Session : ISession
    {
        private Guid _currentSession;
        public Guid CurrentSession
        {
            get { return _currentSession; }
            set { _currentSession = value; }
        }

        private Guid _recoderID;
        public Guid RecorderID
        {
            get { return _recoderID; }
            set { _recoderID = value; }
        }

        public Session()
        {

        }

    }
}
