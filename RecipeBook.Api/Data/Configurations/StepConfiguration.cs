using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecipeBook.Api.Data.Configurations;

public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> b)
    {
        b.HasKey(s => s.Id);
        b.Property(s => s.Instruction).HasMaxLength(500).IsRequired();

        // 1:N Recipe -> Step. Steps live and die with their recipe.
        b.HasOne(s => s.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(s => s.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(s => new { s.RecipeId, s.Order }).IsUnique();
    }
}
