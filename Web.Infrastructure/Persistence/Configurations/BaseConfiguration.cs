//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Web.Domain.Entites;

//namespace Web.Infrastructure.Persistence.Configurations
//{
//    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
//      where TEntity : BaseModel
//    {
//        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
//        {
//            builder.HasKey(x => x.Id);

//            builder.Property(x => x.Createdon)
//                .HasColumnType("datetime2")
//                .HasDefaultValueSql("GETUTCDATE()")
//                .IsRequired();

//            builder.Property(x => x.Deleted)
//                .HasDefaultValue(false)
//                .IsRequired();

//            builder.Property(x => x.UpdatedByid)
//                .HasMaxLength(450);

//            builder.HasIndex(x => x.UpdatedByid);

//            builder.Property(x => x.Updatedon)
//                .HasColumnType("datetime2");

//            // Global Query Filter for Soft Delete
//            builder.HasQueryFilter(x => !x.Deleted);
        
//    }
//    }
//}
