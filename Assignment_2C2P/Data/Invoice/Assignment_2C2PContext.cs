using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Assignment_2C2P.Data.Invoice
{
    public partial class Assignment_2C2PContext : DbContext
    {
        public Assignment_2C2PContext()
        {
        }

        public Assignment_2C2PContext(DbContextOptions<Assignment_2C2PContext> options)
            : base(options)
        {
        }

        public virtual DbSet<InvoiceTransaction> InvoiceTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);

                entity.ToTable("Invoice_transaction");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("transaction_id");

                entity.Property(e => e.Amount)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("amount");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasColumnName("created_date");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasColumnName("currency");

                entity.Property(e => e.InputType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("input_type");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("status");

                entity.Property(e => e.TransactionDate)
                    .HasColumnType("datetime")
                    .HasColumnName("transaction_date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
