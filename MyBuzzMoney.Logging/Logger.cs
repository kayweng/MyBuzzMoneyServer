using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyBuzzMoney.Logging
{
    public class Logger : ILogger
    {
        #region Properties
        public static Logger _instance = new Logger();

        public static Logger Instance
        {
            get{ 
                return _instance;
            }
        }

        public Logger()
        {
            
        }
        #endregion

        /// <summary>
        /// async write a exception message to log of cloud watch
        /// </summary>
        /// <param name="ex"></param>
        public void LogException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// async write a info message to log of cloud watch
        /// </summary>
        /// <param name="message"></param>
        public void LogInfo(string message)
        {
            Console.WriteLine(message);
        }
    }
}
