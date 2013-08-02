using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PublicIPFetcher
{
    class Program
    {

        public const string AppName = "Public IP Fetcher";
        public static string DataDir
        {
            get
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Molini\" + AppName;
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return path;
            }
        }
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
        public static Boolean IPisNew(string ipString)
        {
            if (System.IO.File.Exists(DataDir + @"\LastIP.txt"))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(DataDir + @"\LastIP.txt");
                string oldIPval = sr.ReadToEnd();
                sr.Close();
                if (oldIPval == ipString) return true;
                else return false;
            }
            else return true;
        }
        public static string Output0(string IP)
        {
            //  Get email connection parameters from email.config file in user data directory
            if (System.IO.File.Exists(DataDir + @"\email.config"))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(DataDir + @"\email.config");
                string emailConfigs = sr.ReadToEnd();
                sr.Close();

                //Console.Write(emailConfigs);

                System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(
                    @"(?<key>[\w+]*)\s*=\s*(?<value>.+)", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.MatchCollection matches = re.Matches(emailConfigs);

                string mailServer       = "";
                string mailServerPort   = "25";
                string mailUserName     = "";
                string mailPassword     = "";
                string mailFromName     = "";
                string mailTo           = "";

                foreach (System.Text.RegularExpressions.Match m in matches)
                {
                    if (m.Groups["key"].Value.ToUpper() == "server".ToUpper()) mailServer               = m.Groups["value"].Value.Replace("\r", "");
                    else if (m.Groups["key"].Value.ToUpper() == "serverPort".ToUpper()) mailServerPort  = m.Groups["value"].Value.Replace("\r", "");
                    else if (m.Groups["key"].Value.ToUpper() == "username".ToUpper()) mailUserName      = m.Groups["value"].Value.Replace("\r", "");
                    else if (m.Groups["key"].Value.ToUpper() == "password".ToUpper()) mailPassword      = m.Groups["value"].Value.Replace("\r", "");
                    else if (m.Groups["key"].Value.ToUpper() == "FromName".ToUpper()) mailFromName      = m.Groups["value"].Value.Replace("\r", "");
                    else if (m.Groups["key"].Value.ToUpper() == "ToAddress".ToUpper()) mailTo           = m.Groups["value"].Value.Replace("\r", "");
                }
                //if (mailServer.Length < 1 || mailUserName.Length < 1 || mailPassword.Length < 1) 
                //    return "Incompatible email server configuration values";

                Console.WriteLine(mailServer);
                Console.WriteLine(mailServerPort);
                Console.WriteLine(mailUserName);
                Console.WriteLine(mailPassword);
                Console.WriteLine(mailFromName);
                Console.WriteLine(mailTo);

                try
                {
                    System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient(mailServer, Int32.Parse(mailServerPort));
                    mailClient.UseDefaultCredentials = false;
                    mailClient.Credentials = new NetworkCredential(mailUserName, mailPassword);
                    mailClient.EnableSsl = false;
                    Console.WriteLine("mailClient initiated");


                    System.Net.Mail.MailAddress from = new System.Net.Mail.MailAddress(mailUserName, mailFromName);
                    System.Net.Mail.MailAddress to = new System.Net.Mail.MailAddress(mailTo);

                    System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(from, to);
                    message.Subject = "IP address change";
                    message.SubjectEncoding = System.Text.Encoding.Unicode;
                    message.IsBodyHtml = false;
                    message.BodyEncoding = Encoding.Unicode;
                    message.Body =
                        "This email is automatically generated by Sunny's IP Fetcher.\n" +
                        "The IP has just changed and it is now:\n" +
                        IP;
                    Console.WriteLine("message constructed, sending...");
                    mailClient.Send(message);
                }
                catch (Exception Ex)
                {
                    return Ex.ToString() + " - " + Ex.Message;
                }

                return "winning";
            }
            else return "email configuration file not found";
        }
        public static string Output1(string IP, string DataDirectory)
        {
            string result = "win";
            if (System.IO.Directory.Exists(DataDirectory))
            {
                try
                {

                    System.IO.StreamWriter sr = new System.IO.StreamWriter(DataDirectory + @"\LastIP.txt");
                    sr.WriteLine(IP);
                    sr.Flush();
                    sr.Close();
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            else result = "Primary application data directory not found - " + DataDirectory;

            return result; 
        }
        public static string Output2(string IP)
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
            LogMaker logger = new LogMaker(AppName,DataDir);
            logger.Write("Program Starting");
            
            //  Find out what the current IP is
            string newIP = publicIP;
            logger.Write(       "IP pull attempt complete, result              - " + newIP);

            //  What to do w/ the IP
            if (stringIsIPAddress(newIP))
            {
                string Output0Try = Output0(newIP);
                logger.Write(   "New IP Email send complete, result            - " + Output0Try);
                if (IPisNew(newIP))
                {

                }
                string Output1Try = Output1(newIP, DataDir);
                logger.Write(   "Application data save complete, result        - " + Output1Try);
                string Output2Try = Output2(newIP);
                logger.Write(   "First output attempt complete, result         - " + Output2Try);
            }

            logger.SafeClose();
        }
    }
}
