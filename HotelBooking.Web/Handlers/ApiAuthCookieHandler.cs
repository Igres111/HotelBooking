namespace HotelBooking.Web.Handlers
{
    public class ApiAuthCookieHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        public const string ClaimType = "ApiAuthCookie";
        public const string AntiforgeryCookieClaimType = "ApiAntiforgeryCookie";
        public const string AntiforgeryTokenClaimType = "ApiAntiforgeryToken";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = httpContextAccessor.HttpContext?.User;
            var apiAuthCookie = user?.FindFirst(ClaimType)?.Value;
            var antiforgeryCookie = user?.FindFirst(AntiforgeryCookieClaimType)?.Value;
            var antiforgeryToken = user?.FindFirst(AntiforgeryTokenClaimType)?.Value;

            var cookieParts = new List<string>();

            if (!string.IsNullOrEmpty(apiAuthCookie))
            {
                cookieParts.Add(apiAuthCookie);
            }

            if (!string.IsNullOrEmpty(antiforgeryCookie))
            {
                cookieParts.Add(antiforgeryCookie);
            }

            if (cookieParts.Count > 0)
            {
                request.Headers.Add("Cookie", string.Join("; ", cookieParts));
            }

            if (!string.IsNullOrEmpty(antiforgeryToken))
            {
                request.Headers.Add("X-CSRF-TOKEN", antiforgeryToken);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
