using RecipeSharingApp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Service.Interface
{
    public interface IRecipeService
    {
        List<Recipe> GetAll();
        Recipe? GetById(Guid Id);
        Recipe? GetByName(string name);
        Recipe Update(Recipe recipe);
        Recipe DeleteById(Guid Id);
        Recipe Add(Recipe recipe);
        void AddToCollection(Recipe recipe, Guid collectionId, string userId);
        void AddRating(Recipe recipe, RecipeRating rating);
    }
}
