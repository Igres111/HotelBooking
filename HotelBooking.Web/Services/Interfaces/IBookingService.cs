using HotelBooking.Web.Models.Requests;
using HotelBooking.Web.Models.Responses;

namespace HotelBooking.Web.Services.Interfaces
{
    public interface IBookingService
    {
        Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(GetBookingsRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken);
        Task<byte[]> ExportFile(CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> GetById(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<int>> Create(CreateBookingRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<List<int>>> CreateRecurring(CreateRecurringBookingRequest request, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Cancel(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Confirm(int id, CancellationToken cancellationToken);
        Task<ResponseWrapper<BookingResponse>> Reject(int id, CancellationToken cancellationToken);
    }
}
