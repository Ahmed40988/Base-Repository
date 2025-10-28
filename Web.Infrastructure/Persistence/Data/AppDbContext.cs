using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Web.Domain.Entites;

namespace Web.Infrastructure.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important for Identity

            #region explanation
            //   انا هنا فصلت كل الكونفيجريشن بتاع كل مودل ف كلاس لوحده للتنظيم وعملت هنا كول لكل الكونفيجريشن
            // IEntityTypeConfiguration دي عن طريق انه هيطبق كل الكلاسسز اللي بتورث من  
            #endregion
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            #region explanation2
            // Restrict بص ي هندسه (مصطفي و محمد )انا ضيفت الكود دا هنا علشان اغير سلوك المسح  كله يكون  
            //  علشان مدخلش ف مشكله زي اني امسح كاتيجوري مثلا تمسح كل المنتجات اللي تحتها  
            // Cascade على أكتر من مستوى ====>(CategoryId → Product → OrderItems)، وده يمسح آلاف الصفوف من غير ما تحس.
            //soft delete بيحظرك ويجبرك انك تمسح العلاقات اللي بين الجداول دي وبعض او انك تستخدم   Restrict لكن بقا سلوك 
            #endregion
            var cascadeFKs = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;


        }
    }
}
