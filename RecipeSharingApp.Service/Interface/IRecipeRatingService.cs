using RecipeSharingApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Service.Interface
{
    public interface IRecipeRatingService
    {
        List<RecipeRating> GetAll();
        List<RecipeRating> GetAllFor(Recipe recipe);
        double GetAverageRanking(Recipe recipe);
        RecipeRating? GetById(Guid Id);
        RecipeRating Update(RecipeRating product);
        RecipeRating DeleteById(Guid Id);
        RecipeRating Add(RecipeRating rating);
        public RecipeRating? GetByRecipe(string userId, Guid recipeId);
    }
}
