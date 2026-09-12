namespace HotelBooking.Web.Handlers
{
    public class ApiAuthCookieHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        public const string ClaimType = "ApiAuthCookie";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiAuthCookie = httpContextAccessor.HttpContext?.User.FindFirst(ClaimType)?.Value;

            if (!string.IsNullOrEmpty(apiAuthCookie))
            {
                request.Headers.Add("Cookie", apiAuthCookie);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
