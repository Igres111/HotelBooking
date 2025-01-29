using AutoMapper;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;

namespace HotelBooking.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User,RegisterDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
        }
    }
}
