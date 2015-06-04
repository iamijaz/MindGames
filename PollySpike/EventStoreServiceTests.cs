using System;
using System.Net;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace PollySpike
{
    public class BaseSetup
    {
        protected static Mock<IHttpClientService> _httpClientServiceMocked;
        protected static Mock<ILog> _logMocked;
        private static EventStoreService _eventStoreService;
        protected static Exception _exception;

        private Establish context = () =>
        {
            _httpClientServiceMocked = new Mock<IHttpClientService>();
            _logMocked = new Mock<ILog>();
            _eventStoreService = new EventStoreService(_httpClientServiceMocked.Object, _logMocked.Object);
        };

        private Because of = () =>
        {
            _exception = Catch.Exception(PostData);
        };


        private static void PostData()
        {
            _eventStoreService.PostData();
        }
    }

    public class When_Status_Is_Within_400s__No_Retry : BaseSetup
    {

        private Establish context = () =>
        {
            _httpClientServiceMocked.Setup(x => x.Post()).Throws(new UnableToCreateException(HttpStatusCode.BadRequest));
        };

        private It should_not_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(1));
        };

        private It should_not_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Never);
        };

        private It should_throw_an_UnableToCreateException = () =>
        {
            _exception.ShouldBeOfType<UnableToCreateException>();
        };
    }

    public class When_General_Exception_Is_Then_Retry : BaseSetup
    {
        private Establish context = () =>
        {
            _httpClientServiceMocked.Setup(x => x.Post()).Throws<Exception>();
        };

        private It should_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(5));
        };

        private It should_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Exactly(4));
        };

        private It should_throw_an_Exception = () =>
        {
            _exception.ShouldBeOfType<Exception>();
        };
    }

    public class When_Web_Exception_Is_Then_Retry : BaseSetup
    {

        private Establish context = () =>
        {
            _httpClientServiceMocked.Setup(x => x.Post()).Throws<WebException>();
        };

        private It should_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(5));
        };

        private It should_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Exactly(4));
        };

        private It should_throw_an_WebException = () =>
        {
            _exception.ShouldBeOfType<WebException>();
        };
    }

    public class When_500_Status_Then_Retry : BaseSetup
    {

        private Establish context = () =>
        {
            _httpClientServiceMocked.Setup(x => x.Post()).Throws(new UnableToCreateException(HttpStatusCode.InternalServerError));
        };

        private It should_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(5));
        };

        private It should_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Exactly(4));
        };

        private It should_throw_an_UnableToCreateException = () =>
        {
            _exception.ShouldBeOfType<UnableToCreateException>();
        };
    }

    public class When_Any_Other_Status_Than_400s_Then_Retry : BaseSetup
    {

        private Establish context = () =>
        {
            _httpClientServiceMocked.Setup(x => x.Post()).Throws(new UnableToCreateException(HttpStatusCode.Moved));
        };

        private It should_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(5));
        };

        private It should_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Exactly(4));
        };

        private It should_throw_an_UnableToCreateException = () =>
        {
            _exception.ShouldBeOfType<UnableToCreateException>();
        };
    }

    public class When_There_is_No_error_Then_no_log_no_retry : BaseSetup
    {
        private It should_not_retry = () =>
        {
            _httpClientServiceMocked.Verify(x => x.Post(), Times.Exactly(1));
        };

        private It should_not_log = () =>
        {
            _logMocked.Verify(x => x.LogIt(), Times.Exactly(0));
        };

        private It should_not_throw_any_exception = () =>
        {
            _exception.ShouldBeNull();
        };
    }
}

