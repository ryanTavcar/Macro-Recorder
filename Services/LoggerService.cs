using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Names.Services
{
    public class LoggerService
    {
        private static LoggerService _instance;
        public static LoggerService Instance => _instance ??= new LoggerService();

        // Event that other classes can subscribe to
        public event Action<string> LogAdded;

        // Method to log messages
        public void Log(string message)
        {
            LogAdded?.Invoke(message);
        }
    }
}
