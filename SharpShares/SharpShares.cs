using System;
using System.Collections.Generic;

namespace SharpShares
{
    public class SharpShares
    {
        public static void Main(string[] args)
        {
            Utilities.Banner.Show();
            if (args.Length == 0)
            {
                Helpers.Arguments.Help();
                return;
            }
            else
            {
                if (args[0] == "-h" || args[0] == "--help" || args[0] == "-help")
                {
                    Helpers.Arguments.Help();
                    return;
                }
            }

            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (Helpers.Arguments.ParseArguments(args))
            {
                List<String> targets = Helpers.SubnetParser.GetScope(Helpers.Arguments.Targets);

                if (targets.Count == 0)
                {
                    Logger.Print(Logger.STATUS.ERROR, "No targets supplied!");
                    return;
                }
                else if (targets.Count > 0)
                {
                    Logger.Print(Logger.STATUS.GOOD, "Total targets: ", targets.Count.ToString());
                }

                if (Helpers.Arguments.Jitter != 0)
                {
                    Logger.Print(Logger.STATUS.GOOD, "Random jitter between: ", String.Format("0 - {0}(s)", Helpers.Arguments.Jitter));
                }

                Logger.Print(Logger.STATUS.GOOD, "TCP: ", Helpers.Arguments.TCP.ToString());
                Logger.Print(Logger.STATUS.GOOD, "Verbose: ", Helpers.Arguments.Verbose.ToString());
                Logger.Print(Logger.STATUS.GOOD, "Grep: ", Helpers.Arguments.Grep.ToString());
                Logger.Print(Logger.STATUS.GOOD, "List: ", Helpers.Arguments.ACL.ToString());
                Logger.Print(Logger.STATUS.GOOD, "Randomise: ", Helpers.Arguments.Randomise.ToString());

                Console.WriteLine();

                string domain = Helpers.Arguments.Domain;
                string username = Helpers.Arguments.Username;
                string password = Helpers.Arguments.Password;

                Logger.Print(Logger.STATUS.GOOD, "Credentials:");

                if (String.IsNullOrEmpty(domain) && String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
                {
                    Logger.Print(Logger.STATUS.INFO, "Username: ", "NULL");
                    Logger.Print(Logger.STATUS.INFO, "Password: ", "NULL");
                }
                else if (!String.IsNullOrEmpty(domain) && !String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                {
                    Logger.Print(Logger.STATUS.INFO, "Username: ", domain + "\\" + username);
                    Logger.Print(Logger.STATUS.INFO, "Password: ", password);
                }
                else
                {
                    return;
                }
                Console.WriteLine();

                if (Helpers.Arguments.TCP)
                {
                    List<String> targetsWithSmb = Helpers.Network.GetTargetsWithOpenPorts(targets);
                    if (targetsWithSmb.Count != 0)
                    {
                        Logger.Print(Logger.STATUS.GOOD, "Targets with SMB Open: ", targetsWithSmb.Count.ToString());
                        Console.WriteLine();
                        Helpers.Execution.Execute(targetsWithSmb, username, domain, password);
                    }
                    else
                    {
                        Logger.Print(Logger.STATUS.ERROR, "No targets with SMB open were found");
                    }
                }
                else
                {
                    Helpers.Execution.Execute(targets, username, domain, password);
                }
            }

            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Logger.Print(Logger.STATUS.GOOD, "Done!");
            Logger.Print(Logger.STATUS.INFO, "Execution time: ", Convert.ToInt32(elapsedMs) + "ms");
        }
    }
}
