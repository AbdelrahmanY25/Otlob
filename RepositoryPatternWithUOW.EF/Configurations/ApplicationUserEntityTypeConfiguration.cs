using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otlob.Core.Models;
using EFCore = Microsoft.EntityFrameworkCore.EF;
using Utility;


namespace Otlob.EF.Configurations
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasQueryFilter(u => EFCore.Property<bool>(u, "IsDeleted") == false);           
        }
    }
}
