using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Recitopia.Models;
using System;

namespace Recitopia.Data
{
    public class RecitopiaDBContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public RecitopiaDBContext()
        { }
        public RecitopiaDBContext(DbContextOptions<RecitopiaDBContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            
            builder.Entity<Components>(entity =>
            {
                entity.HasKey(e => e.Comp_Id)
                    .HasName("PK__Componen__DC0BCC2082D90BE8");

                entity.Property(e => e.Comp_Sort).HasMaxLength(50);

                entity.Property(e => e.Component_Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Notes).HasColumnType("text");
            });



            builder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(e => e.Ingredient_Id);

                entity.Property(e => e.Brand).HasMaxLength(50);

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_cup).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_gram).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_gram2).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_lb).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_lb2).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_ounce2).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_oz).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_tbsp).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_tsp).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Ingred_Comp_name).HasColumnType("text");

                entity.Property(e => e.Ingred_name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Packaging).HasMaxLength(50);

                entity.Property(e => e.Per_item).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Vendor_name).HasMaxLength(50);

                entity.Property(e => e.Weight_Equiv_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Weight_Equiv_measure).HasMaxLength(15);

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Ingredient)
                    .HasForeignKey(d => d.Vendor_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_ToTable");
            });

            builder.Entity<Ingredient_Components>(entity =>
            {
                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Components)
                    .WithMany(p => p.Ingredient_Components)
                    .HasForeignKey(d => d.Comp_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_Comp_ToTable_1");

                entity.HasOne(d => d.Ingredients)
                    .WithMany(p => p.Ingredient_Components)
                    .HasForeignKey(d => d.Ingred_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_Comp_ToTable");
            });

            builder.Entity<Ingredient_Nutrients>(entity =>
            {
                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Nut_per_100_grams).HasColumnType("decimal(18, 3)");

                entity.HasOne(d => d.Ingredients)
                    .WithMany(p => p.Ingredient_Nutrients)
                    .HasForeignKey(d => d.Ingred_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_Nutrients_ToTable");

                entity.HasOne(d => d.Nutrition)
                    .WithMany(p => p.Ingredient_Nutrients)
                    .HasForeignKey(d => d.Nutrition_Item_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Ingredient_Nutrients_ToTable_1");
            });

            builder.Entity<Meal_Category>(entity =>
            {
                entity.HasKey(e => e.Category_Id)
                    .HasName("PK__Meal_Cat__6DB38D6EACDEBF3E");

                entity.Property(e => e.Category_Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasColumnType("text");
            });

            builder.Entity<Nutrition>(entity =>
            {
                entity.HasKey(e => e.Nutrition_Item_Id)
                    .HasName("PK__Nutritio__326DD8CD1CE0BF74");

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Measurement).HasMaxLength(50);

                entity.Property(e => e.Nutrition_Item)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrderOnNutrientPanel).HasDefaultValueSql("((50))");
            });

            builder.Entity<Recipe>(entity =>
            {
                entity.HasKey(e => e.Recipe_Id)
                    .HasName("PK__Recipe__0959CED94CA2B8C7");

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.LaborCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Recipe_Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SKU).HasMaxLength(50);

                entity.Property(e => e.UPC).HasMaxLength(50);

                entity.HasOne(d => d.Meal_Category)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.Category_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Recipe_ToTable");

                entity.HasOne(d => d.Serving_Sizes)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.SS_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Serving_Size_ToTable");
            });

            builder.Entity<Recipe_Ingredients>(entity =>
            {
                entity.Property(e => e.Amount_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.Recipe_Ingredients)
                    .HasForeignKey(d => d.Ingredient_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Recipe_Ingredients_ToTable_1");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Recipe_Ingredients)
                    .HasForeignKey(d => d.Recipe_Id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Recipe_Ingredients_ToTable");
            });

            builder.Entity<Serving_Sizes>(entity =>
            {
                entity.HasKey(e => e.SS_Id)
                    .HasName("PK__Serving___456F9402CBFF87ED");

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes).HasColumnType("text");
            });

            builder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.Vendor_Id)
                    .HasName("PK__Vendor__D9CCC2A879687754");

                entity.Property(e => e.Address1).HasMaxLength(50);

                entity.Property(e => e.Address2).HasMaxLength(50);

                entity.Property(e => e.City).HasMaxLength(25);

                entity.Property(e => e.Customer_Id).HasDefaultValueSql("((1))");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.State).HasMaxLength(10);

                entity.Property(e => e.Vendor_Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Web_URL).HasMaxLength(50);
            });

            builder.Entity<View_All_Ingredient_Components>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_All_Ingredient_Components");

                entity.Property(e => e.Component_Name).HasMaxLength(50);

                entity.Property(e => e.Ingred_name).HasMaxLength(50);
            });

            builder.Entity<View_All_Ingredient_Nutrients>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_All_Ingredient_Nutrients");

                entity.Property(e => e.Ingred_name).HasMaxLength(50);

                entity.Property(e => e.Nut_per_100_grams).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Nutrition_Item).HasMaxLength(50);
            });

            builder.Entity<View_All_Recipe_Components>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_All_Recipe_Components");

                entity.Property(e => e.Amount_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Component_Name).HasMaxLength(50);

                entity.Property(e => e.Recipe_Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            builder.Entity<View_All_Recipe_Ingredients>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_All_Recipe_Ingredients");

                entity.Property(e => e.Amount_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_lb).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Ingred_Comp_name).HasColumnType("text");

                entity.Property(e => e.Ingred_name).HasMaxLength(50);

                entity.Property(e => e.Recipe_Name).HasMaxLength(50);
            });

            builder.Entity<View_Angular_Ingredient_Components_Details>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Angular_Ingredient_Components_Details");

                entity.Property(e => e.Component_Name).HasMaxLength(50);

                entity.Property(e => e.Ingred_name).HasMaxLength(50);
            });

            builder.Entity<View_Angular_Ingredient_Nutrients_Details>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Angular_Ingredient_Nutrients_Details");

                entity.Property(e => e.Ingred_name).HasMaxLength(50);

                entity.Property(e => e.Nut_per_100_grams).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Nutrition_Item).HasMaxLength(50);
            });

            builder.Entity<View_Angular_Ingredients_Details>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Angular_Ingredients_Details");

                entity.Property(e => e.Cost).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Cost_per_lb).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Ingred_name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Vendor_Name).HasMaxLength(50);
            });

            builder.Entity<View_Angular_Recipe_Details>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Angular_Recipe_Details");

                entity.Property(e => e.Category_Name).HasMaxLength(50);

                entity.Property(e => e.LaborCost).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Notes).HasColumnType("text");

                entity.Property(e => e.Recipe_Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SKU).HasMaxLength(50);

                entity.Property(e => e.UPC).HasMaxLength(50);
            });

            builder.Entity<View_Angular_Recipe_Ingredients_Details>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Angular_Recipe_Ingredients_Details");

                entity.Property(e => e.Amount_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Ingred_name).HasMaxLength(50);

                entity.Property(e => e.Recipe_Name).HasMaxLength(50);
            });

            builder.Entity<View_Nutrition_Panel>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("View_Nutrition_Panel");

                entity.Property(e => e.Amount_g).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Ingred_name).HasMaxLength(50);

                entity.Property(e => e.Measurement).HasMaxLength(50);

                entity.Property(e => e.Nut_per_100_grams).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Nutrition_Item).HasMaxLength(50);

                entity.Property(e => e.Recipe_Name).HasMaxLength(50);
            });

            

           
            
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public virtual DbSet<Components> Components { get; set; }

        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Ingredient_Components> Ingredient_Components { get; set; }
        public virtual DbSet<Ingredient_Nutrients> Ingredient_Nutrients { get; set; }
        public virtual DbSet<Meal_Category> Meal_Category { get; set; }
        public virtual DbSet<Nutrition> Nutrition { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
       
        public virtual DbSet<Recipe_Ingredients> Recipe_Ingredients { get; set; }
        public virtual DbSet<Serving_Sizes> Serving_Sizes { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }

        public virtual DbSet<AppRole> AppRoles { get; set; }

        public virtual DbSet<Customers> Customers { get; set; }

        public virtual DbSet<Customer_Users> Customer_Users { get; set; }

        //public virtual DbSet<AppUser> AppUser { get; set; }

        //public virtual DbSet<View_All_Recipe_Components> View_All_Recipe_Components { get; set; }
        //public virtual DbSet<View_All_Recipe_Ingredients> View_All_Recipe_Ingredients { get; set; }
        //public virtual DbSet<View_Angular_Ingredient_Components_Details> View_Angular_Ingredient_Components_Details { get; set; }
        //public virtual DbSet<View_Angular_Ingredient_Nutrients_Details> View_Angular_Ingredient_Nutrients_Details { get; set; }
        //public virtual DbSet<View_Angular_Ingredients_Details> View_Angular_Ingredients_Details { get; set; }
        //public virtual DbSet<View_Angular_Recipe_Details> View_Angular_Recipe_Details { get; set; }
        //public virtual DbSet<View_Angular_Recipe_Ingredients_Details> View_Angular_Recipe_Ingredients_Details { get; set; }
        //public virtual DbSet<View_Nutrition_Panel> View_Nutrition_Panel { get; set; }


    }
}
