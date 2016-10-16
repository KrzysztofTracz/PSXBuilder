using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework
{
    public interface IDeviceLog
    {
        void WriteLine(String text);
    }

    public class DefaultDeviceLog : IDeviceLog
    {
        public void WriteLine(string text)
        {
            var timestamp = DateTime.Now;
            Console.WriteLine("[{0}:{1}:{2}|{3}/{4}/{5}] >> {6}",
                              timestamp.TimeOfDay.Hours,
                              timestamp.TimeOfDay.Minutes,
                              timestamp.TimeOfDay.Seconds,
                              timestamp.Date.Day,
                              timestamp.Date.Month,
                              timestamp.Date.Year,
                              text);
        }
    }
}
