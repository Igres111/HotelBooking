using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<UserForHotel> UserForHotels { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(entity => entity.Id);
            modelBuilder.Entity<UserForHotel>(entity =>
            {
                entity.HasKey(entity => new { entity.UserId,entity.HotelId });
                entity.HasOne(entity => entity.User)
               .WithMany(entity => entity.UserForHotels)
               .HasForeignKey(entity => entity.UserId)
               .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(entity => entity.Hotel)
               .WithMany(entity => entity.UserForHotels)
               .HasForeignKey(entity => entity.HotelId)
               .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<RefreshToken>(entity =>
                {
                    entity.HasOne(entity => entity.User)
                    .WithMany(entity => entity.RefreshTokens)
                    .HasForeignKey(entity => entity.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                });
            });
        }
    }
}
