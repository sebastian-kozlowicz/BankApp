using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace BankApp.Models.RequestResponseLogging
{
    public class RequestInfo
    {
        public string TraceIdentifier { get; set; }
        public string Method { get; set; }
        public string Scheme { get; set; }
        public string Path { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public IDictionary<string, object> ActionArguments { get; set; }
    }
}