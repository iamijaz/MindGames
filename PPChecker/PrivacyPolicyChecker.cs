using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PPChecker
{
    public interface IPrivacyPolicyChecker
    {
        bool IsLatestPrivacyPolicyAccepted();
    }

    public interface IUserVerifier
    {
        bool IsAuthenticated { get; }
        int AccountId { get; }
    }

    public interface IHttpContextProvider
    {
        HttpContextBase GetCurrentHttpContext();
    }

    public class PrivacyPolicyChecker : IPrivacyPolicyChecker
    {
        public const string PolicyWarningMessage = "test";

        private readonly IUserVerifier _userVerifier;
        private readonly IWebHomeService _webHomeService;
        private readonly IHttpContextProvider _httpContextProvider;

        public PrivacyPolicyChecker(IUserVerifier userVerifier, IWebHomeService webHomeService, IHttpContextProvider httpContextProvider)
        {
            _userVerifier = userVerifier;
            _webHomeService = webHomeService;
            _httpContextProvider = httpContextProvider;
        }

        public bool IsLatestPrivacyPolicyAccepted()
        {
            if (!_userVerifier.IsAuthenticated)
                return true;

            //hack for users which are created during the classic donation process.
            //they are authenticated but they do not have an account id or user id yet
            if (_userVerifier.AccountId == 0)
                return true;

            var httpContext = _httpContextProvider.GetCurrentHttpContext();

            var privacyPolicyCookie = httpContext.Request.Cookies["JGPP"];

            var acceptLanguage = httpContext.Request.Headers["Accept-Language"];

            var privacyPolicyCookieObject = new { hasApprovedLatest = false };

            if (privacyPolicyCookie != null)
            {
                try
                {
                    var privacyPolicyCookieValue = HttpUtility.HtmlDecode(privacyPolicyCookie.Value);
                    var privacyPolicyCookieObjectDeserialized = JsonConvert.DeserializeAnonymousType(privacyPolicyCookieValue, privacyPolicyCookieObject);
                    return privacyPolicyCookieObjectDeserialized.hasApprovedLatest;
                }
                catch (JsonReaderException)
                {
                    return LatestPrivacyPolicyAcceptedViaWebHomeService(httpContext, acceptLanguage);
                }

            }
            return LatestPrivacyPolicyAcceptedViaWebHomeService(httpContext, acceptLanguage);
        }

        private bool LatestPrivacyPolicyAcceptedViaWebHomeService(HttpContextBase httpContext, string acceptLanguage)
        {
            return _webHomeService.LatestPolicyAccepted(WebUtil.GetClientIp(httpContext.Request), acceptLanguage);
        }
    }

    internal class WebUtil
    {
        public static string GetClientIp(HttpRequestBase request)
        {
            var ip = UserIpAddress.GetFromHeaders(ConvertHeaders(request.Headers));
            if (ip != null)
            {
                return ip;
            }

            return request.UserHostAddress;
        }

        private static IDictionary<string, IEnumerable<string>> ConvertHeaders(NameValueCollection headers)
        {
            return headers.AllKeys.ToDictionary<string, string, IEnumerable<string>>(k => k, headers.GetValues);
        }
    }

    public interface IWebHomeService
    {
        bool LatestPolicyAccepted(string ipAddress, string acceptLanguage);
    }
}