using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;
using HotelBooking.Web.Models.ViewModels;

namespace HotelBooking.Web.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ResponseWrapper<DashboardResponse>> GetStatistics(GetDashboardRequest request, CancellationToken cancellationToken);
        Task<DashboardViewModel> GetDashboardViewModel(
            int? year,
            int? month,
            int? roomId,
            DateOnly? date,
            TimeOnly? fromTime,
            TimeOnly? toTime,
            CancellationToken cancellationToken);
    }
}
