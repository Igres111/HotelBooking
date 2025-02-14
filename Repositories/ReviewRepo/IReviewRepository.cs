using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.ReviewRepo
{
    public interface IReviewRepository
    {
        public Task RegisterReview(RegisterReviewDto review);
        public Task<List<ReturnReviewDto>> GetReviewsById(Guid hotelId);
        public Task DeleteReview(Guid reviewId);
    }
}
