using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.ApiClients.Interfaces
{
    public interface IDashboardApiClient
    {
        Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken);
    }
}
