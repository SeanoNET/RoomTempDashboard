using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RoomTempDashboard.Models
{
    public partial class HelloIotdbContext : DbContext
    {
        public HelloIotdbContext()
        {
        }

        public HelloIotdbContext(DbContextOptions<HelloIotdbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SensorData> SensorData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorData>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Humidity).HasColumnType("decimal(18, 13)");

                entity.Property(e => e.MeasuredAt)
                    .HasColumnName("Measured_At")
                    .HasColumnType("datetime");

                entity.Property(e => e.Temperature).HasColumnType("decimal(18, 13)");
            });
        }
    }
}
