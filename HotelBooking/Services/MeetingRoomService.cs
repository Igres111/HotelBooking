using System.Net;
using FluentValidation;
using HotelBooking.Constants;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class MeetingRoomService(
        IMeetingRoomRepository roomRepository,
        IBookingRepository bookingRepository,
        IValidator<CreateMeetingRoomRequest> createMeetingRoomRequestValidator,
        IValidator<UpdateMeetingRoomRequest> updateMeetingRoomRequestValidator,
        IValidator<GetRoomAvailabilityRequest> getRoomAvailabilityRequestValidator,
        ITimeZoneConverter timeZoneConverter) : IMeetingRoomService
    {
        public async Task<ResponseWrapper<int>> Create(CreateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            await createMeetingRoomRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var existingRoom = await roomRepository.GetByNameAndLocation(request.Name, request.Location, cancellationToken);
            if (existingRoom is not null)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "A meeting room with this name already exists at this location.",
                    default);
            }

            var room = new MeetingRoom
            {
                Name = request.Name,
                Description = request.Description,
                Location = request.Location,
                Capacity = request.Capacity,
                OpeningTime = request.OpeningTime,
                ClosingTime = request.ClosingTime
            };

            await roomRepository.AddAsync(room, cancellationToken);
            await roomRepository.SaveChangesAsync(cancellationToken);

            return new ResponseWrapper<int>(
                true,
                (int)HttpStatusCode.Created,
                "Meeting room created successfully.",
                room.Id);
        }

        public async Task<ResponseWrapper<List<MeetingRoomsResponse>>> GetAllForAdmin(CancellationToken cancellationToken)
        {
            var rooms = await roomRepository.GetAllAsync(cancellationToken);

            var response = rooms
                .Select(room => new MeetingRoomsResponse(
                    room.Id,
                    room.Name,
                    room.Description,
                    room.Location,
                    room.Capacity,
                    room.OpeningTime,
                    room.ClosingTime,
                    room.IsActive))
                .ToList();

            return new ResponseWrapper<List<MeetingRoomsResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Meeting rooms retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<PagedResult<MeetingRoomsResponse>>> GetAllActive(GetMeetingRoomsRequest request, CancellationToken cancellationToken)
        {
            var page = Math.Max(request.Page, 1);

            int NormalizePageSize(int pageSize) => pageSize >= 1 && pageSize <= PaginationConstants.MaxPageSize ? pageSize : PaginationConstants.DefaultPageSize;

            var pageSize = NormalizePageSize(request.PageSize);

            var pagedRooms = await roomRepository.GetActiveRoomsPaged(
                name: request.Name,
                location: request.Location,
                minCapacity: request.MinCapacity,
                sortBy: request.SortBy,
                sortDescending: request.SortDescending,
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken);

            var items = pagedRooms.Items
                .Select(room => new MeetingRoomsResponse(
                    room.Id,
                    room.Name,
                    room.Description,
                    room.Location,
                    room.Capacity,
                    room.OpeningTime,
                    room.ClosingTime,
                    room.IsActive))
                .ToList();

            var response = new PagedResult<MeetingRoomsResponse>(items, pagedRooms.TotalCount, page, pageSize);

            return new ResponseWrapper<PagedResult<MeetingRoomsResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Meeting rooms retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetActiveById(int id, CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetActiveById(id, cancellationToken);

            if (room is null)
            {
                return new ResponseWrapper<MeetingRoomsResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found.",
                    default);
            }

            var response = new MeetingRoomsResponse(
                room.Id,
                room.Name,
                room.Description,
                room.Location,
                room.Capacity,
                room.OpeningTime,
                room.ClosingTime,
                room.IsActive);

            return new ResponseWrapper<MeetingRoomsResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Meeting room retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> GetByIdForAdmin(int id, CancellationToken cancellationToken)
        {
            var room = await roomRepository.GetByIdAsync(id, cancellationToken);

            if (room is null)
            {
                return new ResponseWrapper<MeetingRoomsResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found.",
                    default);
            }

            var response = new MeetingRoomsResponse(
                room.Id,
                room.Name,
                room.Description,
                room.Location,
                room.Capacity,
                room.OpeningTime,
                room.ClosingTime,
                room.IsActive);

            return new ResponseWrapper<MeetingRoomsResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Meeting room retrieved successfully.",
                response);
        }

        public async Task<ResponseWrapper<MeetingRoomsResponse>> Update(int id, UpdateMeetingRoomRequest request, CancellationToken cancellationToken)
        {
            await updateMeetingRoomRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var room = await roomRepository.GetByIdAsync(id, cancellationToken);
            if (room is null)
            {
                return new ResponseWrapper<MeetingRoomsResponse>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found.",
                    default);
            }

            var mergedName = request.Name ?? room.Name;
            var mergedLocation = request.Location ?? room.Location;
            var mergedCapacity = request.Capacity ?? room.Capacity;
            var mergedOpeningTime = request.OpeningTime ?? room.OpeningTime;
            var mergedClosingTime = request.ClosingTime ?? room.ClosingTime;
            var mergedIsActive = request.IsActive ?? room.IsActive;

            if (mergedOpeningTime >= mergedClosingTime)
            {
                return new ResponseWrapper<MeetingRoomsResponse>(
                    false,
                    (int)HttpStatusCode.BadRequest,
                    "Opening time must be before closing time.",
                    default);
            }

            var conflictingRoom = await roomRepository.GetByNameAndLocation(mergedName, mergedLocation, cancellationToken);
            if (conflictingRoom is not null && conflictingRoom.Id != id)
            {
                return new ResponseWrapper<MeetingRoomsResponse>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "A meeting room with this name already exists at this location.",
                    default);
            }

            room.Name = mergedName;
            room.Description = request.Description ?? room.Description;
            room.Location = mergedLocation;
            room.Capacity = mergedCapacity;
            room.OpeningTime = mergedOpeningTime;
            room.ClosingTime = mergedClosingTime;
            room.IsActive = mergedIsActive;
            room.UpdatedAt = DateTime.UtcNow;

            await roomRepository.SaveChangesAsync(cancellationToken);

            var response = new MeetingRoomsResponse(
                room.Id,
                room.Name,
                room.Description,
                room.Location,
                room.Capacity,
                room.OpeningTime,
                room.ClosingTime,
                room.IsActive);

            return new ResponseWrapper<MeetingRoomsResponse>(
                true,
                (int)HttpStatusCode.OK,
                "Meeting room updated successfully.",
                response);
        }

        public async Task<ResponseWrapper<List<TimeSlotResponse>>> GetAvailability(int roomId, GetRoomAvailabilityRequest request, CancellationToken cancellationToken)
        {
            await getRoomAvailabilityRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var room = await roomRepository.GetActiveById(roomId, cancellationToken);
            if (room is null)
            {
                return new ResponseWrapper<List<TimeSlotResponse>>(
                    false,
                    (int)HttpStatusCode.NotFound,
                    "Meeting room not found or is not active.",
                    default);
            }

            var duration = TimeSpan.FromMinutes(request.DurationMinutes);
            var slotInterval = TimeSpan.FromMinutes(ValidatorConstants.Numbers.BookingMinimumDurationMinutes);
            var openingSpan = room.OpeningTime.ToTimeSpan();
            var closingSpan = room.ClosingTime.ToTimeSpan();

            var windowStartUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(room.OpeningTime), request.TimeZoneId);
            var windowEndUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(room.ClosingTime), request.TimeZoneId);

            var confirmedBookings = await bookingRepository.GetConfirmedBookingsInRange(roomId, windowStartUtc, windowEndUtc, cancellationToken);

            var availableSlots = new List<TimeSlotResponse>();
            var lastCandidateStartSpan = closingSpan - duration;

            for (var currentStart = openingSpan; currentStart <= lastCandidateStartSpan; currentStart += slotInterval)
            {
                var candidateStart = TimeOnly.FromTimeSpan(currentStart);
                var candidateEnd = TimeOnly.FromTimeSpan(currentStart + duration);

                var candidateStartUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(candidateStart), request.TimeZoneId);
                var candidateEndUtc = timeZoneConverter.ConvertToUtc(request.Date.ToDateTime(candidateEnd), request.TimeZoneId);

                if (candidateStartUtc <= DateTime.UtcNow)
                {
                    continue;
                }

                var hasOverlap = confirmedBookings.Any(booking =>
                    booking.StartUtc < candidateEndUtc &&
                    candidateStartUtc < booking.EndUtc);

                if (!hasOverlap)
                {
                    availableSlots.Add(new TimeSlotResponse(candidateStart, candidateEnd));
                }
            }

            return new ResponseWrapper<List<TimeSlotResponse>>(
                true,
                (int)HttpStatusCode.OK,
                "Availability retrieved successfully.",
                availableSlots);
        }
    }
}
