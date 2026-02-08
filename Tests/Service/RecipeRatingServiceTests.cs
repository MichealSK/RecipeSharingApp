using FluentAssertions;
using Moq;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Impl;
using System.Linq.Expressions;
using Xunit;

namespace RecipeSharingApp.Tests.Unit
{
    public class RecipeRatingServiceTests
    {
        private readonly Mock<IRepository<Recipe>> _mockRecipeRepo;
        private readonly Mock<IRepository<RecipeRating>> _mockRatingRepo;
        private readonly RecipeRatingService _service;

        public RecipeRatingServiceTests()
        {
            _mockRecipeRepo = new Mock<IRepository<Recipe>>();
            _mockRatingRepo = new Mock<IRepository<RecipeRating>>();

            _service = new RecipeRatingService(_mockRecipeRepo.Object, _mockRatingRepo.Object);
        }

        [Fact]
        public void GetAverageRanking_ShouldReturnCorrectAverage_WhenRatingsExist()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var recipe = new Recipe { Id = recipeId };
            var ratings = new List<RecipeRating>
            {
                new RecipeRating { RecipeId = recipeId, Rating = 5 },
                new RecipeRating { RecipeId = recipeId, Rating = 4 },
                new RecipeRating { RecipeId = recipeId, Rating = 2 }
            };

            _mockRatingRepo.Setup(repo => repo.GetAll<RecipeRating>(
                It.IsAny<Expression<Func<RecipeRating, RecipeRating>>>(),
                It.IsAny<Expression<Func<RecipeRating, bool>>>(),
                null,
                null
            )).Returns(ratings);

            // Act
            var result = _service.GetAverageRanking(recipe);

            // Assert
            result.Should().Be(3.7);
        }

        [Fact]
        public void Add_ShouldAssignNewId_BeforeInserting()
        {
            // Arrange
            var newRating = new RecipeRating { Rating = 5 };
            _mockRatingRepo.Setup(r => r.Insert(It.IsAny<RecipeRating>()))
                           .Returns((RecipeRating r) => r);

            // Act
            var result = _service.Add(newRating);

            // Assert
            result.Id.Should().NotBe(Guid.Empty);
            _mockRatingRepo.Verify(r => r.Insert(It.IsAny<RecipeRating>()), Times.Once);
        }

        [Fact]
        public void Add_GetById_GetAll_ShouldExecuteSuccessfully()
        {
            // Arrange
            var ratingId = Guid.NewGuid();
            var rating = new RecipeRating { Id = ratingId, Rating = 5 };
            var list = new List<RecipeRating> { rating };

            _mockRatingRepo.Setup(r => r.Insert(It.IsAny<RecipeRating>())).Returns(rating);

            _mockRatingRepo.Setup(r => r.Get<RecipeRating>(
                It.IsAny<Expression<Func<RecipeRating, RecipeRating>>>(),
                It.IsAny<Expression<Func<RecipeRating, bool>>>(),
                null,
                null))
                .Returns(rating);

            _mockRatingRepo.Setup(r => r.GetAll<RecipeRating>(
                It.IsAny<Expression<Func<RecipeRating, RecipeRating>>>(),
                null, null, null))
                .Returns(list);

            // Act
            var added = _service.Add(rating);
            var retrieved = _service.GetById(ratingId);
            var all = _service.GetAll();

            // Assert
            added.Id.Should().NotBeEmpty();
            retrieved.Should().NotBeNull();
            all.Should().HaveCount(1);
        }

        [Fact]
        public void GetAllFor_ShouldReturnRatingsForSpecificRecipe()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var recipe = new Recipe { Id = recipeId };
            var ratings = new List<RecipeRating>
            {
                new RecipeRating { RecipeId = recipeId, Rating = 4 }
            };

            _mockRatingRepo.Setup(repo => repo.GetAll<RecipeRating>(
                It.IsAny<Expression<Func<RecipeRating, RecipeRating>>>(),
                It.IsAny<Expression<Func<RecipeRating, bool>>>(),
                null, null))
                .Returns(ratings);

            // Act
            var result = _service.GetAllFor(recipe);

            // Assert
            result.Should().NotBeEmpty();
            result.First().RecipeId.Should().Be(recipeId);
        }

        [Fact]
        public void Update_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var rating = new RecipeRating { Id = Guid.NewGuid(), Rating = 3 };
            _mockRatingRepo.Setup(r => r.Update(It.IsAny<RecipeRating>())).Returns(rating);

            // Act
            var result = _service.Update(rating);

            // Assert
            result.Should().NotBeNull();
            _mockRatingRepo.Verify(r => r.Update(It.IsAny<RecipeRating>()), Times.Once);
        }
    }
}