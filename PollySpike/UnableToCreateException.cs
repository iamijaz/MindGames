using System;
using System.Net;

namespace PollySpike
{
    public class UnableToCreateException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public UnableToCreateException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
    }
}