using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Service.Impl
{
    public class RecipeRatingService : IRecipeRatingService
    {
        private readonly IRepository<Recipe> _recipeRepository;
        private readonly IRepository<RecipeRating> _recipeRatingRepository;

        public RecipeRatingService(IRepository<Recipe> recipeRepository, IRepository<RecipeRating> recipeRatingRepository)
        {
            _recipeRepository = recipeRepository;
            _recipeRatingRepository = recipeRatingRepository;
        }

        public RecipeRating Add(RecipeRating rating)
        {
            rating.Id = Guid.NewGuid();
            return _recipeRatingRepository.Insert(rating);
        }

        public RecipeRating DeleteById(Guid Id)
        {
            var recipe = _recipeRatingRepository.Get(selector: x => x, predicate: x => x.Id == Id);
            return _recipeRatingRepository.Delete(recipe);
        }

        public List<RecipeRating> GetAll()
        {
            return _recipeRatingRepository.GetAll(selector: x => x).ToList();
        }

        public List<RecipeRating> GetAllFor(Recipe recipe)
        {
            return _recipeRatingRepository.GetAll(selector: x => x, predicate: x => x.RecipeId == recipe.Id).ToList();
        }

        public double GetAverageRanking(Recipe recipe)
        {
            double rankingTotal = 0;
            List<RecipeRating> rankings = GetAllFor(recipe);
            foreach (var ranking in rankings)
            {
                rankingTotal += ranking.Rating;
            }
            return Math.Round(rankingTotal/rankings.Count, 1);
        }

        public RecipeRating? GetById(Guid Id)
        {
            return _recipeRatingRepository.Get(selector: x => x, predicate: x => x.Id == Id);
        }

        public RecipeRating? GetByRecipe(string userId, Guid recipeId)
        {
            return _recipeRatingRepository.Get(selector: x => x, predicate: x => x.RecipeId == recipeId && x.UserId == userId);
        }

        public RecipeRating Update(RecipeRating rating)
        {
            return _recipeRatingRepository.Update(rating);
        }
    }
}
