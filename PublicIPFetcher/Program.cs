﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PublicIPFetcher
{
    class Program
    {

        //  Methods
        public static string publicIP
        {
            get
            {
                string result;

                try
                {
                    string url = "http://checkip.dyndns.org";
                    System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                    System.Net.WebResponse resp = req.GetResponse();
                    System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                    string response = sr.ReadToEnd().Trim();
                    sr.Close();
                    string[] a = response.Split(':');
                    string a2 = a[1].Substring(1);
                    string[] a3 = a2.Split('<');
                    string a4 = a3[0];
                    result = a4;
                }
                catch (Exception Ex)
                {
                    result = "Unable to retrieve Public IP because - " + Ex.Message;
                }

                return result;
            }
        }
        public static Boolean stringIsIPAddress(string ipString)
        {
            System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(ipString, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
            if (match.Success)
            {
                return true;
            }
            else return false;
        }
        public static string Output1(string IP)
        {
            string result = "win";
            string saveLocation = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\SkyDrive\";
            if (System.IO.Directory.Exists(saveLocation))
            {
                try
                {

                    System.IO.StreamWriter sr = new System.IO.StreamWriter(saveLocation + "PubIP-" + Environment.MachineName + ".txt");
                    sr.WriteLine(IP);
                    sr.Flush();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            else result = "SkyDrive directory not found - " + saveLocation;

            return result;

        }


        static void Main(string[] args)
        {
            LogMaker logger = new LogMaker("IP-Fetcher");
            logger.Write("Program Starting");
            string newIP = publicIP;
            logger.Write("IP pull attempt complete, result      - " + newIP);
            string FirstTry = Output1(newIP);
            logger.Write("First output attempt complete, result - " + FirstTry);

            if (FirstTry != "win")
            {
 
            }

            logger.SafeClose();
        }
    }
}