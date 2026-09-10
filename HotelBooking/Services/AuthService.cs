using System.Net;
using FluentValidation;
using HotelBooking.Models.Entities;
using HotelBooking.Models.Enums;
using HotelBooking.Models.Requests;
using HotelBooking.Models.Responses;
using HotelBooking.Repositories.Interfaces;
using HotelBooking.Services.Interfaces;

namespace HotelBooking.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IValidator<RegisterUserRequest> registerUserRequestValidator,
        IValidator<LoginUserRequest> loginUserRequestValidator) : IAuthService
    {

        public async Task<ResponseWrapper<int>> Register(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            await registerUserRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var email = request.Email.ToLower();

            var existingUser = await userRepository.GetByEmail(email, cancellationToken);
            if (existingUser is not null)
            {
                return new ResponseWrapper<int>(
                    false,
                    (int)HttpStatusCode.Conflict,
                    "A user with this email already exists.",
                    default);
            }

            var user = new User
            {
                FullName = request.FullName,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserRole.Employee,
            };

            await userRepository.AddAsync(user, cancellationToken);
            await userRepository.SaveChangesAsync(cancellationToken);

            return new ResponseWrapper<int>(
                true,
                (int)HttpStatusCode.Created,
                "User registered successfully.",
                user.Id);
        }

        public async Task<ResponseWrapper<User>> Login(LoginUserRequest request, CancellationToken cancellationToken)
        {
            await loginUserRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

            var email = request.Email.ToLower();

            var user = await userRepository.GetByEmail(email, cancellationToken);

            if (user is null)
            {
                return new ResponseWrapper<User>(
                    false,
                    (int)HttpStatusCode.Unauthorized,
                    "Invalid email or password.",
                    default);
            }

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user?.PasswordHash);

            if (!isPasswordValid)
            {
                return new ResponseWrapper<User>(
                    false,
                    (int)HttpStatusCode.Unauthorized,
                    "Invalid email or password.",
                    default);
            }

            return new ResponseWrapper<User>(
                true,
                (int)HttpStatusCode.OK,
                "Login successful.", 
                user);
        }
    }
}
