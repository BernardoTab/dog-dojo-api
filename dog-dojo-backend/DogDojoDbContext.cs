using dog_dojo_backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace dog_dojo_backend
{
    public class DogDojoDbContext : DbContext
    {
        public DogDojoDbContext(DbContextOptions<DogDojoDbContext> options) : base(options)
        {
        }

        // Define your DbSet here (matching your table names)
        public DbSet<CompletedQuest> CompletedQuests { get; set; }
        public DbSet<CurrentQuest> CurrentQuests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Important: Supabase uses 'public' schema by default
            modelBuilder.HasDefaultSchema("public");


            modelBuilder.Entity<CompletedQuest>().ToTable("completed_quest");
            modelBuilder.Entity<CurrentQuest>().ToTable("current_quest");

            modelBuilder.Entity<CompletedQuest>(entity =>
            {
                entity.ToTable("completed_quest");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CompletedDate).HasColumnName("completed_date");
                entity.Property(e => e.QuestId).HasColumnName("quest_id");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
            });

            modelBuilder.Entity<CurrentQuest>(entity =>
            {
                entity.ToTable("current_quest");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.QuestId).HasColumnName("quest_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.StartDate).HasColumnName("start_date");
            });
            // Optional: If you used UUIDs for IDs, configure them here
            // modelBuilder.Entity<User>().Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
        }
    }
}
