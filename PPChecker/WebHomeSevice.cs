using System;
using System.Net;
using System.Web.Security;
using log4net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace PPChecker
{
    public class WebHomeSevice : IWebHomeService
    {
        private readonly ILog _log;
        private readonly IRestClient _client;
        private readonly string _webHomeServiceBaseUrl;
        private readonly bool _ignoreSslErrors;
        private readonly IHttpContextProvider _contextProvider;


        public WebHomeSevice(ILog log, IRestClient restClient, string webHomeServiceBaseUrl, bool ignoreSslErrors, IHttpContextProvider contextProvider)
        {
            _log = log;
            _client = restClient;
            _webHomeServiceBaseUrl = webHomeServiceBaseUrl;
            _ignoreSslErrors = ignoreSslErrors;
            _contextProvider = contextProvider;
        }

        public bool LatestPolicyAccepted(string ipAddress, string acceptLanguage)
        {
            if (_ignoreSslErrors)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }

            var request = new RestRequest(string.Format("{0}/me/privacypolicy", _webHomeServiceBaseUrl)) { Method = Method.GET };
            request.AddParameter("ip", ipAddress, ParameterType.QueryString);
            request.AddParameter("acceptLanguage", acceptLanguage);
            var authCookie = _contextProvider.GetCurrentHttpContext().Request.Cookies[FormsAuthentication.FormsCookieName];
            request.AddParameter(FormsAuthentication.FormsCookieName, authCookie.Value, ParameterType.Cookie);

            try
            {
                var response = _client.Execute(request);
                dynamic result = JObject.Parse(response.Content);
                return result.hasApprovedLatest;
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("{0} , failed while invoking the policy checker", ex);
                return true;
            }
        }
    }
}