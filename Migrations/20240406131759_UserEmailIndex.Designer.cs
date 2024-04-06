﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using bretts_services.Models.EF;

#nullable disable

namespace bretts_services.Migrations
{
    [DbContext(typeof(BrettsAppContext))]
    [Migration("20240406131759_UserEmailIndex")]
    partial class UserEmailIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.Property<long>("RolesRoleID")
                        .HasColumnType("bigint");

                    b.Property<long>("RolesUserID")
                        .HasColumnType("bigint");

                    b.HasKey("RolesRoleID", "RolesUserID");

                    b.HasIndex("RolesUserID");

                    b.ToTable("RoleUser");
                });

            modelBuilder.Entity("bretts_services.Models.EF.Role", b =>
                {
                    b.Property<long>("RoleID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RoleID"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid>("RoleGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.HasKey("RoleID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleID = 1L,
                            Name = "Admin",
                            RoleGuid = new Guid("cdf2beff-ea73-4d8b-9fe8-33818e52776f")
                        },
                        new
                        {
                            RoleID = 2L,
                            Name = "User",
                            RoleGuid = new Guid("111224ad-f6a4-4ca1-ade2-2e6ab407d8e8")
                        });
                });

            modelBuilder.Entity("bretts_services.Models.EF.User", b =>
                {
                    b.Property<long>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("UserID"));

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETDATE()");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<byte[]>("Password")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varbinary(64)");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("varbinary(64)");

                    b.Property<Guid>("UserGuid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasDefaultValueSql("NEWID()");

                    b.HasKey("UserID");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("RoleUser", b =>
                {
                    b.HasOne("bretts_services.Models.EF.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesRoleID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("bretts_services.Models.EF.User", null)
                        .WithMany()
                        .HasForeignKey("RolesUserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
