using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Logging;

namespace PrismExampleOne
{
    class CallbackLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            throw new NotImplementedException();
        }
    }
}
