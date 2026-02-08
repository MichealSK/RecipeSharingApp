using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RecipeSharingApp.Service.Implementation
{
    public class RecipeCollectionService : IRecipeCollectionService
    {
        private readonly IRepository<RecipeCollection> _collectionRepository;
        private readonly IRepository<RecipeInCollection> _recipeInCollectionRepository;

        public RecipeCollectionService(IRepository<RecipeCollection> collectionRepository, IRepository<RecipeInCollection> recipeInCollectionRepository)
        {
            _collectionRepository = collectionRepository;
            _recipeInCollectionRepository = recipeInCollectionRepository;
        }

        public void Add(RecipeCollection entity)
        {
            _collectionRepository.Insert(entity);
        }

        public RecipeCollection DeleteById(Guid Id)
        {
            var recipe = GetById(Id);
            return _collectionRepository.Delete(recipe);
        }

        public List<RecipeCollection> GetAll()
        {
            return _collectionRepository.GetAll(selector: x => x, include: query => query.Include(c => c.Recipes)).ToList();
        }
        public List<RecipeCollection> GetUserCollectionsSync(string userId)
        {
            return _collectionRepository.GetAll(selector: x => x, predicate: x => x.UserId == userId, include: query => query.Include(c => c.Recipes)).ToList();
        }
        public async Task<List<RecipeCollection>> GetUserCollections(string userId)
        {
            var collections = _collectionRepository.GetAll(
                                        selector: x => x,
                                        predicate: x => x.UserId == userId,
                                        include: query => query
                                            .Include(c => c.Recipes)
                                            .ThenInclude(ric => ric.Recipe)
                                       ).ToList();

            return collections;
        }

        public RecipeCollection GetById(Guid id)
        {
            return _collectionRepository.Get(selector: x => x, predicate: x => x.Id == id, include: query => query.Include(c => c.Recipes));
        }

        public void Update(RecipeCollection entity)
        {
            _collectionRepository.Update(entity);
        }

        public Task AddRecipeToCollection(Guid collectionId, Recipe recipe)
        {
            var collectionToUpdate = GetById(collectionId);

            if (collectionToUpdate == null)
            {
                throw new KeyNotFoundException($"Recipe collection with ID {collectionId} not found.");
            }

            RecipeInCollection rc = new RecipeInCollection(collectionId, collectionToUpdate, recipe.Id, recipe);
            _recipeInCollectionRepository.Insert(rc);
            collectionToUpdate.Recipes.Add(rc);
            Update(collectionToUpdate);

            return Task.CompletedTask;
        }

        public Task RemoveRecipeFromCollection(Guid recipeInCollectionId)
        {
            RecipeInCollection rc = _recipeInCollectionRepository.Get(selector: x => x, predicate: x => x.Id == recipeInCollectionId);
            var collectionToUpdate = GetById(rc.CollectionId);

            _recipeInCollectionRepository.Delete(rc);
            collectionToUpdate.Recipes.Remove(rc);
            Update(collectionToUpdate);

            return Task.CompletedTask;
        }
    }
}
