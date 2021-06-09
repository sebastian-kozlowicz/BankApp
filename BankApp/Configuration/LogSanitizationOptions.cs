using System.Collections.Generic;

namespace BankApp.Configuration
{
    public class LogSanitizationOptions
    {
        public bool IsEnabled { get; set; }
        public List<string> HeaderNamesToSanitize { get; set; }
    }
}
