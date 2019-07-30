using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Models
{
    public class BookingDbContext : DbContext
    {
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<MeetingAttendee> MeetingAttendees { get; set; }

        public BookingDbContext() : base(new DbContextOptions<BookingDbContext>())
        {
            //...
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=192.168.1.5; Initial Catalog=BookingManagement; User Id=sa; Password=asd123!@#");

            optionsBuilder.EnableSensitiveDataLogging(true);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Attendee>(OnAttendeeConfiguration);
            modelBuilder.Entity<Meeting>(OnMeetingConfiguration);
            modelBuilder.Entity<MeetingAttendee>(OnMeetingAttendeeConfiguration);

            base.OnModelCreating(modelBuilder);
        }

        private void OnAttendeeConfiguration(EntityTypeBuilder<Attendee> entity)
        {
            entity.ToTable("Attendee");

            entity.HasKey(e => e.AttendeeID);
        }

        private void OnMeetingConfiguration(EntityTypeBuilder<Meeting> entity)
        {
            entity.ToTable("Meetings");

            entity.HasKey(e => e.MeetingID);

            entity.Property(e => e.End)
                .IsRequired()
                .HasColumnType("DATETIME");

            entity.Property(e => e.IsAllDay)
                .IsRequired()
                .HasColumnType("BOOLEAN");

            entity.Property(e => e.RecurrenceID)
                .HasColumnType("INT")
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.RoomID)
                .HasColumnType("INT")
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Start)
                .IsRequired()
                .HasColumnType("DATETIME");

            entity.Property(e => e.Title).IsRequired();

            entity.HasOne(d => d.Recurrence).WithMany(p => p.InverseRecurrence).HasForeignKey(d => d.RecurrenceID);
        }

        private void OnMeetingAttendeeConfiguration(EntityTypeBuilder<MeetingAttendee> entity)
        {
            entity.ToTable("MeetingAttendees");

            entity.HasKey(e => new { e.MeetingID, e.AttendeeID });

            entity.Property(e => e.MeetingID).HasColumnType("INT").ValueGeneratedNever();

            entity.Property(e => e.AttendeeID).HasColumnType("INT").ValueGeneratedNever();

            entity.HasOne(d => d.Meeting).WithMany(p => p.MeetingAttendees).HasForeignKey(d => d.MeetingID).OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<BookingManagement.Models.ViewModels.ChangePasswordViewModel> ChangePasswordViewModel { get; set; }
    }
}
