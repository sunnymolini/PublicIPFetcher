using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections;

//using System.Collections;
namespace PublicIPFetcher
{
    public class LogMaker
    {
        private String AppTitle;
        private String logDirectory;
        private String logFile;
        private Queue msgQue;
        public Boolean stillRunning = true;
        public Thread QueTenderThread;

        private static String GetLogFileName(String LogDir, String title)
        {
            String result =  LogDir
                + @"\"
                + title
                + "."
                + Environment.UserName
                + "."
                + DateTime.Now.ToString("yyyy.MMdd.HHmm.ssff") + ".log";
            return result;
        }

        public LogMaker(String AppTitleParameter)
        {
            AppTitle = AppTitleParameter;
            logDirectory =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + AppTitle + @"\Logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFile = GetLogFileName(logDirectory, AppTitle);

            msgQue = new Queue();
            QueTenderThread = new Thread(this.QueTender);

            QueTenderThread.Start();
        }

        public LogMaker(String AppTitleParameter, String AppFileRepository)
        {
            AppTitle = AppTitleParameter;
            if (!Directory.Exists(AppFileRepository))
            {
                Directory.CreateDirectory(AppFileRepository);
            }
            logDirectory = AppFileRepository + @"\Logs";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            logFile = GetLogFileName(logDirectory, AppTitle);

            msgQue = new Queue();
            QueTenderThread = new Thread(this.QueTender);

            QueTenderThread.Start();
            //  There is probably a way to redo this so that the QueTender thread
            //  doesn't run constantly, but only if there are things being 
            //  written. Then it wouldn't have to test for the 'stillrunning' bool.
        }

        public LogMaker()
        {
            // TODO: Complete member initialization
        }
        ~LogMaker()
        {
            stillRunning = false;
        }
        public void Write(String message)
        {
            String s2write =
                DateTime.Now.ToString("MMdd.HH:mm:ss.fffffff")
                + " - "
                + message
                + Environment.NewLine;
            msgQue.Enqueue(s2write);
        }
        public void SafeClose()
        {
            stillRunning = false;
        }
        private void QueTender()
        {
            do
            {
                if (msgQue.Count != 0)
                {
                    StringBuilder sb = new StringBuilder();
                    while (msgQue.Count != 0)
                    {
                        try
                        {
                            sb.Append(msgQue.Dequeue().ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception in QueTender: " + ex.Message);
                        }
                    }
                    Send2File(sb.ToString());
                }
                else
                {
                    Thread.Sleep(50);
                }
            } while (stillRunning||msgQue.Count!=0);
        }
        private void Send2File(String message)
        {
            String oldLog = "";
            if (File.Exists(logFile))
            {
                StreamReader sr = new StreamReader(logFile);
                oldLog = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }
            try
            {
                StreamWriter sw = new StreamWriter(logFile);
                String s2write = oldLog + message;
                sw.Write(s2write);
                sw.Flush();
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Send2File: " + ex.Message);
            }
        }
    }
}
