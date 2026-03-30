using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WalletManagement.Core.Domain.Models;

public partial class idp_dtplatformContext : DbContext
{
    public idp_dtplatformContext()
    {
    }

    public idp_dtplatformContext(DbContextOptions<idp_dtplatformContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<Credential> Credentials { get; set; }

    public virtual DbSet<CredentialVerifier> CredentialVerifiers { get; set; }

    public virtual DbSet<ProvisionStatus> ProvisionStatuses { get; set; }

    public virtual DbSet<QrCredential> QrCredentials { get; set; }

    public virtual DbSet<QrCredentialVerifier> QrCredentialVerifiers { get; set; }

    public virtual DbSet<WalletConfiguration> WalletConfigurations { get; set; }

    public virtual DbSet<WalletConsent> WalletConsents { get; set; }

    public virtual DbSet<WalletDomain> WalletDomains { get; set; }

    public virtual DbSet<WalletPurpose> WalletPurposes { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("category_pkey");

            entity.ToTable("category");

            entity.HasIndex(e => e.CategoryUid, "category_category_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryUid)
                .HasMaxLength(50)
                .HasColumnName("category_uid");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(50)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Configuration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configuration_pkey");

            entity.ToTable("configuration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Hash)
                .HasMaxLength(260)
                .HasColumnName("hash");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Credential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("credential_pkey");

            entity.ToTable("credential");

            entity.HasIndex(e => e.CredentialUid, "credential_credential_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthenticationScheme)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("authentication_scheme");
            entity.Property(e => e.Categories).HasColumnName("categories");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("category_id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialName)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("credential_name");
            entity.Property(e => e.CredentialOffer).HasColumnName("credential_offer");
            entity.Property(e => e.CredentialUid)
                .HasMaxLength(50)
                .HasColumnName("credential_uid");
            entity.Property(e => e.DataAttributes).HasColumnName("data_attributes");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("display_name");
            entity.Property(e => e.Logo).HasColumnName("logo");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("organization_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.ServiceDetails).HasColumnName("service_details");
            entity.Property(e => e.SharingAuthenticationScheme)
                .HasMaxLength(50)
                .HasColumnName("sharing_authentication_scheme");
            entity.Property(e => e.SignedDocument).HasColumnName("signed_document");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TestVcData).HasColumnName("test_vc_data");
            entity.Property(e => e.TrustUrl).HasColumnName("trust_url");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.Validity).HasColumnName("validity");
            entity.Property(e => e.VerificationDocType)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("verification_doc_type");
            entity.Property(e => e.ViewingAuthenticationScheme)
                .HasMaxLength(50)
                .HasColumnName("viewing_authentication_scheme");
        });

        modelBuilder.Entity<CredentialVerifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("credential_verifiers_pkey");

            entity.ToTable("credential_verifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attributes).HasColumnName("attributes");
            entity.Property(e => e.Configuration).HasColumnName("configuration");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("credential_id");
            entity.Property(e => e.Domains).HasColumnName("domains");
            entity.Property(e => e.Emails).HasColumnName("emails");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("organization_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
            entity.Property(e => e.Validity).HasColumnName("validity");

            entity.HasOne(d => d.Credential).WithMany(p => p.CredentialVerifiers)
                .HasPrincipalKey(p => p.CredentialUid)
                .HasForeignKey(d => d.CredentialId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("credential_verifiers_credential_fk");
        });

        modelBuilder.Entity<ProvisionStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("provision_status_pkey");

            entity.ToTable("provision_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("credential_id");
            entity.Property(e => e.DocumentId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("document_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Suid)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("suid");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
        });

        modelBuilder.Entity<QrCredential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qr_credentials_pkey");

            entity.ToTable("qr_credentials");

            entity.HasIndex(e => e.CredentialUid, "qr_credentials_credential_uid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialName)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("credential_name");
            entity.Property(e => e.CredentialOffer).HasColumnName("credential_offer");
            entity.Property(e => e.CredentialUid)
                .HasMaxLength(50)
                .HasColumnName("credential_uid");
            entity.Property(e => e.DataAttributes).HasColumnName("data_attributes");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("display_name");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("organization_id");
            entity.Property(e => e.PortraitVerificationRequired).HasColumnName("portrait_verification_required");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TestVcData).HasColumnName("test_vc_data");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
        });

        modelBuilder.Entity<QrCredentialVerifier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("qr_credential_verifiers_pkey");

            entity.ToTable("qr_credential_verifiers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Attributes).HasColumnName("attributes");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("credential_id");
            entity.Property(e => e.Emails).HasColumnName("emails");
            entity.Property(e => e.OrganizationId)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("organization_id");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");

            entity.HasOne(d => d.Credential).WithMany(p => p.QrCredentialVerifiers)
                .HasPrincipalKey(p => p.CredentialUid)
                .HasForeignKey(d => d.CredentialId)
                .HasConstraintName("fk_qr_credential");
        });

        modelBuilder.Entity<WalletConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wallet_configuration_pkey");

            entity.ToTable("wallet_configuration");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedTime).HasColumnName("created_time");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("name");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedTime).HasColumnName("updated_time");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<WalletConsent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wallet_consent_pkey");

            entity.ToTable("wallet_consent");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId)
                .HasMaxLength(50)
                .HasColumnName("application_id");
            entity.Property(e => e.ConsentData).HasColumnName("consent_data");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.CredentialId)
                .HasMaxLength(50)
                .HasColumnName("credential_id");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Suid)
                .HasMaxLength(50)
                .HasColumnName("suid");
            entity.Property(e => e.UpdatedDate).HasColumnName("updated_date");
        });

        modelBuilder.Entity<WalletDomain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wallet_domains_pkey");

            entity.ToTable("wallet_domains");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("display_name");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Purposes).HasColumnName("purposes");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<WalletPurpose>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("wallet_purpose_pkey");

            entity.ToTable("wallet_purpose");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate).HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("display_name");
            entity.Property(e => e.ModifiedDate).HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL::character varying")
                .HasColumnName("updated_by");
        });
        modelBuilder.HasSequence("configuration_id_seq");
        modelBuilder.HasSequence("wallet_consent_id_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
