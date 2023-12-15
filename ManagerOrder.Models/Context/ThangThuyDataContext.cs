using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ManagerOrder.Models.Entities;
using System.IO;

#nullable disable

namespace ManagerOrder.Models.Context
{
    public partial class ThangThuyDataContext : DbContext
    {
        public ThangThuyDataContext()
        {
        }

        public ThangThuyDataContext(DbContextOptions<ThangThuyDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<HistoryOrder> HistoryOrders { get; set; }
        public virtual DbSet<HistoryOrderDetail> HistoryOrderDetails { get; set; }
        public virtual DbSet<RegisterCustomer> RegisterCustomers { get; set; }
        public virtual DbSet<RegisterProduct> RegisterProducts { get; set; }
        public virtual DbSet<Unit> Units { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //string targetDbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Database\\BobDB.db");

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@"Data Source=D:\LeTheAnh\RTC\Project\2023\ManagerOrder-master_151223\ManagerOrder-master\ManagerOrder.Models\Database\ThangThuyData.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HistoryOrder>(entity =>
            {
                entity.ToTable("HistoryOrder");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            });

            modelBuilder.Entity<HistoryOrderDetail>(entity =>
            {
                entity.ToTable("HistoryOrderDetail");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.HistoryOrderId).HasColumnName("HistoryOrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");
            });

            modelBuilder.Entity<RegisterCustomer>(entity =>
            {
                entity.ToTable("RegisterCustomer");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<RegisterProduct>(entity =>
            {
                entity.ToTable("RegisterProduct");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.ToTable("Unit");

                entity.Property(e => e.Id).HasColumnName("ID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName, "IX_Users_UserName")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.UserName).IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
