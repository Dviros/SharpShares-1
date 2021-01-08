using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpShares.Helpers
{
    class Generic
    {
        public static void Jitter()
        {
            if (Arguments.Jitter != 0)
            {
                Int32 jitter = new Random().Next(0, Arguments.Jitter);
                Logger.Print(Logger.STATUS.VERBOSE, "Jitter: ", jitter.ToString());
                Int32 ms = Convert.ToInt32(TimeSpan.FromSeconds(jitter).TotalMilliseconds);
                System.Threading.Thread.Sleep(ms);
            }
        }
        public static List<String> RandomiseList(List<String> randomiseMe)
        {
            var r = new Random();
            return randomiseMe.OrderBy(item => r.Next()).ToList();
        }
        public static String GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }
}