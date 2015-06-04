using System;
using System.Net;
using Polly;

namespace PollySpike
{
    public class EventStoreService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly ILog _log;

        public EventStoreService(IHttpClientService httpClientService, ILog log)
        {
            _httpClientService = httpClientService;
            _log = log;
        }

        public void PostData()
        {
            Policy
                .Handle<UnableToCreateException>(x => (int)x.StatusCode < 400 || (int)x.StatusCode > 499)
                .Or<Exception>(x => x.GetType() != typeof(UnableToCreateException))
                .Or<WebException>().WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                }, (exception, timeSpan) =>
                {
                    Console.WriteLine("Before Retry: {0:mm:ss.fff}-{1}", DateTime.UtcNow, timeSpan.Seconds);
                    _log.LogIt();
                })
                .Execute(Post);
        }

        private int i = 1;
        private void Post()
        {
            Console.WriteLine("Actual: {0}: {1:mm:ss}", i++, DateTime.Now);
            _httpClientService.Post();
        }
    }
}