using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Swachify.Infrastructure.Models;

namespace Swachify.Infrastructure.Data;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<customer_complaint> customer_complaints { get; set; }

    public virtual DbSet<email_template> email_templates { get; set; }

    public virtual DbSet<master_department> master_departments { get; set; }

    public virtual DbSet<master_gender> master_genders { get; set; }

    public virtual DbSet<master_location> master_locations { get; set; }

    public virtual DbSet<master_role> master_roles { get; set; }

    public virtual DbSet<master_service> master_services { get; set; }

    public virtual DbSet<master_service_mapping> master_service_mappings { get; set; }

    public virtual DbSet<master_service_type> master_service_types { get; set; }

    public virtual DbSet<master_slot> master_slots { get; set; }

    public virtual DbSet<master_status> master_statuses { get; set; }

    public virtual DbSet<otp_history> otp_histories { get; set; }

    public virtual DbSet<service_booking> service_bookings { get; set; }

    public virtual DbSet<service_tracking> service_trackings { get; set; }

    public virtual DbSet<user_auth> user_auths { get; set; }

    public virtual DbSet<user_department> user_departments { get; set; }

    public virtual DbSet<user_registration> user_registrations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=13.211.191.251;Port=5432;Database=swachify_dev;Username=postgres;Password=admin@123\n");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<customer_complaint>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_customer_complaints_id");

            entity.HasIndex(e => e.created_by, "idx_customer_complaints_created_by");

            entity.HasIndex(e => e.location_id, "idx_customer_complaints_location_id");

            entity.HasIndex(e => e.modified_by, "idx_customer_complaints_modified_by");

            entity.HasIndex(e => e.user_id, "idx_customer_complaints_user_id");

            entity.Property(e => e.address).HasColumnType("character varying");
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.customer_complaintcreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_customer_complaints_created_by");

            entity.HasOne(d => d.location).WithMany(p => p.customer_complaints)
                .HasForeignKey(d => d.location_id)
                .HasConstraintName("fk_customer_complaints_location_id");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.customer_complaintmodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_customer_complaints_modified_by");

            entity.HasOne(d => d.user).WithMany(p => p.customer_complaintusers)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_customer_complaints_user_id");
        });

        modelBuilder.Entity<email_template>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_email_template_id");

            entity.ToTable("email_template");

            entity.HasIndex(e => e.created_by, "idx_email_template_created_by");

            entity.HasIndex(e => e.modified_by, "idx_email_template_modified_by");

            entity.HasIndex(e => e.title, "uk_email_template_title").IsUnique();

            entity.Property(e => e.id).HasDefaultValueSql("nextval('booking_template_id_seq'::regclass)");
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.title).HasMaxLength(255);

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.email_templatecreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_email_template_created_by");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.email_templatemodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_email_template_modified_by");
        });

        modelBuilder.Entity<master_department>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_department_id");

            entity.ToTable("master_department");

            entity.HasIndex(e => e.department_name, "uk_master_department_department_name").IsUnique();

            entity.Property(e => e.department_name).HasMaxLength(255);
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<master_gender>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_gender_id");

            entity.ToTable("master_gender");

            entity.HasIndex(e => e.gender_name, "uk_master_gender_gender_name").IsUnique();

            entity.Property(e => e.gender_name).HasMaxLength(255);
            entity.Property(e => e.is_active).HasDefaultValue(true);
        });

        modelBuilder.Entity<master_location>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_location_id");

            entity.ToTable("master_location");

            entity.HasIndex(e => e.location_name, "uk_master_location_location_name").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.location_name).HasMaxLength(255);
        });

        modelBuilder.Entity<master_role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_role_id");

            entity.ToTable("master_role");

            entity.HasIndex(e => e.role_name, "uk_master_role_role_name").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.role_name).HasMaxLength(255);
        });

        modelBuilder.Entity<master_service>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_service_id");

            entity.ToTable("master_service");

            entity.HasIndex(e => e.dept_id, "idx_master_service_dept_id");

            entity.HasIndex(e => new { e.service_name, e.dept_id }, "uk_master_service_service_name").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.service_name).HasMaxLength(255);

            entity.HasOne(d => d.dept).WithMany(p => p.master_services)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("fk_master_service_dept_id");
        });

        modelBuilder.Entity<master_service_mapping>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_service_mapping_id");

            entity.ToTable("master_service_mapping");

            entity.HasIndex(e => e.service_id, "idx_master_service_mapping_service_id");

            entity.HasIndex(e => e.service_type_id, "idx_master_service_mapping_service_type_id");

            entity.HasIndex(e => new { e.service_type_id, e.service_id }, "uk_master_service_mapping_service_id_service_type_id").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);

            entity.HasOne(d => d.service).WithMany(p => p.master_service_mappings)
                .HasForeignKey(d => d.service_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_master_service_mapping_service_id");

            entity.HasOne(d => d.service_type).WithMany(p => p.master_service_mappings)
                .HasForeignKey(d => d.service_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_master_service_mapping_service_type_id");
        });

        modelBuilder.Entity<master_service_type>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_service_type_id");

            entity.ToTable("master_service_type");

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.service_type).HasMaxLength(255);
        });

        modelBuilder.Entity<master_slot>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_slots_id");

            entity.HasIndex(e => e.slot_time, "uk_master_slots_slot_time").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.slot_time).HasMaxLength(255);
        });

        modelBuilder.Entity<master_status>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_master_status_id");

            entity.ToTable("master_status");

            entity.HasIndex(e => e.status, "uk_master_status_status").IsUnique();

            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.status).HasMaxLength(255);
        });

        modelBuilder.Entity<otp_history>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_otp_history_id");

            entity.ToTable("otp_history");

            entity.HasIndex(e => e.created_by, "idx_otp_history_created_by");

            entity.HasIndex(e => e.modified_by, "idx_otp_history_modified_by");

            entity.HasIndex(e => e.user_id, "idx_otp_history_user_id");

            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.booking).WithMany(p => p.otp_histories)
                .HasForeignKey(d => d.booking_id)
                .HasConstraintName("fk_otp_history_booking_id");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.otp_historycreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_otp_history_created_by");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.otp_historymodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_otp_history_modified_by");

            entity.HasOne(d => d.user).WithMany(p => p.otp_historyusers)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_otp_history_user_id");
        });

        modelBuilder.Entity<service_booking>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_service_booking_id");

            entity.ToTable("service_booking");

            entity.HasIndex(e => e.created_by, "idx_service_booking_created_by");

            entity.HasIndex(e => e.modified_by, "idx_service_booking_modified_by");

            entity.HasIndex(e => e.service_type_id, "idx_service_booking_service_type_id");

            entity.HasIndex(e => e.slot_id, "idx_service_booking_slot_id");

            entity.HasIndex(e => e.status_id, "idx_service_booking_status_id");

            entity.Property(e => e.address).HasMaxLength(500);
            entity.Property(e => e.booking_id).HasMaxLength(100);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.customer_requested_amount).HasPrecision(10, 2);
            entity.Property(e => e.discount_amount).HasPrecision(10, 2);
            entity.Property(e => e.discount_percentage).HasPrecision(10, 2);
            entity.Property(e => e.discount_total).HasPrecision(10, 2);
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.full_name).HasMaxLength(255);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.phone).HasMaxLength(15);
            entity.Property(e => e.subtotal).HasPrecision(10, 2);
            entity.Property(e => e.total).HasPrecision(10, 2);

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.service_bookingcreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_customer_complaints_created_by");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.service_bookingmodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_customer_complaints_modified_by");

            entity.HasOne(d => d.service_type).WithMany(p => p.service_bookings)
                .HasForeignKey(d => d.service_type_id)
                .HasConstraintName("fk_service_booking_service_type_id");

            entity.HasOne(d => d.slot).WithMany(p => p.service_bookings)
                .HasForeignKey(d => d.slot_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_service_booking_slot_id");

            entity.HasOne(d => d.status).WithMany(p => p.service_bookings)
                .HasForeignKey(d => d.status_id)
                .HasConstraintName("fk_service_booking_status_id");
        });

        modelBuilder.Entity<service_tracking>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_service_tracking_id");

            entity.ToTable("service_tracking");

            entity.HasIndex(e => e.dept_id, "idx_service_tracking_dept_id");

            entity.HasIndex(e => e.service_booking_id, "idx_service_tracking_service_booking_id");

            entity.HasIndex(e => e.service_id, "idx_service_tracking_service_id");

            entity.HasIndex(e => e.service_type_id, "idx_service_tracking_service_type_id");

            entity.HasIndex(e => e.status_id, "idx_service_tracking_status_id");

            entity.Property(e => e.booking_id).HasMaxLength(100);
            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.room_sqfts).HasMaxLength(100);
            entity.Property(e => e.with_basement_price).HasPrecision(10, 2);

            entity.HasOne(d => d.dept).WithMany(p => p.service_trackings)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("fk_service_tracking_dept_id");

            entity.HasOne(d => d.service_booking).WithMany(p => p.service_trackings)
                .HasForeignKey(d => d.service_booking_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_service_tracking_service_booking_id");

            entity.HasOne(d => d.service).WithMany(p => p.service_trackings)
                .HasForeignKey(d => d.service_id)
                .HasConstraintName("fk_service_tracking_service_id");

            entity.HasOne(d => d.service_type).WithMany(p => p.service_trackings)
                .HasForeignKey(d => d.service_type_id)
                .HasConstraintName("fk_service_tracking_service_type_id");

            entity.HasOne(d => d.status).WithMany(p => p.service_trackings)
                .HasForeignKey(d => d.status_id)
                .HasConstraintName("fk_service_tracking_status_id");
        });

        modelBuilder.Entity<user_auth>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_user_auth_id");

            entity.ToTable("user_auth");

            entity.HasIndex(e => e.created_by, "idx_user_auth_created_by");

            entity.HasIndex(e => e.modified_by, "idx_user_auth_modified_by");

            entity.HasIndex(e => e.user_id, "idx_user_auth_user_id");

            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.password).HasMaxLength(500);

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.user_authcreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_user_auth_created_by");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.user_authmodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_user_auth_modified_by");

            entity.HasOne(d => d.user).WithMany(p => p.user_authusers)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_auth_user_id");
        });

        modelBuilder.Entity<user_department>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_user_department_id");

            entity.ToTable("user_department");

            entity.HasIndex(e => e.created_by, "idx_user_department_created_by");

            entity.HasIndex(e => e.dept_id, "idx_user_department_dept_id");

            entity.HasIndex(e => e.modified_by, "idx_user_department_modified_by");

            entity.HasIndex(e => e.user_id, "idx_user_department_user_id");

            entity.HasIndex(e => new { e.dept_id, e.user_id }, "uk_user_department_dept_id_user_id").IsUnique();

            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.created_byNavigation).WithMany(p => p.user_departmentcreated_byNavigations)
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_user_department_created_by");

            entity.HasOne(d => d.dept).WithMany(p => p.user_departments)
                .HasForeignKey(d => d.dept_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_department_dept_id");

            entity.HasOne(d => d.modified_byNavigation).WithMany(p => p.user_departmentmodified_byNavigations)
                .HasForeignKey(d => d.modified_by)
                .HasConstraintName("fk_user_department_modified_by");

            entity.HasOne(d => d.user).WithMany(p => p.user_departmentusers)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_user_department_user_id");
        });

        modelBuilder.Entity<user_registration>(entity =>
        {
            entity.HasKey(e => e.id).HasName("pk_user_registration_id");

            entity.ToTable("user_registration");

            entity.HasIndex(e => e.dept_id, "idx_user_registration_dept_id");

            entity.HasIndex(e => e.gender_id, "idx_user_registration_gender_id");

            entity.HasIndex(e => e.location_id, "idx_user_registration_location_id");

            entity.HasIndex(e => e.role_id, "idx_user_registration_role_id");

            entity.HasIndex(e => e.service_id, "idx_user_registration_service_id");

            entity.HasIndex(e => e.email, "uk_user_registration_email").IsUnique();

            entity.HasIndex(e => e.mobile, "uk_user_registration_mobile").IsUnique();

            entity.Property(e => e.created_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.first_name).HasMaxLength(255);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.last_name).HasMaxLength(255);
            entity.Property(e => e.mobile).HasMaxLength(15);
            entity.Property(e => e.modified_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.dept).WithMany(p => p.user_registrations)
                .HasForeignKey(d => d.dept_id)
                .HasConstraintName("fk_user_registration_dept_id");

            entity.HasOne(d => d.gender).WithMany(p => p.user_registrations)
                .HasForeignKey(d => d.gender_id)
                .HasConstraintName("fk_user_registration_gender_id");

            entity.HasOne(d => d.location).WithMany(p => p.user_registrations)
                .HasForeignKey(d => d.location_id)
                .HasConstraintName("fk_user_registration_location_id");

            entity.HasOne(d => d.role).WithMany(p => p.user_registrations)
                .HasForeignKey(d => d.role_id)
                .HasConstraintName("fk_user_registration_role_id");

            entity.HasOne(d => d.service).WithMany(p => p.user_registrations)
                .HasForeignKey(d => d.service_id)
                .HasConstraintName("fk_user_registration_service_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
