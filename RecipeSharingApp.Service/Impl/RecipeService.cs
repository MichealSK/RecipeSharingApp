using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Impl;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Service.Impl
{
    public class RecipeService : IRecipeService
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IRepository<RecipeRating> _recipeRatingRepository;
        private readonly IRecipeRatingService _recipeRatingService;

        public RecipeService(IRepository<Recipe> recipeRepository,
            IRepository<RecipeRating> recipeRatingRepository,
            IRecipeRatingService recipeRatingService)
        {
            _recipeRepository = recipeRepository;
            _recipeRatingRepository = recipeRatingRepository;
            _recipeRatingService = recipeRatingService;
        }

        public Recipe Add(Recipe recipe)
        {
            recipe.Id = Guid.NewGuid();
            return _recipeRepository.Insert(recipe);
        }

        public void AddRating(Recipe recipe, RecipeRating rating)
        {
            _recipeRatingService.Add(rating);
            Recipe newRecipe = recipe;
            recipe.Rating = _recipeRatingService.GetAverageRanking(recipe);
            Update(newRecipe);
        }

        public void AddToCollection(Recipe recipe, Guid collectionId, string userId)
        {
            throw new NotImplementedException();
        }

        public Recipe DeleteById(Guid Id)
        {
            var recipe = _recipeRepository.Get(selector: x => x, predicate: x => x.Id == Id);
            return _recipeRepository.Delete(recipe);
        }

        public List<Recipe> GetAll()
        {
            return _recipeRepository.GetAll(selector: x => x).ToList();
        }

        public Recipe? GetById(Guid Id)
        {
            return _recipeRepository.Get(selector: x => x, predicate: x => x.Id == Id);
        }

        public Recipe? GetByName(string name)
        {
            return _recipeRepository.Get(selector: x => x, predicate: x => x.Name == name);
        }

        public Recipe Update(Recipe recipe)
        {
            return _recipeRepository.Update(recipe);
        }
    }
}
