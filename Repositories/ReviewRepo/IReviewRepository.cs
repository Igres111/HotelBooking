using HotelBooking.DTOs.ReviewDtos;

namespace HotelBooking.Repositories.ReviewRepo
{
    public interface IReviewRepository
    {
        public Task RegisterReview(RegisterReviewDto review);
    }
}
