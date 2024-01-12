﻿// <auto-generated />
using System;
using CommandManagment.backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CommandManagment.backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CommandManagment.backend.Models.InviteTeamToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Invites");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.Project", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreateUserId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("TeamId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProjectId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ScrumBoards");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoardColumn", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<int>("ScrumBoardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScrumBoardId");

                    b.ToTable("ScrumBoardColumns");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoardTask", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<Guid?>("ScrumBoardColumnId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ScrumBoardId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ScrumBoardColumnId");

                    b.HasIndex("ScrumBoardId");

                    b.ToTable("ScrumBoardTasks");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CreateUserId")
                        .HasColumnType("int");

                    b.Property<string>("TeamDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeamName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Photo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Specialization")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.Property<int>("TeamsId")
                        .HasColumnType("int");

                    b.Property<int>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("TeamsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("TeamUser");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.Project", b =>
                {
                    b.HasOne("CommandManagment.backend.Models.User", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommandManagment.backend.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreateUser");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoard", b =>
                {
                    b.HasOne("CommandManagment.backend.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommandManagment.backend.Models.User", "User")
                        .WithMany("ScrumBoards")
                        .HasForeignKey("UserId");

                    b.Navigation("Project");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoardColumn", b =>
                {
                    b.HasOne("CommandManagment.backend.Models.ScrumBoard", "ScrumBoard")
                        .WithMany("ScrumBoardColumns")
                        .HasForeignKey("ScrumBoardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScrumBoard");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoardTask", b =>
                {
                    b.HasOne("CommandManagment.backend.Models.ScrumBoardColumn", "ScrumBoardColumn")
                        .WithMany("ScrumBoardTasks")
                        .HasForeignKey("ScrumBoardColumnId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CommandManagment.backend.Models.ScrumBoard", "ScrumBoard")
                        .WithMany("ScrumBoardTasks")
                        .HasForeignKey("ScrumBoardId");

                    b.Navigation("ScrumBoard");

                    b.Navigation("ScrumBoardColumn");
                });

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.HasOne("CommandManagment.backend.Models.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CommandManagment.backend.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoard", b =>
                {
                    b.Navigation("ScrumBoardColumns");

                    b.Navigation("ScrumBoardTasks");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.ScrumBoardColumn", b =>
                {
                    b.Navigation("ScrumBoardTasks");
                });

            modelBuilder.Entity("CommandManagment.backend.Models.User", b =>
                {
                    b.Navigation("ScrumBoards");
                });
#pragma warning restore 612, 618
        }
    }
}
