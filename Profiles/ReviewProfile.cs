
using AutoMapper;
using HotelBooking.DTOs.ReviewDtos;
using HotelBooking.Models;

namespace HotelBooking.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile() 
        {
            CreateMap<Reviews,RegisterReviewDto>().ReverseMap();
        }
    }
}
