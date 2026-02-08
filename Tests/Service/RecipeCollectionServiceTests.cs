using Moq;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Implementation;
using Xunit;
using FluentAssertions;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeSharingApp.Tests.Unit
{
    public class RecipeCollectionServiceTests
    {
        private readonly Mock<IRepository<RecipeCollection>> _mockCollectionRepo;
        private readonly Mock<IRepository<RecipeInCollection>> _mockRICRepo;
        private readonly RecipeCollectionService _service;

        public RecipeCollectionServiceTests()
        {
            _mockCollectionRepo = new Mock<IRepository<RecipeCollection>>();
            _mockRICRepo = new Mock<IRepository<RecipeInCollection>>();

            _service = new RecipeCollectionService(
                _mockCollectionRepo.Object,
                _mockRICRepo.Object);
        }

        [Fact]
        public void BasicCRUD_ShouldCoordinateWithRepository()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var collection = new RecipeCollection { Id = collectionId, Name = "Breakfast" };
            var list = new List<RecipeCollection> { collection };

            _mockCollectionRepo.Setup(r => r.Get<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                It.IsAny<Expression<Func<RecipeCollection, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()))
                .Returns(collection);

            _mockCollectionRepo.Setup(r => r.GetAll<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                null,
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()
            )).Returns(list);

            // Act
            _service.Add(collection);
            var result = _service.GetById(collectionId);
            var all = _service.GetAll();

            // Assert
            result.Should().NotBeNull();
            all.Should().HaveCount(1);
            _mockCollectionRepo.Verify(r => r.Insert(collection), Times.Once);
        }

        [Fact]
        public async Task AddRecipeToCollection_ShouldLinkRecipeToCollection()
        {
            // Arrange
            var collectionId = Guid.NewGuid();
            var recipe = new Recipe { Id = Guid.NewGuid() };
            var collection = new RecipeCollection { Id = collectionId, Recipes = new List<RecipeInCollection>() };

            _mockCollectionRepo.Setup(r => r.Get<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                It.IsAny<Expression<Func<RecipeCollection, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()))
                .Returns(collection);

            // Act
            await _service.AddRecipeToCollection(collectionId, recipe);

            // Assert
            collection.Recipes.Should().HaveCount(1);
            _mockRICRepo.Verify(r => r.Insert(It.IsAny<RecipeInCollection>()), Times.Once);
        }

        [Fact]
        public async Task RemoveRecipeFromCollection_ShouldInvokeDeleteAndUpdate()
        {
            // Arrange
            var ricId = Guid.NewGuid();
            var collectionId = Guid.NewGuid();
            var ric = new RecipeInCollection { Id = ricId, CollectionId = collectionId };
            var collection = new RecipeCollection { Id = collectionId, Recipes = new List<RecipeInCollection> { ric } };

            _mockRICRepo.Setup(r => r.Get<RecipeInCollection>(
                It.IsAny<Expression<Func<RecipeInCollection, RecipeInCollection>>>(),
                It.IsAny<Expression<Func<RecipeInCollection, bool>>>(),
                null, null))
                .Returns(ric);

            _mockCollectionRepo.Setup(r => r.Get<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                It.IsAny<Expression<Func<RecipeCollection, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()))
                .Returns(collection);

            // Act
            await _service.RemoveRecipeFromCollection(ricId);

            // Assert
            collection.Recipes.Should().BeEmpty();
            _mockRICRepo.Verify(r => r.Delete(ric), Times.Once);
        }

        [Fact]
        public void Update_ShouldInvokeRepository()
        {
            var collection = new RecipeCollection { Id = Guid.NewGuid() };
            _service.Update(collection);
            _mockCollectionRepo.Verify(r => r.Update(collection), Times.Once);
        }

        [Fact]
        public void GetUserCollectionsSync_ShouldFilterByUserId()
        {
            string userId = "user-123";
            var collections = new List<RecipeCollection> { new RecipeCollection { UserId = userId } };

            _mockCollectionRepo.Setup(r => r.GetAll<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                It.IsAny<Expression<Func<RecipeCollection, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()))
                .Returns(collections);

            var result = _service.GetUserCollectionsSync(userId);

            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddRecipeToCollection_InvalidCollectionId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var invalidId = Guid.NewGuid();
            var recipe = new Recipe { Id = Guid.NewGuid() };

            _mockCollectionRepo.Setup(r => r.Get<RecipeCollection>(
                It.IsAny<Expression<Func<RecipeCollection, RecipeCollection>>>(),
                It.IsAny<Expression<Func<RecipeCollection, bool>>>(),
                null,
                It.IsAny<Func<IQueryable<RecipeCollection>, IIncludableQueryable<RecipeCollection, object>>>()))
                .Returns((RecipeCollection)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddRecipeToCollection(invalidId, recipe));

            Assert.Equal($"Recipe collection with ID {invalidId} not found.", exception.Message);
        }

        [Fact]
        public void Update_NonExistingCollection_ShouldNotFail()
        {
            // Arrange
            var collection = new RecipeCollection { Id = Guid.NewGuid(), Name = "Empty Collection" };
            _mockCollectionRepo.Setup(r => r.Update(It.IsAny<RecipeCollection>()));

            // Act
            var exception = Record.Exception(() => _service.Update(collection));

            // Assert
            Assert.Null(exception);
            _mockCollectionRepo.Verify(r => r.Update(collection), Times.Once);
        }
    }
}