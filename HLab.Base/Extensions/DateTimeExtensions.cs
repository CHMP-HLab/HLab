using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLab.Base.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime? ToUniversalTime(this DateTime? dateTime)
        {
            if(dateTime.HasValue) return dateTime.Value.ToUniversalTime();
            return null;
        }
        public static DateTime? ToLocalTime(this DateTime? dateTime)
        {
            if(dateTime.HasValue) return dateTime.Value.ToLocalTime();
            return null;
        }
    }
}
