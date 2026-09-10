using System.Net;
using FluentValidation;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class MeetingRoomService(
        IMeetingRoomRepository roomRepository,
        IValidator<CreateMeetingRoomRequest> createMeetingRoomRequestValidator,
        IValidator<UpdateMeetingRoomRequest> updateMeetingRoomRequestValidator) : IMeetingRoomService
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
            var rooms = await roomRepository.GetAllForAdminAsync(cancellationToken);

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

        public async Task<ResponseWrapper<List<MeetingRoomsResponse>>> GetAllActive(CancellationToken cancellationToken)
        {
            var rooms = await roomRepository.GetAllActiveRooms(cancellationToken);

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
    }
}
