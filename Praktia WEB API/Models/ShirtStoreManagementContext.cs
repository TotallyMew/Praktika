using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Praktia_WEB_API.Models
{
	public partial class ShirtStoreManagementContext : DbContext
	{
		public ShirtStoreManagementContext()
		{
		}

		public ShirtStoreManagementContext(DbContextOptions<ShirtStoreManagementContext> options)
			: base(options)
		{
		}

		public virtual DbSet<NotificationSmsBillingInformation> NotificationSmsBillingInformations { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			=> optionsBuilder.UseSqlServer("Server=BSS-PC-31;Database=ShirtStoreManagement;Trusted_Connection=True;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<NotificationSmsBillingInformation>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("PK_Notification_SmsBillingInformation_Id");

				entity.ToTable("Notification_SmsBillingInformation");

				entity.Property(e => e.ActionDate).HasColumnType("datetime");
				entity.Property(e => e.ActionType).HasMaxLength(254);
				entity.Property(e => e.PhoneNumber).HasMaxLength(64);
				entity.Property(e => e.RegistrationInfo).HasMaxLength(4000);
				entity.Property(e => e.SmsProviderInfo).HasMaxLength(254);
				entity.Property(e => e.SmsProviderName).HasMaxLength(64);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
