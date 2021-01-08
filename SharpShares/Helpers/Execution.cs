using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace SharpShares.Helpers
{
    class Execution
    {
        public static void Execute(List<String> targets, String username, String domain, String password)
        {
            if (String.IsNullOrEmpty(domain) && String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
            {
                Logger.Print(Logger.STATUS.GOOD, String.Format("Request shares on {0} host(s) as NULL", targets.Count));
                Console.WriteLine();
                foreach (String target in targets)
                {
                    if (Arguments.Jitter != 0)
                    {
                        Generic.Jitter();
                    }
                    CheckShare(target);
                }
            }
            else if (!String.IsNullOrEmpty(domain) && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                Logger.Print(Logger.STATUS.GOOD, String.Format("Request shares on {0} host(s) as {1}", targets.Count, domain + "\\" + username));
                Console.WriteLine();
                IntPtr hToken = Network.Authenticate(username, domain, password);
                if (hToken != IntPtr.Zero)
                {
                    using (WindowsImpersonationContext context = WindowsIdentity.Impersonate(hToken))
                    {
                        foreach (String target in targets)
                        {
                            Generic.Jitter();
                            CheckShare(target);
                        }
                    }
                    PInvoke.NativeMethods.CloseHandle(hToken);
                }
            }
            Console.WriteLine();
            return;
        }
        private static void CheckShare(string target)
        {
            if (!Helpers.Arguments.Grep)
            {
                Logger.Print(Logger.STATUS.GOOD, String.Format("Shares ({0}): ", target));
            }
            List<String> shares = Network.EnumNetShares(target);
            ParseShares(target, shares);
            return;
        }
        private static void ParseShares(String target, List<String> shares)
        {
            if (shares.Count == 0)
            {
                Logger.Print(Logger.STATUS.INFO, "No shares!");
            }
            else
            {
                if (!Helpers.Arguments.Grep)
                {
                    foreach (String share in shares)
                    {
                        Logger.Print(Logger.STATUS.INFO, share);
                    }
                }
                else
                {
                    Logger.Print(Logger.STATUS.INFO, String.Format("{0}: {1}", target, String.Join(", ", shares)));
                }
            }
            return;
        }


    }
}
