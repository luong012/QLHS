using Microsoft.EntityFrameworkCore;
using QLHS_Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLHS_Admin.Data
{
    public class QLHSContext:DbContext
    {

        public QLHSContext(DbContextOptions<QLHSContext> options)
        : base(options)
        {
        }

        //private const string connectionString = "server=localhost;port=3306;database=homestay;uid=qlhs,;password=qlhs";
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseMySQL(connectionString);

        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>()
                .HasKey(b => b.Bill_ID)
                .HasName("PrimaryKey_Bill_ID");

            modelBuilder.Entity<Customer>()
                .HasKey(b => b.Cus_ID)
                .HasName("PrimaryKey_Cus_ID");

            modelBuilder.Entity<Employee>()
                .HasKey(b => b.Emp_ID)
                .HasName("PrimaryKey_Emp_ID");

            modelBuilder.Entity<Facility>()
                .HasKey(b => b.Fac_ID)
                .HasName("PrimaryKey_Fac_ID");
            //modelBuilder.Entity<Facility>()
            //    .HasMany(f => f.Rooms)
            //    .WithOne(r => r.Facility);

            modelBuilder.Entity<Feature>()
                .HasKey(b => b.Feature_ID)
                .HasName("PrimaryKey_Feature_ID");

            modelBuilder.Entity<Room>()
                .HasKey(b => b.Room_ID)
                .HasName("PrimaryKey_Room_ID");
            modelBuilder.Entity<Room>()
                .Property(b => b.Room_ID)
                .ValueGeneratedOnAdd();
            //modelBuilder.Entity<Room>()
            //    .HasOne(r => r.Facility)
            //    .WithMany(f => f.Rooms);

            modelBuilder.Entity<RoomBooking>()
                .HasKey(b => new { b.Room_ID, b.Booking_Date })
                .HasName("PrimaryKey_RBK");

            modelBuilder.Entity<RoomType>()
                .HasKey(b => b.RoomType_ID)
                .HasName("PrimaryKey_RoomType_ID");

            modelBuilder.Entity<RoomType_Feature>()
                .HasKey(b => new { b.RoomType_ID, b.Feature_ID })
                .HasName("PrimaryKey_RoomTypeFT_ID");
            modelBuilder.Entity<RoomType_Feature>()
                .HasOne(rf => rf.RoomType)
                .WithMany(r => r.RoomType_Feature)
                .HasForeignKey(rf => rf.RoomType_ID);
            modelBuilder.Entity<RoomType_Feature>()
                .HasOne(rf => rf.Feature)
                .WithMany(f => f.RoomType_Feature)
                .HasForeignKey(rf => rf.Feature_ID);

            modelBuilder.Entity<Service>()
                .HasKey(b => b.Service_ID)
                .HasName("PrimaryKey_Service_ID");
            modelBuilder.Entity<Service>()
                .Property(b => b.Service_ID)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Service_Detail>()
                .HasKey(b => new { b.Service_ID, b.Bill_ID })
                .HasName("PrimaryKey_Service_Detail");

            modelBuilder.Entity<UserAcc>()
                .HasKey(b => b.User_ID)
                .HasName("PrimaryKey_User_ID");


        }

        public DbSet<Bill> Bill { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Facility> Facility { get; set; }
        public DbSet<Feature> Feature { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomBooking> RoomBooking { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<RoomType_Feature> RoomType_Feature { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Service_Detail> Service_Detail { get; set; }
        public DbSet<UserAcc> UserAcc { get; set; }
    }
}
