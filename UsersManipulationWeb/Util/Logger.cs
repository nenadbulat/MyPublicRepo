using System.Net;

namespace UsersManipulationWeb.Util
{
    public static class Logger
    {
        static string logFileName = string.Format(@"C:\UsersManipulation\Log_{0}_{1}_{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        private static int currentDay = 0;

        public static void LogMessage(Enums.LogLevel logLevel, string message)
        {
            try
            {
                GenerateNewLogFileCheck();
                FileStream stream = new FileStream(logFileName, File.Exists(logFileName) ? FileMode.Append : FileMode.Create);
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    sw.Write("\t");
                    sw.Write(logLevel.ToString());
                    sw.Write("\t");
                    sw.WriteLine(message);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void LogMethodCall(string userHostAddress, string userHostName, string methodName, string printParameters)
        {
            try
            {
                string hostName = Dns.GetHostName();
                string contextData = string.Format("\n\tMethod: {0} with parameters: \n\t{1} \n\tcalled by: {2} from IP address: {3} on host: {4}",
                    methodName,
                    printParameters,
                    userHostName,
                    userHostAddress,
                    hostName);
                LogMessage(Enums.LogLevel.Info, contextData);
            }
            catch (Exception)
            {
            }
        }

        private static void GenerateNewLogFileCheck()
        {
            //At any midnight only day changes
            if (currentDay != DateTime.Now.Day)
            {
                currentDay = DateTime.Now.Day;
                GenerateNewDayLogFile();
            }
        }

        public static void GenerateNewDayLogFile()
        {
            logFileName = string.Format(@"C:\UsersManipulation\Log_{0}_{1}_{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }
    }
}
