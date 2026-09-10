namespace HotelBooking.Constants
{
    public static class ValidatorConstants
    {
        public static class StringLengths
        {
            public const int FullNameMaxLength = 200;
            public const int EmailMaxLength = 256;
            public const int PasswordMinLength = 8;
            public const int PasswordMaxLength = 100;
            public const int RoomNameMaxLength = 200;
            public const int LocationMaxLength = 200;
            public const int DescriptionMaxLength = 1000;
            public const int NotesMaxLength = 500;
            public const int IdempotencyKeyMaxLength = 100;
        }

        public static class Numbers
        {
            public const int MinimumPositiveValue = 0;
            public const int BookingMinimumDurationMinutes = 30;
            public const int BookingMaximumDurationHours = 4;
            public const int MaximumAdvanceBookingDays = 30;

            public static readonly TimeSpan BookingMinimumDuration = TimeSpan.FromMinutes(BookingMinimumDurationMinutes);
            public static readonly TimeSpan BookingMaximumDuration = TimeSpan.FromHours(BookingMaximumDurationHours);
        }
    }
}
