using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BikeDealerMgtAPI.Models;

public partial class BikeDealerMgmtDbContext : DbContext
{
    public BikeDealerMgmtDbContext()
    {
    }

    public BikeDealerMgmtDbContext(DbContextOptions<BikeDealerMgmtDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BikeStore> BikeStores { get; set; }

    public virtual DbSet<Dealer> Dealers { get; set; }

    public virtual DbSet<DealerMaster> DealerMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-5C1CRU1C;Initial Catalog=BikeDealerMgmtDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BikeStore>(entity =>
        {
            entity.HasKey(e => e.BikeId).HasName("PK__BikeStor__7DC81721E1F67129");

            entity.Property(e => e.EngineCc).HasColumnName("EngineCC");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.HasKey(e => e.DealerId).HasName("PK__Dealers__CA2F8EB22063A87C");

            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DealerName).HasMaxLength(100);
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ZipCode)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DealerMaster>(entity =>
        {
            entity.HasKey(e => e.DealerMasterId).HasName("PK__DealerMa__70FBBB3219C8158C");

            entity.ToTable("DealerMaster");

            entity.HasOne(d => d.Bike).WithMany(p => p.DealerMasters)
                .HasForeignKey(d => d.BikeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DM_Bike");

            entity.HasOne(d => d.Dealer).WithMany(p => p.DealerMasters)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DM_Dealer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
