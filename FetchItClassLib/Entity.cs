using FetchItClassLib.Comment;
using FetchItClassLib.Feedback;
using FetchItClassLib.Issue;
using FetchItClassLib.Language;
using FetchItClassLib.Payment;
using FetchItClassLib.Profile;
using FetchItClassLib.Task;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace FetchItClassLib
{
    // TODO: Fix naming of properties in models
    public partial class Entity : DbContext
    {
        public Entity()
            : base("name=Entity")
        {
        }

        public virtual DbSet<CommentModel> Comments { get; set; }
        public virtual DbSet<FeedbackModel> Feedbacks { get; set; }
        public virtual DbSet<IssueModel> Issues { get; set; }
        public virtual DbSet<LanguageModel> Languages { get; set; }
        public virtual DbSet<PaymentModel> Payments { get; set; }
        public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<ProfileLevel> ProfileLevels { get; set; }
        public virtual DbSet<ProfileModel> Profiles { get; set; }
        public virtual DbSet<ProfileStatus> ProfileStatuses { get; set; }
        public virtual DbSet<TaskModel> Tasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentModel>()
                .Property(e => e.CommentText)
                .IsFixedLength();

            modelBuilder.Entity<IssueModel>()
                .Property(e => e.IssueTitle)
                .IsFixedLength();

            modelBuilder.Entity<PaymentModel>()
                .Property(e => e.PaymentAmount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<PaymentStatus>()
                .Property(e => e.PaymentStatus1)
                .IsFixedLength();

            modelBuilder.Entity<PaymentStatus>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.PaymentStatus1)
                .HasForeignKey(e => e.PaymentStatus)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<PaymentType>()
                .Property(e => e.PaymentType1)
                .IsFixedLength();

            modelBuilder.Entity<PaymentType>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.PaymentType1)
                .HasForeignKey(e => e.PaymentType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileLevel>()
                .Property(e => e.ProfileLevel1)
                .IsFixedLength();

            modelBuilder.Entity<ProfileLevel>()
                .HasMany(e => e.Profiles)
                .WithOptional(e => e.ProfileLevel1)
                .HasForeignKey(e => e.ProfileLevel);

            modelBuilder.Entity<ProfileModel>()
                .Property(e => e.ProfileName)
                .IsFixedLength();

            modelBuilder.Entity<ProfileModel>()
                .Property(e => e.ProfilePhone)
                .IsFixedLength();

            modelBuilder.Entity<ProfileModel>()
                .Property(e => e.ProfileMobile)
                .IsFixedLength();

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.Profile)
                .HasForeignKey(e => e.CommentCreator)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Feedbacks)
                .WithRequired(e => e.Profile)
                .HasForeignKey(e => e.FeedbackCreator)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Feedbacks1)
                .WithRequired(e => e.Profile1)
                .HasForeignKey(e => e.FeedbackTarget)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Issues)
                .WithRequired(e => e.Profile)
                .HasForeignKey(e => e.IssueCreator)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Issues1)
                .WithRequired(e => e.Profile1)
                .HasForeignKey(e => e.IssueTarget)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.Profile)
                .HasForeignKey(e => e.PaymentFromProfile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Payments1)
                .WithRequired(e => e.Profile1)
                .HasForeignKey(e => e.PaymentToProfile)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Tasks)
                .WithRequired(e => e.Profile)
                .HasForeignKey(e => e.TaskMaster)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileModel>()
                .HasMany(e => e.Tasks1)
                .WithRequired(e => e.Profile1)
                .HasForeignKey(e => e.TaskMaster)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProfileStatus>()
                .Property(e => e.Status)
                .IsFixedLength();

            modelBuilder.Entity<ProfileStatus>()
                .HasMany(e => e.Profiles)
                .WithOptional(e => e.ProfileStatus1)
                .HasForeignKey(e => e.ProfileStatus);

            modelBuilder.Entity<TaskModel>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.Task)
                .HasForeignKey(e => e.CommentTask)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TaskModel>()
                .HasMany(e => e.Payments)
                .WithRequired(e => e.Task)
                .HasForeignKey(e => e.PaymentForTask)
                .WillCascadeOnDelete(false);
        }
    }
}
