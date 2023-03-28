using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRapPhim.Service
{
    public static class Service
    {
        public static int getDateSubtract(DateTime start, DateTime end) {

            TimeSpan span = end.Subtract(start);
            return span.Days;
        }
    }
}