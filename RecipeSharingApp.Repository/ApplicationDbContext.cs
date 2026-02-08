using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeSharingApp.Domain.Identity;
using RecipeSharingApp.Domain.Models;

namespace RecipeSharingApp.Repository
{
    public class ApplicationDbContext : IdentityDbContext<RecipeAppUser>
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeRating> RecipeRatings { get; set; }
        public DbSet<RecipeInCollection> RecipeInCollections { get; set; }
        public DbSet<RecipeCollection> RecipeCollections { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
