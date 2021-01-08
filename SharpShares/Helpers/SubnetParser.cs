using LukeSkywalker.IPNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SharpShares.Helpers
{
    class SubnetParser
    {
        public static List<String> GetScope(string arg)
        {
            List<String> scope = new List<string>();
            List<String> targets = new List<string>();
            try
            {
                string[] lines = File.ReadAllLines(arg);
                scope = new List<String>(lines);
                return scope;
            }
            catch
            {

            }
            if (arg.Contains("-"))
            {
                targets = IPDashSeperated(arg);
                scope.AddRange(targets);
            }
            else if (arg.Contains(","))
            {
                string[] split = arg.Split(',');
                if (split.Length > 2)
                {
                    return targets;
                }
                targets = RemoveWhiteSpace(split);
                foreach (string target in targets)
                {
                    if (target.Contains("/"))
                    {
                        scope.AddRange(ListFromSubnet(target));
                    }
                }
                scope.AddRange(targets);
            }
            else
            {
                if (arg.Contains("/"))
                {
                    scope = ListFromSubnet(arg);
                }
                else
                {
                    if (!String.IsNullOrEmpty(arg))
                    {
                        scope.Add(arg);
                    }
                }
            }

            for (int i = 0; i < scope.Count; i++)
            {
                scope[i] = scope[i].Replace("\"", "");
            }

            if (Arguments.Randomise)
            {
                return Generic.RandomiseList(scope);
            }
            else
            {
                return scope;
            }
        }
        private static List<String> IPDashSeperated(string Scope)
        {
            List<String> IPs = new List<String>();
            string[] ScopeSplit = Scope.Split('-');

            String StartIP = ScopeSplit[0]; // Start IP
            String endIP = ScopeSplit[1];
            string[] StartIPOctets = StartIP.Split('.'); // splits: 192.168.0.1 on the dot
            string[] EndIPOctets = endIP.Split('.'); // splits: 192.168.0.1 on the dot

            string[] FirstThreeOctetsOfStartIP = StartIPOctets.Take(StartIPOctets.Count() - 1).ToArray(); // string[] { 192, 168, 0 };
            int StartEndOctet = Int32.Parse(StartIPOctets[StartIPOctets.Length - 1]);  // takes the final octet of the starting ip: .1 from 192.168.0.1

            int finalOctetOfEndIP = Int32.Parse(EndIPOctets[EndIPOctets.Length - 1]); // takes the - [whatever the top of the range is]

            for (int i = StartEndOctet; i <= finalOctetOfEndIP; i++)
            {
                String ip = String.Join(".", FirstThreeOctetsOfStartIP) + "." + i;
                if (!IPs.Contains(ip))
                {
                    IPs.Add(ip);
                }
            }
            return IPs;
        }
        private static List<String> RemoveWhiteSpace(string[] split)
        {
            List<String> parsed = new List<string>();
            try
            {
                foreach (string s in split)
                {
                    String x = Regex.Replace(s, @"\s+", "").ToString();
                    if (!parsed.Contains(x))
                    {
                        parsed.Add(x);
                    }
                }
            }
            catch
            {

            }
            return parsed;
        }

        public static List<String> ListFromSubnet(string arg)
        {
            arg = arg.Replace("\"", "");
            List<String> listOfIps = new List<String>();
            IPNetwork ipn = IPNetwork.Parse(arg);
            IPAddressCollection ips = IPNetwork.ListIPAddress(ipn);
            foreach (IPAddress i in ips)
            {
                string x = i.ToString();
                if (!listOfIps.Contains(x))
                {
                    if (!x.EndsWith(".0") && !x.EndsWith(".255"))
                    {
                        listOfIps.Add(x);
                    }
                }
            }
            return listOfIps;
        }
    }
}
