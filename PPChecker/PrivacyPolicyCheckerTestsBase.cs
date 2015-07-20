using System;
using System.Collections.Specialized;
using System.Web;
using Moq;
using NUnit.Framework;

namespace PPChecker
{
    public class PrivacyPolicyCheckerTestsBase
    {
        protected Mock<HttpContextBase> _httpContextMock;
        protected Mock<HttpRequestBase> _mockHttpRequest;
        protected PrivacyPolicyChecker _sut;
        protected bool _latestPolicyAccepted;
        protected Mock<IWebHomeService> _webHomeService;
        protected Mock<IUserVerifier> _userVerifierMock;

        [SetUp]
        public void SetUp()
        {
            _httpContextMock = new Mock<HttpContextBase>();
            _mockHttpRequest = new Mock<HttpRequestBase>();
            _mockHttpRequest.Setup(f => f.UrlReferrer).Returns(new Uri("http://www.justgiving.com"));
            _mockHttpRequest.Setup(f => f.QueryString).Returns(new NameValueCollection());
            _mockHttpRequest.Setup(f => f.Headers).Returns(new NameValueCollection());
            _httpContextMock.Setup(f => f.Request).Returns(_mockHttpRequest.Object);
            _webHomeService = new Mock<IWebHomeService>();
            _userVerifierMock = new Mock<IUserVerifier>();
            _userVerifierMock.Setup(f => f.IsAuthenticated).Returns(true);
            _userVerifierMock.Setup(f => f.AccountId).Returns(100);
            var httpContextProvider = new Mock<IHttpContextProvider>();
            httpContextProvider.Setup(c => c.GetCurrentHttpContext()).Returns(_httpContextMock.Object);
            _sut = new PrivacyPolicyChecker(_userVerifierMock.Object, _webHomeService.Object, httpContextProvider.Object);
        }

        [TestFixture]
        public class When_building_the_awesome_model_and_the_policy_cookie_is_present_and_the_policy_has_not_been_accepted : PrivacyPolicyCheckerTestsBase
        {

            [SetUp]
            public void SetUp()
            {
                _mockHttpRequest.Setup(f => f.Cookies).Returns(new HttpCookieCollection() { new HttpCookie("JGPP", HttpUtility.HtmlEncode("{hasApprovedLatest: false, explicit: true}")) });
                _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
            }

            [Test]
            public void It_should_return_that_the_latest_policy_has_not_been_accepted()
            {
                Assert.That(_latestPolicyAccepted, Is.False);
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_policy_cookie_is_present_and_the_policy_has_already_been_accepted : PrivacyPolicyCheckerTestsBase
            {
                [SetUp]
                public void SetUp()
                {
                    _mockHttpRequest.Setup(f => f.Cookies).Returns(new HttpCookieCollection() { new HttpCookie("JGPP", "{hasApprovedLatest: true, explicit: true}") });
                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_return_that_the_latest_policy_has_been_accepted()
                {
                    Assert.That(_latestPolicyAccepted, Is.True);
                }
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_policy_cookie_is_not_present_and_latest_policy_has_not_been_accepted : PrivacyPolicyCheckerTestsBase
            {
                private string _acceptLanguage;

                [SetUp]
                public void SetUp()
                {
                    _mockHttpRequest.Setup(f => f.Cookies).Returns(new HttpCookieCollection());
                    _acceptLanguage = "en-GB";
                    var headers = new NameValueCollection
                {
                    {"Accept-Language", _acceptLanguage},
                    {"X-Forwarded-For", "129.78.138.66, 129.78.64.103"}
                };
                    _mockHttpRequest.Setup(f => f.Headers).Returns(headers);
                    _webHomeService.Setup(f => f.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
                    _userVerifierMock.Setup(f => f.IsAuthenticated).Returns(true);
                    _userVerifierMock.Setup(f => f.AccountId).Returns(100);
                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_call_the_web_home_service_for_the_correct_user()
                {
                    _webHomeService.Verify(f => f.LatestPolicyAccepted(It.Is<string>(i => i == "129.78.138.66"), It.Is<string>(d => d == _acceptLanguage)));
                }

                [Test]
                public void It_should_return_that_the_latest_policy_has_not_been_accepted()
                {
                    Assert.That(_latestPolicyAccepted, Is.False);
                }
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_policy_cookie_is_not_present_and_latest_policy_has_already_been_accepted : PrivacyPolicyCheckerTestsBase
            {

                [SetUp]
                public void SetUp()
                {
                    _mockHttpRequest.Setup(f => f.Cookies).Returns(new HttpCookieCollection());
                    _webHomeService.Setup(f => f.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_display_a_message_saying_the_policy_has_been_updated()
                {
                    Assert.That(_latestPolicyAccepted, Is.True);
                }
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_policy_cookie_is_malformed : PrivacyPolicyCheckerTestsBase
            {

                [SetUp]
                public void SetUp()
                {

                    _mockHttpRequest.Setup(f => f.Cookies).Returns(new HttpCookieCollection() { new HttpCookie("JGPP", "not_a_json") });

                    _webHomeService.Setup(f => f.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_call_home_service_to_determine_the_status()
                {
                    Assert.That(_latestPolicyAccepted, Is.True);
                }
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_user_is_not_logged_in : PrivacyPolicyCheckerTestsBase
            {

                [SetUp]
                public void SetUp()
                {
                    _webHomeService.Setup(f => f.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

                    _userVerifierMock.Setup(f => f.IsAuthenticated).Returns(false);

                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_not_call_home_service_to_determine_the_status()
                {
                    _webHomeService.Verify(x => x.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                }

                [Test]
                public void It_should_return_that_the_latest_policy_has_been_accepted()
                {
                    Assert.That(_latestPolicyAccepted, Is.True);
                }
            }

            [TestFixture]
            public class When_building_the_awesome_model_and_the_user_does_not_have_an_account_id_yet : PrivacyPolicyCheckerTestsBase
            {

                [SetUp]
                public void SetUp()
                {
                    _webHomeService.Setup(f => f.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

                    _userVerifierMock.Setup(f => f.IsAuthenticated).Returns(true);
                    _userVerifierMock.Setup(f => f.AccountId).Returns(0);
                    _latestPolicyAccepted = _sut.IsLatestPrivacyPolicyAccepted();
                }

                [Test]
                public void It_should_not_call_home_service_to_determine_the_status()
                {
                    _webHomeService.Verify(x => x.LatestPolicyAccepted(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                }

                [Test]
                public void It_should_return_that_the_latest_policy_has_been_accepted()
                {
                    Assert.That(_latestPolicyAccepted, Is.True);
                }
            }
        }
    }
}
