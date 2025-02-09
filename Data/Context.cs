using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<UserForHotel> UserForHotels { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<BillingInfo> BillingInfos { get; set; }
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
            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(entity => entity.User)
                .WithMany(entity => entity.RefreshTokens)
                .HasForeignKey(entity => entity.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Reviews>(entity =>
            {
                entity.HasKey(entity => entity.Id);
                entity.HasOne(entity => entity.User)
                .WithMany(entity => entity.Reviews)
                .HasForeignKey(entity => entity.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(entity => entity.Hotel)
                .WithMany(entity => entity.Reviews)
                .HasForeignKey(entity => entity.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<BillingInfo>(entity =>
            {
                entity.HasKey(entity => entity.Id);
                entity.HasOne(entity => entity.User)
                .WithMany(entity => entity.BillingInfos)
                .HasForeignKey(entity => entity.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(entity => entity.Hotel)
                .WithMany(entity => entity.BillingInfos)
                .HasForeignKey(entity => entity.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
