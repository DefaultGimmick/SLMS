using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SLMS.Models.Entities;

namespace SLMS.Models.SLMS.EntityFrameworkCore
{
    public partial class SLMSDBContext : DbContext
    {
        public SLMSDBContext(DbContextOptions<SLMSDBContext> options)
            : base(options)
        {
        }


        public DbSet<EntityBook> Books { get; set; }
        public DbSet<EntityInventory> Inventorys { get; set; }
        public DbSet<EntityUser> Users{ get; set; }
        public DbSet<EntityCategory> Categorys { get; set; }
        public DbSet<EntityBorrowRecord> BorrowRecords { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.EntityUser>(entiy =>
            {
                entiy.ToTable("TB_User");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Entities.EntityBook>(entiy =>
            {
                entiy.ToTable("TB_Book");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Entities.EntityInventory>(entiy =>
            {
                entiy.ToTable("TB_Inventory");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Entities.EntityCategory>(entiy =>
            {
                entiy.ToTable("TB_Category");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Entities.EntityBorrowRecord>(entiy =>
            {
                entiy.ToTable("TB_BorrowRecord");
                entiy.HasKey("Id");
                entiy.Property(c => c.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<EntityBook>().HasOne(e => e.Inventory).WithOne(e => e.Book).HasForeignKey<EntityInventory>(e => e.BookId);
            modelBuilder.Entity<EntityBook>().HasOne(e => e.User).WithMany(e => e.Books).HasForeignKey(e => e.UserId);
            modelBuilder.Entity<EntityBook>().HasOne(e => e.Category).WithMany(e => e.Books).HasForeignKey(e => e.CategoryId);
            modelBuilder.Entity<EntityBorrowRecord>().HasOne(e => e.Book).WithMany(e => e.BorrowRecords).HasForeignKey(e => e.BookId);
            modelBuilder.Entity<EntityBorrowRecord>().HasOne(e => e.User).WithMany(e => e.BorrowRecords).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
