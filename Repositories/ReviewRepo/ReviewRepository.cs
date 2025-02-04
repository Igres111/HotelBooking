using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.ReviewRepo
{
    public class ReviewRepository : IReviewRepository
    {
        public readonly Context _context;
        public readonly IMapper _mapper;
        public ReviewRepository(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task RegisterReview(RegisterReviewDto review)
        {
            var result = _mapper.Map<Reviews>(review);
            result.Id = Guid.NewGuid();
            result.CreatedAt = DateTime.Now;
            await _context.Reviews.AddAsync(result);
            await _context.SaveChangesAsync();
        }
    }
}
