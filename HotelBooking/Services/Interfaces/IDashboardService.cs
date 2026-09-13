using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;

namespace HotelBooking.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken);
    }
}
