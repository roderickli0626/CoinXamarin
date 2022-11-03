using System;
using System.Net;
using System.Net.Http;

namespace mycoin.Models
{
    public class ApiCallLog
    {
        public string BaseAddress { get; set; }
        public string Url { get; set; }
        public string ApiVersion { get; set; }
        public HttpMethod MethodType { get; set; }
        public string RequestContent { get; set; }
        public HttpStatusCode ResponseCode { get; set; }
        public string ResponseContent { get; set; }
        public DateTime DateCalled { get; set; }
    }
}
