using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EmployeeMgtCore
{
    public partial class EmployeeMgtCoreContext : DbContext
    {
        public EmployeeMgtCoreContext()
        {
        }

        public EmployeeMgtCoreContext(DbContextOptions<EmployeeMgtCoreContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Team> Teams { get; set; } = null!;
        public virtual DbSet<Tmember> Tmembers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=localhost\\MSSQLSERVER01;Database=EmployeeMgtCore;User ID=sa;Password=Messi1234*;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpId)
                    .HasName("PK__employee__128545C9AD896791");

                entity.ToTable("employee");

                entity.Property(e => e.EmpId).HasColumnName("emp_ID");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Fname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("fname");

                entity.Property(e => e.Lname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lname");

                entity.Property(e => e.Password)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.Phonenum)
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasColumnName("phonenum");

                entity.Property(e => e.RoleId).HasColumnName("role_ID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__employee__role_I__3B75D760");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.ToTable("promotion");

                entity.Property(e => e.PromotionId).HasColumnName("promotion_ID");

                entity.Property(e => e.Datecreated)
                    .HasColumnType("date")
                    .HasColumnName("datecreated");

                entity.Property(e => e.EmpId).HasColumnName("emp_Id");

                entity.Property(e => e.Newrole).HasColumnName("newrole");

                entity.Property(e => e.Oldrole).HasColumnName("oldrole");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("FK__promotion__emp_I__48CFD27E");

                entity.HasOne(d => d.NewroleNavigation)
                    .WithMany(p => p.Promotions)
                    .HasForeignKey(d => d.Newrole)
                    .HasConstraintName("FK__promotion__newro__49C3F6B7");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("role_ID");

                entity.Property(e => e.Rolename)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rolename");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.Property(e => e.TeamId).HasColumnName("team_ID");

                entity.Property(e => e.ManagerId).HasColumnName("manager_ID");

                entity.Property(e => e.Teamname)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("teamname");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK__team__manager_ID__3E52440B");
            });

            modelBuilder.Entity<Tmember>(entity =>
            {
                entity.HasKey(e => e.MemberId)
                    .HasName("PK__tmember__B29A816C685EFC37");

                entity.ToTable("tmember");

                entity.Property(e => e.MemberId).HasColumnName("member_ID");

                entity.Property(e => e.EmpId).HasColumnName("emp_ID");

                entity.Property(e => e.TeamId).HasColumnName("team_ID");

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.Tmembers)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("FK__tmember__emp_ID__412EB0B6");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Tmembers)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__tmember__team_ID__4222D4EF");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
