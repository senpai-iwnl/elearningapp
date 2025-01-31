﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using e_learning_app.Data;

#nullable disable

namespace e_learning_app.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250131143835_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.HasKey("GroupId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("GroupUser");
                });

            modelBuilder.Entity("e_learning_app.Models.Class", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("e_learning_app.Models.Document", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ClassId")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("File")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("SubjectId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ClassId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("SubjectId");

                    b.ToTable("Documents");
                });

            modelBuilder.Entity("e_learning_app.Models.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("e_learning_app.Models.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("e_learning_app.Models.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("TestId")
                        .HasColumnType("uuid");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TestId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("e_learning_app.Models.Subject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("e_learning_app.Models.Test", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("e_learning_app.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("e_learning_app.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("e_learning_app.Models.User", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("e_learning_app.Models.Class", b =>
                {
                    b.HasOne("e_learning_app.Models.Subject", "Subject")
                        .WithMany("Classes")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("e_learning_app.Models.Document", b =>
                {
                    b.HasOne("e_learning_app.Models.Class", "Class")
                        .WithMany("Documents")
                        .HasForeignKey("ClassId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("e_learning_app.Models.Project", null)
                        .WithMany("Files")
                        .HasForeignKey("ProjectId");

                    b.HasOne("e_learning_app.Models.Subject", "Subject")
                        .WithMany("Documents")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Class");

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("e_learning_app.Models.Project", b =>
                {
                    b.HasOne("e_learning_app.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });

            modelBuilder.Entity("e_learning_app.Models.Question", b =>
                {
                    b.HasOne("e_learning_app.Models.Test", null)
                        .WithMany("Questions")
                        .HasForeignKey("TestId");
                });

            modelBuilder.Entity("e_learning_app.Models.Test", b =>
                {
                    b.HasOne("e_learning_app.Models.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("e_learning_app.Models.Class", b =>
                {
                    b.Navigation("Documents");
                });

            modelBuilder.Entity("e_learning_app.Models.Project", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("e_learning_app.Models.Subject", b =>
                {
                    b.Navigation("Classes");

                    b.Navigation("Documents");
                });

            modelBuilder.Entity("e_learning_app.Models.Test", b =>
                {
                    b.Navigation("Questions");
                });
#pragma warning restore 612, 618
        }
    }
}
