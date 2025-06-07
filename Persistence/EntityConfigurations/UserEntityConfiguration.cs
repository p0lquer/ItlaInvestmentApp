using InvestmentApp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentApp.Infrastructure.Persistence.EntityConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            #region Basic configuration
            builder.HasKey(x => x.Id);
            builder.ToTable("Users");
            #endregion

            #region Property configurations
            builder.Property(u => u.Name).IsRequired().HasMaxLength(200);
            builder.Property(u => u.Password).IsRequired().HasMaxLength(int.MaxValue);//nvarchar(max)
            #endregion

            #region relationships

            builder.HasMany(u => u.InvestmentPortfolios)
                .WithOne(ip => ip.User)
                .HasForeignKey(ip => ip.UserId)
                .OnDelete(DeleteBehavior.Cascade);//lambda
    
            #endregion
        }
    }
}
