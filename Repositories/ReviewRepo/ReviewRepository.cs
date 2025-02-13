using AutoMapper;
using HotelBooking.Data;
using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;
using Sprache;

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
        public async Task<List<ReturnReviewDto>> GetReviewsById(Guid hotelId)
        {
            var result = await _context.Hotels
                .Where(x => x.Id == hotelId)
                .Select(h => new ReturnReviewDto
                {
                    HotelId = h.Id,
                    Name = h.Name,
                    Address = h.Address,
                    City = h.City,
                    PricePerNight = h.PricePerNight,
                    AvgRating = h.AvgRating,
                    StarReviews = h.StarReviews,
                    Capacity = h.Capacity,
                    Description = h.Description,
                    Reviews = h.Reviews.Select(r => new ReviewForUserDto
                    {
                        Review = new List<ReviewDto> { new ReviewDto
                        {
                            Id = r.Id,
                            HotelId = r.HotelId,
                            UserId = r.UserId,
                            Rating = r.Rating,
                            Comment = r.Comment,
                            CreatedAt = r.CreatedAt
                        }},
                        UserId = r.User.Id,
                        FullName = r.User.FullName,
                        Email = r.User.Email,
                    }).ToList()
                }).ToListAsync();
            return result;
        }
    }
}
