﻿// <auto-generated />
using System;
using DoneTool.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DoneTool.Migrations
{
    [DbContext(typeof(DoneToolContext))]
    [Migration("20240914153454_AddIsDuplicateToTaskChecklist")]
    partial class AddIsDuplicateToTaskChecklist
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DoneTool.Models.Domain.CheckSkipReason", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("ID");

                    b.ToTable("CheckSkipReasons");
                });

            modelBuilder.Entity("DoneTool.Models.Domain.Checks", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Item")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Checks");
                });

            modelBuilder.Entity("DoneTool.Models.Domain.TaskChecklist", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Guard")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDuplicate")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("TaskChecksID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TaskID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("TaskChecklist");
                });

            modelBuilder.Entity("DoneTool.Models.Domain.TaskChecks", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CheckID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Step")
                        .HasColumnType("int");

                    b.Property<string>("TaskType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("TaskChecks");
                });
#pragma warning restore 612, 618
        }
    }
}
