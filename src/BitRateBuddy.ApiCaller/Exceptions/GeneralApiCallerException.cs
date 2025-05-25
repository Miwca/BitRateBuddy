using System.Net;

namespace BitRateBuddy.ApiCaller.Exceptions
{
    public class ApiCallerException(HttpStatusCode statusCode, string message) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; set; } = statusCode;
    }
}
