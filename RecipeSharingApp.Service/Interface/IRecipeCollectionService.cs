using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Service.Interface
{
    public interface IRecipeCollectionService
    {

        public void Add(RecipeCollection entity);

        public RecipeCollection DeleteById(Guid Id);

        public List<RecipeCollection> GetAll();

        public RecipeCollection GetById(Guid id);

        public void Update(RecipeCollection entity);
        public List<RecipeCollection> GetUserCollectionsSync(string userId);
        public Task<List<RecipeCollection>> GetUserCollections(string userId);
        public Task AddRecipeToCollection(Guid collectionId, Recipe recipe);
        public Task RemoveRecipeFromCollection(Guid recipeInCollectionId);
    }
}
