using MeetUpHubV2.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // BU SATIR EKLENDİ
using Microsoft.EntityFrameworkCore;

namespace MeetUpHubV2.DataAccess
{
    // SINIF MİRASI DEĞİŞTİ:
    public class MeetUpHubV2DbContext : IdentityDbContext<User, Role, int>
    {
        // YENİ CONSTRUCTOR EKLENDİ (Dependency Injection için)
        public MeetUpHubV2DbContext(DbContextOptions<MeetUpHubV2DbContext> options) : base(options)
        {
        }

        // OnConfiguring METODU SİLİNDİ
        // (Bağlantı dizesini artık buradan değil, Program.cs'ten alacağız)
        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("...");
        }
        */
        
        // OnModelCreating METODUN AYENEN KALIYOR (HARİKA!)
        // base.OnModelCreating'in başta olması Identity için de önemlidir.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Document)
                .WithOne(d => d.User)
                .HasForeignKey<UserDocument>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserDocument>()
                .HasIndex(d => d.UserId)
                .IsUnique();

            modelBuilder.Entity<UserRoom>()
                .HasKey(ur => new { ur.UserId, ur.RoomId });

            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRooms)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRoom>()
                .HasOne(ur => ur.Room)
                .WithMany(r => r.UserRooms)
                .HasForeignKey(ur => ur.RoomId);
        }

        // BU SATIR SİLİNDİ, çünkü IdentityDbContext'ten zaten geliyor:
        // public DbSet<User> Users { get; set; }

        // DİĞER TÜM DbSet'LERİN AYENEN KALIYOR:
        public DbSet<UserDocument> UserDocuments { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; } //Odalara katıulan kullanıcı bilgileri.
        public DbSet<Venue> Venues { get; set; } //Mekanlar tablosu.
        public DbSet<Event> Events { get; set; } //Etkinlik tablosu
    }
}