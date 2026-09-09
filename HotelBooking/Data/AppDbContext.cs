using HotelBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<RecurringBookingSeries> RecurringBookingSeries => Set<RecurringBookingSeries>();
        public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
        public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(user =>
            {
                user.Property(u => u.FullName).IsRequired().HasMaxLength(200);
                user.Property(u => u.Email).IsRequired().HasMaxLength(256);
                user.Property(u => u.PasswordHash).IsRequired();
                user.HasIndex(u => u.Email).IsUnique();
            });

            builder.Entity<MeetingRoom>(room =>
            {
                room.Property(r => r.Name).IsRequired().HasMaxLength(200);
                room.Property(r => r.Location).IsRequired().HasMaxLength(200);
                room.Property(r => r.Description).HasMaxLength(1000);
                room.HasIndex(r => new { r.Name, r.Location }).IsUnique();
            });

            builder.Entity<Booking>(booking =>
            {
                booking.Property(b => b.Notes).HasMaxLength(500);
                booking.Property(b => b.IdempotencyKey).HasMaxLength(100);
                booking.HasIndex(b => new { b.RoomId, b.StartUtc, b.EndUtc });

                booking.HasOne(b => b.Room)
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                booking.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                booking.HasOne(b => b.RecurringSeries)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(b => b.RecurringSeriesId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<RecurringBookingSeries>(series =>
            {
                series.HasOne(s => s.Room)
                    .WithMany()
                    .HasForeignKey(s => s.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                series.HasOne(s => s.User)
                    .WithMany(u => u.RecurringBookingSeries)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<BookingStatusHistory>(history =>
            {
                history.Property(h => h.Reason).HasMaxLength(500);

                history.HasOne(h => h.Booking)
                    .WithMany(b => b.StatusHistory)
                    .HasForeignKey(h => h.BookingId)
                    .OnDelete(DeleteBehavior.Cascade);

                history.HasOne(h => h.ActingUser)
                    .WithMany()
                    .HasForeignKey(h => h.ActingUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<IdempotencyRecord>(idempotency =>
            {
                idempotency.Property(i => i.Key).IsRequired().HasMaxLength(100);
                idempotency.HasIndex(i => i.Key).IsUnique();
            });
        }
    }
}
