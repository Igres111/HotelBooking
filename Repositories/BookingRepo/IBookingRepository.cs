﻿using HotelBooking.DTOs.BookingDtos;
using HotelBooking.Models;

namespace HotelBooking.Repositories.BookingRepo
{
    public interface IBookingRepository
    {
        public Task<BookingInfoDto> BookHotel(BookingInfoDto info);
        public Task<ReceiveBookingDto> GetBookingById(Guid id);
        public Task<ReceiveBookingDto> GetBookingByUser(Guid userId);
        public Task DeleteBooking(Guid id);
    }
}
