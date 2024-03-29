﻿using Microsoft.AspNetCore.Http;

namespace BankApp.Models.RequestResponseLogging
{
    public class ResponseInfo
    {
        public string TraceIdentifier { get; set; }
        public string Path { get; set; }
        public int StatusCode { get; set; }
        public IHeaderDictionary Headers { get; set; }
        public object Result { get; set; }
        public string ExceptionMessage { get; set; }
        public bool ExceptionThrown => ExceptionMessage != null;
        public bool IsServerErrorStatusCode => StatusCode >= 500;
    }
}