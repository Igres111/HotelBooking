using AutoMapper;
using HotelBooking.DTOs;
using HotelBooking.Models;

namespace HotelBooking.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User,RegisterDto>().ReverseMap();
        }
    }
}
