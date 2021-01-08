using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SharpShares
{
    class Logger
    {
        public enum STATUS
        {
            GOOD = 0,
            ERROR = 1,
            INFO = 2,
            VERBOSE = 3
        };

        public static void Print(STATUS status, String msg, String highlight = "")
        {
            if (status == STATUS.GOOD)
            {
                Console.Write(GetCurrentTime());
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" ==> ");
                Console.ResetColor();
                Console.Write(msg);
                if (highlight != "")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(highlight);
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                }
            }
            else if (status == STATUS.ERROR)
            {
                Console.Write(GetCurrentTime());
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" ==> ");
                Console.ResetColor();
                Console.Write(msg);
                if (highlight != "")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(highlight);
                    Console.ResetColor();

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                }
            }
            else if (status == STATUS.INFO)
            {
                Console.Write(GetCurrentTime());
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" |_ ");
                Console.ResetColor();
                Console.Write(msg);
                if (highlight != "")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(highlight);
                    Console.ResetColor();
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                }
            }
            else if (status == STATUS.VERBOSE)
            {
                if (Helpers.Arguments.Verbose)
                {
                    Console.Write(GetCurrentTime());
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(" ==> ");
                    Console.ResetColor();
                    Console.Write(msg);
                    if (highlight != "")
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(highlight);
                        Console.ResetColor();
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        public static String GetLastWindowsError()
        {
            Win32Exception win32Exception = new Win32Exception(Marshal.GetLastWin32Error());
            return string.Format("{0} ({1})", win32Exception.Message, win32Exception.NativeErrorCode);
        }
        private static String GetCurrentTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
        }
    }
}
