using Moq;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Repository.Interface;
using RecipeSharingApp.Service.Impl;
using RecipeSharingApp.Service.Interface;
using Xunit;
using FluentAssertions;
using System.Linq.Expressions;

namespace RecipeSharingApp.Tests.Unit
{
    public class RecipeServiceTests
    {
        private readonly Mock<IRepository<Recipe>> _mockRecipeRepo;
        private readonly Mock<IRepository<RecipeRating>> _mockRatingRepo;
        private readonly Mock<IRecipeRatingService> _mockRatingService;
        private readonly RecipeService _service;

        public RecipeServiceTests()
        {
            _mockRecipeRepo = new Mock<IRepository<Recipe>>();
            _mockRatingRepo = new Mock<IRepository<RecipeRating>>();
            _mockRatingService = new Mock<IRecipeRatingService>();

            _service = new RecipeService(
                _mockRecipeRepo.Object,
                _mockRatingRepo.Object,
                _mockRatingService.Object);
        }

        [Fact]
        public void Add_GetById_GetAll_ShouldCoordinateWithRepository()
        {
            // Arrange
            var recipe = new Recipe { Name = "Pasta" };
            var list = new List<Recipe> { recipe };

            _mockRecipeRepo.Setup(r => r.Insert(It.IsAny<Recipe>())).Returns(recipe);
            _mockRecipeRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Recipe, Recipe>>>(), It.IsAny<Expression<Func<Recipe, bool>>>(), null, null))
                           .Returns(recipe);
            _mockRecipeRepo.Setup(r => r.GetAll(It.IsAny<Expression<Func<Recipe, Recipe>>>(), null, null, null))
                           .Returns(list);

            // Act
            var added = _service.Add(recipe);
            var found = _service.GetById(Guid.NewGuid());
            var all = _service.GetAll();

            // Assert
            added.Id.Should().NotBeEmpty();
            found.Should().NotBeNull();
            all.Should().HaveCount(1);
        }

        [Fact]
        public void DeleteById_ShouldFindAndDeleteRecipe()
        {
            // Arrange
            var id = Guid.NewGuid();
            var recipe = new Recipe { Id = id };
            _mockRecipeRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Recipe, Recipe>>>(), It.IsAny<Expression<Func<Recipe, bool>>>(), null, null))
                           .Returns(recipe);
            _mockRecipeRepo.Setup(r => r.Delete(recipe)).Returns(recipe);

            // Act
            var result = _service.DeleteById(id);

            // Assert
            result.Should().Be(recipe);
            _mockRecipeRepo.Verify(r => r.Delete(recipe), Times.Once);
        }

        [Fact]
        public void GetByName_ShouldReturnCorrectRecipe()
        {
            // Arrange
            var name = "Tacos";
            var recipe = new Recipe { Name = name };
            _mockRecipeRepo.Setup(r => r.Get(It.IsAny<Expression<Func<Recipe, Recipe>>>(), It.IsAny<Expression<Func<Recipe, bool>>>(), null, null))
                           .Returns(recipe);

            // Act
            var result = _service.GetByName(name);

            // Assert
            result.Name.Should().Be(name);
        }

        [Fact]
        public void AddRating_ShouldUpdateRecipeWithNewAverage()
        {
            // Arrange
            var recipe = new Recipe { Id = Guid.NewGuid(), Rating = 0 };
            var rating = new RecipeRating { RecipeId = recipe.Id, Rating = 5 };

            _mockRatingService.Setup(s => s.GetAverageRanking(recipe)).Returns(4.5);
            _mockRecipeRepo.Setup(r => r.Update(recipe)).Returns(recipe);

            // Act
            _service.AddRating(recipe, rating);

            // Assert
            recipe.Rating.Should().Be(4.5);
            _mockRatingService.Verify(s => s.Add(rating), Times.Once);
            _mockRecipeRepo.Verify(r => r.Update(recipe), Times.Once);
        }

        [Fact]
        public void Update_ShouldReturnUpdatedRecipe()
        {
            // Arrange
            var recipe = new Recipe { Name = "Old Name" };
            _mockRecipeRepo.Setup(r => r.Update(recipe)).Returns(recipe);

            // Act
            recipe.Name = "New Name";
            var result = _service.Update(recipe);

            // Assert
            result.Name.Should().Be("New Name");
            _mockRecipeRepo.Verify(r => r.Update(recipe), Times.Once);
        }
    }
}