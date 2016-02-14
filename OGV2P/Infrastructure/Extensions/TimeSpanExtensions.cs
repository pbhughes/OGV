using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class TimeSpanExtensions
    {

        public static string ToAgendaTimeString(this TimeSpan value)
        {
            return string.Format("[{0}:{1}:{2}]", value.Hours.ToString().PadLeft(2, '0'),
                                                 value.Minutes.ToString().PadLeft(2, '0'),
                                                 value.Seconds.ToString().PadLeft(2, '0')
                                                 );
        }
    }
}
