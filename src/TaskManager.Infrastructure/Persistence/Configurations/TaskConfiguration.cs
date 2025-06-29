using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Status).IsRequired();
        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(t => t.CreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}