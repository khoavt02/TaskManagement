﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Models;

public partial class TaskManagementContext : DbContext
{
    public TaskManagementContext()
    {
    }

    public TaskManagementContext(DbContextOptions<TaskManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleGroup> RoleGroups { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TaskEvaluate> TaskEvaluates { get; set; }

    public virtual DbSet<TaskProgress> TaskProgresses { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Positions> Positons { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<HubConnection> HubConnections { get; set; }
    public virtual DbSet<Module> Modules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CommentParent).HasColumnName("comment_parent");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.FileAttach)
                .HasMaxLength(500)
                .HasColumnName("file_attach");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
        });
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notification");
            entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.IsRead).HasColumnName("is_read");
			entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("user_name");
            entity.Property(e => e.NotificationDateTime)
                .HasColumnType("datetime")
                .HasColumnName("notification_date_time");
            entity.Property(e => e.Message)
                .HasMaxLength(1000)
                .HasColumnName("message");
			entity.Property(e => e.Link)
				.HasMaxLength(50)
				.HasColumnName("link");
		});

        modelBuilder.Entity<HubConnection>(entity =>
        {
            entity.ToTable("hub_connection");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConnectionId)
                .HasMaxLength(255)
                .HasColumnName("connection_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("user_name");
           
        });
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("department");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(50)
                .HasColumnName("department_code");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(255)
                .HasColumnName("department_name");
            entity.Property(e => e.Mannager)
                .HasMaxLength(50)
                .HasColumnName("mannager");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });
        modelBuilder.Entity<Positions>(entity =>
        {
            entity.ToTable("positions");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Name)
				.HasMaxLength(250)
				.HasColumnName("name");
            entity.Property(e => e.CreatedBy)
               .HasMaxLength(50)
               .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

        });
			modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("project");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CompleteTime)
                .HasColumnType("datetime")
                .HasColumnName("complete_time");
            entity.Property(e => e.Department)
                .HasMaxLength(500)
                .HasColumnName("department");
            entity.Property(e => e.Users)
               .HasMaxLength(1000)
               .HasColumnName("users");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.LinkFiles)
                .HasMaxLength(1000)
                .HasColumnName("link_files");
            entity.Property(e => e.Manager)
                .HasMaxLength(50)
                .HasColumnName("manager");
            entity.Property(e => e.MembersQuantity).HasColumnName("members_quantity");
            entity.Property(e => e.Point)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("point");
            entity.Property(e => e.PriorityLevel)
                .HasMaxLength(50)
                .HasColumnName("priority_level");
            entity.Property(e => e.Process)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("process");
            entity.Property(e => e.ProjectCode)
                .HasMaxLength(50)
                .HasColumnName("project_code");
            entity.Property(e => e.ProjectName)
                .HasMaxLength(250)
                .HasColumnName("project_name");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ModuleId).HasColumnName("module_id");
            entity.Property(e => e.ModuleName)
               .HasMaxLength(250)
               .HasColumnName("module_name");
            entity.Property(e => e.Add).HasColumnName("add");
            entity.Property(e => e.Edit).HasColumnName("edit");
            entity.Property(e => e.Export).HasColumnName("export");
            entity.Property(e => e.RoleGroupId).HasColumnName("role_group_id");
            entity.Property(e => e.View).HasColumnName("view");
            entity.Property(e => e.Delete).HasColumnName("delete");
            entity.Property(e => e.Review).HasColumnName("review");
            entity.Property(e => e.Comment).HasColumnName("comment");
        });

        modelBuilder.Entity<RoleGroup>(entity =>
        {
            entity.ToTable("role_group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .HasColumnName("updated_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.ToTable("task");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.CompleteTime)
                .HasColumnType("datetime")
                .HasColumnName("complete_time");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.EstimateTime)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("estimate_time");
            entity.Property(e => e.Level)
                .HasMaxLength(50)
                .HasColumnName("level");
            entity.Property(e => e.Points)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("points");
            entity.Property(e => e.ProcessPercent)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("process_percent");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.AssignedUser)
                .HasMaxLength(50)
                .HasColumnName("assigned_user");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TaskCode)
                .HasMaxLength(50)
                .HasColumnName("task_code");
            entity.Property(e => e.TaskParent).HasColumnName("task_parent");
            entity.Property(e => e.TaskName)
                .HasMaxLength(500)
                .HasColumnName("task_name");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(50)
                .HasColumnName("update_by");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
			entity.Property(e => e.LinkFiles)
				.HasMaxLength(500)
				.HasColumnName("link_files");
		});

        modelBuilder.Entity<TaskEvaluate>(entity =>
        {
            entity.ToTable("task_evaluate");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Points)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("points");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
        });

        modelBuilder.Entity<TaskProgress>(entity =>
        {
            entity.ToTable("task_progress");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FileAttach)
                .HasMaxLength(500)
                .HasColumnName("file_attach");
            entity.Property(e => e.ProcessPercent)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("process_percent");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Account)
                .HasMaxLength(50)
                .HasColumnName("account");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.DepartmentCode)
                .HasMaxLength(50)
                .HasColumnName("department_code");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(255)
                .HasColumnName("department_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PositionCode)
                .HasMaxLength(50)
                .HasColumnName("position_code");
            entity.Property(e => e.PositionName)
                .HasMaxLength(255)
                .HasColumnName("position_name");
            entity.Property(e => e.Role)
                .HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateBy)
                .HasMaxLength(50)
                .HasColumnName("update_by");
            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");
            entity.Property(e => e.UserCode)
                .HasMaxLength(50)
                .HasColumnName("user_code");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("user_name");
        });

		modelBuilder.Entity<Module>(entity =>
		{
			entity.ToTable("modules");

			entity.Property(e => e.Id).HasColumnName("id");
			entity.Property(e => e.ModuleCode)
				.HasMaxLength(100)
				.HasColumnName("module_code");
			entity.Property(e => e.ModuleName)
				.HasMaxLength(255)
				.HasColumnName("module_name");
			entity.Property(e => e.DisplayName)
				.HasMaxLength(1000)
				.HasColumnName("display_name");
		});

		OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
