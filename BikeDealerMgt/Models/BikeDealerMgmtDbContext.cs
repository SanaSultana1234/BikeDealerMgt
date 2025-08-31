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

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<BikeStore> BikeStores { get; set; }

    public virtual DbSet<Dealer> Dealers { get; set; }

    public virtual DbSet<DealerMaster> DealerMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-5C1CRU1C;Initial Catalog=BikeDealerMgmtDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<BikeStore>(entity =>
        {
            entity.HasKey(e => e.BikeId).HasName("PK__BikeStor__7DC81721E1F67129");
        });

        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.HasKey(e => e.DealerId).HasName("PK__Dealers__CA2F8EB22063A87C");
        });

        modelBuilder.Entity<DealerMaster>(entity =>
        {
            entity.HasKey(e => e.DealerMasterId).HasName("PK__DealerMa__70FBBB3219C8158C");

            entity.HasOne(d => d.Bike).WithMany(p => p.DealerMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DM_Bike");

            entity.HasOne(d => d.Dealer).WithMany(p => p.DealerMasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_DM_Dealer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
