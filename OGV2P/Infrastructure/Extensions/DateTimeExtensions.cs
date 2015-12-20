using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToFileNameComponent( this DateTime targetDate)
        {
            return targetDate.ToString("mm-dd-yyyy");
        }
    }
}
