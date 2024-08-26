using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace pizza_backend_api.Models;

public partial class EhrlichContext : DbContext
{
    public EhrlichContext()
    {
    }

    public EhrlichContext(DbContextOptions<EhrlichContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<PizzaPrice> PizzaPrices { get; set; }

    public virtual DbSet<PizzaType> PizzaTypes { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionDetail> TransactionDetails { get; set; }

    public virtual DbSet<VwCompletePizzaDetail> VwCompletePizzaDetails { get; set; }

    public virtual DbSet<VwCompletePizzaTypeDetail> VwCompletePizzaTypeDetails { get; set; }

    public virtual DbSet<VwCompleteTransactionDetail> VwCompleteTransactionDetails { get; set; }

    public virtual DbSet<VwMinimalTransactionDetail> VwMinimalTransactionDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SqlExpress;Database=Ehrlich;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Name });

            entity.ToTable("categories", "pizza");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PizzaPrice>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.TypeId, e.LongCode, e.SizeId }).HasName("PK_price");

            entity.ToTable("pizzaPrices", "pizza");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.LongCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<PizzaType>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Code, e.CategoryId }).HasName("PK_pizzatypes");

            entity.ToTable("pizzaTypes", "pizza");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.ToTable("sizes", "pizza");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_transactions_1");

            entity.ToTable("transactions", "order");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TransactionDetail>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.OrderId, e.PizzaLongCode }).HasName("PK_transactions");

            entity.ToTable("transactionDetails", "order");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.PizzaLongCode)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VwCompletePizzaDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_completePizzaDetail");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<VwCompletePizzaTypeDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_completePizzaTypeDetail");

            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwCompleteTransactionDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_completeTransactionDetails");

            entity.Property(e => e.Amount).HasColumnType("decimal(29, 2)");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Size)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwMinimalTransactionDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_minimalTransactionDetails");

            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(38, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
