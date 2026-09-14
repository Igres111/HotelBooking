using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelBooking.Filters
{
    public class ValidateApiAntiforgeryTokenAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var antiforgery = context.HttpContext.RequestServices.GetRequiredService<IAntiforgery>();

            await antiforgery.ValidateRequestAsync(context.HttpContext);

            await next();
        }
    }
}
