using AutoMapper;
using HotelBooking.DTOs.UserDtos;
using HotelBooking.Models;

namespace HotelBooking.Profiles
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, RegisterDto>().ReverseMap().ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<User, ReceiveUserDto>().ReverseMap();
        }
    }
}
