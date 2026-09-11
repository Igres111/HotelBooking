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
        IRepository<RecurringBookingSeries> recurringBookingSeriesRepository,
        IValidator<CreateBookingRequest> createBookingRequestValidator,
        IValidator<CreateRecurringBookingRequest> createRecurringBookingRequestValidator,
        ITimeZoneConverter timeZoneConverter) : IBookingService
    {
        public async Task<ResponseWrapper<int>> Create(CreateBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken)
        {
            await createBookingRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "The Idempotency-Key header is required.",
                    default);
            }

            if (idempotencyKey.Length > ValidatorConstants.StringLengths.IdempotencyKeyMaxLength)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    $"Idempotency key must not exceed {ValidatorConstants.StringLengths.IdempotencyKeyMaxLength} characters.",
                    default);
            }

            var requestHash = IdempotencyHelper.ComputeRequestHash(request);

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

            var idempotencyRecord = new IdempotencyRecord
            {
                UserId = userId,
                Key = idempotencyKey,
                RequestHash = requestHash
            };

            await idempotencyRecordRepository.AddAsync(idempotencyRecord, cancellationToken);
            await bookingRepository.SaveChangesAsync(cancellationToken);

            var response = new ResponseWrapper<int>(
                true,
                (int)HttpStatusCode.Created,
                "Booking created successfully.",
                booking.Id);

            idempotencyRecord.ResponseStatusCode = response.StatusCode;
            idempotencyRecord.ResponseBody = JsonSerializer.Serialize(response);

            await bookingRepository.SaveChangesAsync(cancellationToken);

            return response;
        }

        public async Task<ResponseWrapper<List<int>>> CreateRecurring(CreateRecurringBookingRequest request, int userId, string? idempotencyKey, CancellationToken cancellationToken)
        {
            await createRecurringBookingRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                return new ResponseWrapper<List<int>>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "The Idempotency-Key header is required.",
                    default);
            }

            if (idempotencyKey.Length > ValidatorConstants.StringLengths.IdempotencyKeyMaxLength)
            {
                return new ResponseWrapper<List<int>>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    $"Idempotency key must not exceed {ValidatorConstants.StringLengths.IdempotencyKeyMaxLength} characters.",
                    default);
            }

            var requestHash = IdempotencyHelper.ComputeRequestHash(request);

            var existingRecord = await idempotencyRecordRepository.GetByKey(userId, idempotencyKey, cancellationToken);
            if (existingRecord is not null)
            {
                if (existingRecord.RequestHash != requestHash)
                {
                    return new ResponseWrapper<List<int>>(
                        false,
                        (int)HttpStatusCode.Conflict,
                        "This idempotency key has already been used with different booking data.",
                        default);
                }

                return JsonSerializer.Deserialize<ResponseWrapper<List<int>>>(existingRecord.ResponseBody)!;
            }

            var room = await roomRepository.GetActiveById(request.RoomId, cancellationToken);
            if (room is null)
            {
                return new ResponseWrapper<List<int>>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found or is not active.",
                    default);
            }

            if (request.AttendeeCount > room.Capacity)
            {
                return new ResponseWrapper<List<int>>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "Attendee count exceeds room capacity.",
                    default);
            }

            if (request.StartTime < room.OpeningTime || request.EndTime > room.ClosingTime)
            {
                return new ResponseWrapper<List<int>>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "Booking must be within the room's business hours.",
                    default);
            }

            var occurrenceUtcRanges = new List<(DateTime StartUtc, DateTime EndUtc)>();

            for (var occurrenceIndex = 0; occurrenceIndex < request.OccurrenceCount; occurrenceIndex++)
            {
                var occurrenceDate = request.StartDate.AddDays(ValidatorConstants.Numbers.RecurringOccurrenceIntervalDays * occurrenceIndex);

                if (!BookingValidationHelper.IsInTheFuture(occurrenceDate, request.StartTime, request.TimeZoneId, timeZoneConverter))
                {
                    return new ResponseWrapper<List<int>>(
                        false,
                        (int)HttpStatusCode.BadRequest,
                        $"Occurrence {occurrenceIndex + 1} ({occurrenceDate:yyyy-MM-dd}) must be in the future.",
                        default);
                }

                if (!BookingValidationHelper.IsWithinAdvanceBookingWindow(occurrenceDate, request.TimeZoneId, timeZoneConverter))
                {
                    return new ResponseWrapper<List<int>>(
                        false,
                        (int)HttpStatusCode.BadRequest,
                        $"Occurrence {occurrenceIndex + 1} ({occurrenceDate:yyyy-MM-dd}) cannot be more than {ValidatorConstants.Numbers.MaximumAdvanceBookingDays} days in advance.",
                        default);
                }

                var occurrenceStartUtc = timeZoneConverter.ConvertToUtc(occurrenceDate.ToDateTime(request.StartTime), request.TimeZoneId);
                var occurrenceEndUtc = timeZoneConverter.ConvertToUtc(occurrenceDate.ToDateTime(request.EndTime), request.TimeZoneId);

                var hasOverlap = await bookingRepository.HasOverlappingConfirmedBooking(request.RoomId, occurrenceStartUtc, occurrenceEndUtc, cancellationToken);
                if (hasOverlap)
                {
                    return new ResponseWrapper<List<int>>(
                        false,
                        (int)HttpStatusCode.Conflict,
                        $"Occurrence {occurrenceIndex + 1} ({occurrenceDate:yyyy-MM-dd}) is already booked for the requested time slot.",
                        default);
                }

                occurrenceUtcRanges.Add((occurrenceStartUtc, occurrenceEndUtc));
            }

            var series = new RecurringBookingSeries
            {
                RoomId = request.RoomId,
                UserId = userId,
                OccurrenceCount = request.OccurrenceCount
            };

            await recurringBookingSeriesRepository.AddAsync(series, cancellationToken);

            var bookings = occurrenceUtcRanges
                .Select(range => new Booking
                {
                    RoomId = request.RoomId,
                    UserId = userId,
                    StartUtc = range.StartUtc,
                    EndUtc = range.EndUtc,
                    AttendeeCount = request.AttendeeCount,
                    Notes = request.Notes,
                    Status = BookingStatus.Pending,
                    RecurringSeries = series
                })
                .ToList();

            foreach (var booking in bookings)
            {
                await bookingRepository.AddAsync(booking, cancellationToken);
            }

            var idempotencyRecord = new IdempotencyRecord
            {
                UserId = userId,
                Key = idempotencyKey,
                RequestHash = requestHash
            };

            await idempotencyRecordRepository.AddAsync(idempotencyRecord, cancellationToken);
            await bookingRepository.SaveChangesAsync(cancellationToken);

            var response = new ResponseWrapper<List<int>>(
                true,
                (int)HttpStatusCode.Created,
                "Recurring booking created successfully.",
                bookings.Select(booking => booking.Id).ToList());

            idempotencyRecord.ResponseStatusCode = response.StatusCode;
            idempotencyRecord.ResponseBody = JsonSerializer.Serialize(response);

            await bookingRepository.SaveChangesAsync(cancellationToken);

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
                    booking.Status,
                    booking.RecurringSeriesId))
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
                    booking.Status,
                    booking.RecurringSeriesId))
                .ToList();

            var response = new PagedResult<BookingResponse>(items, pagedBookings.TotalCount, page, pageSize);

            return new ResponseWrapper<PagedResult<BookingResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Bookings retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<BookingResponse>> GetById(int bookingId, int actingUserId, bool isAdmin, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetByIdWithDetails(bookingId, cancellationToken);

            if (booking is null)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Booking not found.",
                    default);
            }

            if (!isAdmin && booking.UserId != actingUserId)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Forbidden,
                    "You do not have permission to view this booking.",
                    default);
            }

            var response = new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.Room!.Name,
                booking.UserId,
                booking.User!.FullName,
                booking.StartUtc,
                booking.EndUtc,
                booking.AttendeeCount,
                booking.Notes,
                booking.Status,
                booking.RecurringSeriesId);

            return new ResponseWrapper<BookingResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Booking retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<List<BookingStatusHistoryResponse>>> GetHistory(int bookingId, CancellationToken cancellationToken)
        {
            var booking = await bookingRepository.GetByIdWithDetails(bookingId, cancellationToken);

            if (booking is null)
            {
                return new ResponseWrapper<List<BookingStatusHistoryResponse>>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Booking not found.",
                    default);
            }

            var history = await bookingRepository.GetStatusHistory(bookingId, cancellationToken);

            var response = history
                .Select(entry => new BookingStatusHistoryResponse(
                    entry.Id,
                    entry.PreviousStatus,
                    entry.NewStatus,
                    entry.ActingUserId,
                    entry.ActingUser!.FullName,
                    entry.Reason,
                    entry.CreatedAt))
                .ToList();

            return new ResponseWrapper<List<BookingStatusHistoryResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Booking history retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<BookingResponse>> Confirm(int bookingId, int adminUserId, CancellationToken cancellationToken)
        {
            var outcome = await bookingRepository.TryConfirm(bookingId, adminUserId, cancellationToken);

            if (outcome.Result == BookingConfirmationResult.NotFound)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Booking not found.",
                    default);
            }

            if (outcome.Result == BookingConfirmationResult.InvalidTransition)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "Only a pending booking can be confirmed.",
                    default);
            }

            if (outcome.Result == BookingConfirmationResult.Overlap)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "This room is already booked for the requested time slot.",
                    default);
            }

            var booking = outcome.Booking!;
            var response = new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.Room!.Name,
                booking.UserId,
                booking.User!.FullName,
                booking.StartUtc,
                booking.EndUtc,
                booking.AttendeeCount,
                booking.Notes,
                booking.Status,
                booking.RecurringSeriesId);

            return new ResponseWrapper<BookingResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Booking confirmed successfully.",
                response);
        }

        public async Task<ResponseWrapper<BookingResponse>> Reject(int bookingId, int adminUserId, BookingReasonRequest? request, CancellationToken cancellationToken)
        {
            var reason = request?.Reason;

            if (reason is not null && reason.Length > ValidatorConstants.StringLengths.ReasonMaxLength)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    $"Reason must not exceed {ValidatorConstants.StringLengths.ReasonMaxLength} characters.",
                    default);
            }

            var outcome = await bookingRepository.TryReject(bookingId, adminUserId, reason, cancellationToken);

            if (outcome.Result == BookingRejectionResult.NotFound)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Booking not found.",
                    default);
            }

            if (outcome.Result == BookingRejectionResult.InvalidTransition)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "Only a pending booking can be rejected.",
                    default);
            }

            var booking = outcome.Booking!;
            var response = new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.Room!.Name,
                booking.UserId,
                booking.User!.FullName,
                booking.StartUtc,
                booking.EndUtc,
                booking.AttendeeCount,
                booking.Notes,
                booking.Status,
                booking.RecurringSeriesId);

            return new ResponseWrapper<BookingResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Booking rejected successfully.",
                response);
        }

        public async Task<ResponseWrapper<BookingResponse>> Cancel(int bookingId, int actingUserId, bool isAdmin, BookingReasonRequest? request, CancellationToken cancellationToken)
        {
            var reason = request?.Reason;

            if (reason is not null && reason.Length > ValidatorConstants.StringLengths.ReasonMaxLength)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    $"Reason must not exceed {ValidatorConstants.StringLengths.ReasonMaxLength} characters.",
                    default);
            }

            var outcome = await bookingRepository.TryCancel(bookingId, actingUserId, isAdmin, reason, cancellationToken);

            if (outcome.Result == BookingCancellationResult.NotFound)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Booking not found.",
                    default);
            }

            if (outcome.Result == BookingCancellationResult.Forbidden)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Forbidden,
                    "You do not have permission to cancel this booking.",
                    default);
            }

            if (outcome.Result == BookingCancellationResult.InvalidTransition)
            {
                return new ResponseWrapper<BookingResponse>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "Only a pending or confirmed booking can be cancelled.",
                    default);
            }

            var booking = outcome.Booking!;
            var response = new BookingResponse(
                booking.Id,
                booking.RoomId,
                booking.Room!.Name,
                booking.UserId,
                booking.User!.FullName,
                booking.StartUtc,
                booking.EndUtc,
                booking.AttendeeCount,
                booking.Notes,
                booking.Status,
                booking.RecurringSeriesId);

            return new ResponseWrapper<BookingResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Booking cancelled successfully.",
                response);
        }
    }
}
