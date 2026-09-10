using System.Net;
using System.Text.Json;
using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Helpers;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class BookingService(
        IBookingRepository bookingRepository,
        IMeetingRoomRepository roomRepository,
        IIdempotencyRecordRepository idempotencyRecordRepository,
        IValidator<CreateBookingRequest> createBookingRequestValidator,
        ITimeZoneConverter timeZoneConverter) : IBookingService
    {
        public async Task<ResponseWrapper<int>> Create(CreateBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken)
        {
            await createBookingRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            string? requestHash = null;

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                if (idempotencyKey.Length > ValidatorConstants.StringLengths.IdempotencyKeyMaxLength)
                {
                    return new ResponseWrapper<int>(
                        false,
                        (int)HttpStatusCode.BadRequest,
                        $"Idempotency key must not exceed {ValidatorConstants.StringLengths.IdempotencyKeyMaxLength} characters.",
                        default);
                }

                requestHash = IdempotencyHelper.ComputeRequestHash(request);

                var existingRecord = await idempotencyRecordRepository.GetByKey(userId, idempotencyKey, cancellationToken);
                if (existingRecord is not null)
                {
                    if (existingRecord.RequestHash != requestHash)
                    {
                        return new ResponseWrapper<int>(
                            false,
                            (int)HttpStatusCode.Conflict,
                            "This idempotency key has already been used with different booking data.",
                            default);
                    }

                    return JsonSerializer.Deserialize<ResponseWrapper<int>>(existingRecord.ResponseBody)!;
                }
            }

            var room = await roomRepository.GetActiveById(request.RoomId, cancellationToken);
            if (room is null)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found or is not active.",
                    default);
            }

            if (request.AttendeeCount > room.Capacity)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "Attendee count exceeds room capacity.",
                    default);
            }

            if (request.StartTime < room.OpeningTime || request.EndTime > room.ClosingTime)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "Booking must be within the room's business hours.",
                    default);
            }

            var startUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(request.StartTime), request.TimeZoneId);
            var endUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(request.EndTime), request.TimeZoneId);

            var hasOverlap = await bookingRepository.HasOverlappingConfirmedBooking(request.RoomId, startUtc, endUtc, cancellationToken);
            if (hasOverlap)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "This room is already booked for the requested time slot.",
                    default);
            }

            var booking = new Booking
            {
                RoomId = request.RoomId,
                UserId = userId,
                StartUtc = startUtc,
                EndUtc = endUtc,
                AttendeeCount = request.AttendeeCount,
                Notes = request.Notes,
                Status = BookingStatus.Pending
            };

            await bookingRepository.AddAsync(booking, cancellationToken);

            IdempotencyRecord? idempotencyRecord = null;

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                idempotencyRecord = new IdempotencyRecord
                {
                    UserId = userId,
                    Key = idempotencyKey,
                    RequestHash = requestHash!
                };

                await idempotencyRecordRepository.AddAsync(idempotencyRecord, cancellationToken);
            }

            await bookingRepository.SaveChangesAsync(cancellationToken);

            var response = new ResponseWrapper<int>(
                true,
                (int)HttpStatusCode.Created,
                "Booking created successfully.",
                booking.Id);

            if (idempotencyRecord is not null)
            {
                idempotencyRecord.ResponseStatusCode = response.StatusCode;
                idempotencyRecord.ResponseBody = JsonSerializer.Serialize(response);

                await bookingRepository.SaveChangesAsync(cancellationToken);
            }

            return response;
        }

        public async Task<ResponseWrapper<List<BookingResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var bookings = await bookingRepository.GetAllWithDetails(cancellationToken);

            var response = bookings
                .Select(booking => new BookingResponse(
                    booking.Id,
                    booking.RoomId,
                    booking.Room!.Name,
                    booking.UserId,
                    booking.User!.FullName,
                    booking.StartUtc,
                    booking.EndUtc,
                    booking.AttendeeCount,
                    booking.Notes,
                    booking.Status))
                .ToList();

            return new ResponseWrapper<List<BookingResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Bookings retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<PagedResult<BookingResponse>>> GetAllForUser(int userId, GetBookingsRequest request, CancellationToken cancellationToken)
        {
            var page = Math.Max(request.Page, 1);

            int NormalizePageSize(int pageSize) => pageSize >= 1 && pageSize <= PaginationConstants.MaxPageSize ? pageSize : PaginationConstants.DefaultPageSize;

            var pageSize = NormalizePageSize(request.PageSize);

            var pagedBookings = await bookingRepository.GetForUserPaged(
                userId: userId,
                roomId: request.RoomId,
                status: request.Status,
                sortBy: request.SortBy,
                sortDescending: request.SortDescending,
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            var items = pagedBookings.Items
                .Select(booking => new BookingResponse(
                    booking.Id,
                    booking.RoomId,
                    booking.Room!.Name,
                    booking.UserId,
                    booking.User!.FullName,
                    booking.StartUtc,
                    booking.EndUtc,
                    booking.AttendeeCount,
                    booking.Notes,
                    booking.Status))
                .ToList();

            var response = new PagedResult<BookingResponse>(items, pagedBookings.TotalCount, page, pageSize);

            return new ResponseWrapper<PagedResult<BookingResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Bookings retrieved successfully.",
                response);
        }
    }
}
