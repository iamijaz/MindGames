using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using log4net;
using Moq;
using NUnit.Framework;
using RestSharp;
using HttpCookie = System.Web.HttpCookie;

namespace PPChecker
{
    public class Base
    {
        protected Mock<IRestClient> MockedRestClient;
        protected Mock<IRestResponse> MockedResponse;
        protected Mock<ILog> MockedLog;
        protected bool Result;
        protected string WebHomeServiceBaseUrl;
        protected string AuthCookieValue;
        protected Mock<IHttpContextProvider> HttpContextProvider;

        [SetUp]
        public void SetUp()
        {
            MockedLog = new Mock<ILog>();
            MockedRestClient = new Mock<IRestClient>();
            MockedResponse = new Mock<IRestResponse>();
            HttpContextProvider = new Mock<IHttpContextProvider>();
            var httpContext = new Mock<HttpContextBase>();
            AuthCookieValue = "someValue";
            httpContext.Setup(f => f.Request.Cookies).Returns(new HttpCookieCollection { new HttpCookie(FormsAuthentication.FormsCookieName, AuthCookieValue) });
            HttpContextProvider.Setup(f => f.GetCurrentHttpContext()).Returns(httpContext.Object);

        }
    }

    public class When_the_service_is_invoked_successfully : Base
    {
        private string _ipAddress;
        private string _acceptLanguage;

        [SetUp]
        public void SetUp()
        {
            MockedResponse.Setup(x => x.Content).Returns("{\"hasApprovedLatest\":true,\"explicit\":false}");
            MockedRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>())).Returns(MockedResponse.Object);
            WebHomeServiceBaseUrl = "http://something.com";


            var client = new WebHomeSevice(MockedLog.Object, MockedRestClient.Object, WebHomeServiceBaseUrl, true, HttpContextProvider.Object);
            _ipAddress = "127.0.0.1";
            _acceptLanguage = "en-GB";

            Result = client.LatestPolicyAccepted(_ipAddress, _acceptLanguage);
        }

        [Test]
        public void Then_it_should_call_the_web_home_with_the_correct_url()
        {
            MockedRestClient.Verify(f => f.Execute(It.Is<IRestRequest>(c => c.Resource == string.Format("{0}/me/privacypolicy", WebHomeServiceBaseUrl))));
        }

        [Test]
        public void Then_it_should_send_the_ip_address()
        {
            MockedRestClient.Verify(f => f.Execute(It.Is<IRestRequest>(c => c.Parameters.First(d => d.Name == "ip").Value == _ipAddress)));
        }

        [Test]
        public void Then_it_should_send_the_accept_language()
        {
            MockedRestClient.Verify(f => f.Execute(It.Is<IRestRequest>(c => c.Parameters.First(d => d.Name == "acceptLanguage").Value == _acceptLanguage)));
        }

        [Test]
        public void Then_it_should_send_the_auth_cookie()
        {
            MockedRestClient.Verify(f => f.Execute(It.Is<IRestRequest>(c => c.Parameters.First(d => d.Name == FormsAuthentication.FormsCookieName && d.Type == ParameterType.Cookie).Value == AuthCookieValue)));
        }

        [Test]
        public void Then_it_should_return_the_reslut_as_true()
        {
            Assert.That(Result, Is.True);
        }
    }

    public class When_service_call_is_failed : Base
    {
        [SetUp]
        public void SetUp()
        {
            MockedLog.Setup(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<Exception>()));
            MockedRestClient.Setup(x => x.Execute(It.IsAny<RestRequest>())).Throws<Exception>();

            var client = new WebHomeSevice(MockedLog.Object, MockedRestClient.Object, WebHomeServiceBaseUrl, true, HttpContextProvider.Object);

            Result = client.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>());
        }

        [Test]
        public void Then_default_result_true_should_be_returnted()
        {
            Assert.That(Result, Is.True);
        }

        [Test]
        public void Then_an_error_is_logged()
        {
            MockedLog.Verify(x => x.ErrorFormat(It.IsAny<string>(), It.IsAny<Exception>()), Times.AtLeastOnce);
        }
    }
}