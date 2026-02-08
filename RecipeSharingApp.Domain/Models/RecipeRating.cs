using RecipeSharingApp.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.Models
{
    public class RecipeRating : BaseEntity
    {
        public RecipeRating() { }
        public RecipeRating(Recipe recipe, double rating, string user)
        {
            Recipe = recipe;
            RecipeId = recipe.Id;
            UserId = user;
            Rating = rating;
        }

        public Guid RecipeId { get; set; }
        public Recipe? Recipe { get; set; }
        public string? UserId { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserName { get; set; }
    }
}
