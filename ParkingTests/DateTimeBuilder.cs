using System;
using System.Collections.Generic;
using System.Text;

namespace ParkingTests
{
    class DateTimeBuilder
    {
        public DateTime GetTimeBeforeMinutes(int minutes)
        {
            var now = DateTime.Now;
            var offset = new TimeSpan(0, minutes, 0);
            return now.Subtract(offset);
        }
    }
}
