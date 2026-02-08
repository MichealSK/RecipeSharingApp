using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeSharingApp.Domain.DTOModels;
using RecipeSharingApp.Domain.Models;
using RecipeSharingApp.Service.Interface;
using System.Security.Claims;
using FluentAssertions;
using Xunit;

namespace Tests.Controller
{
    public class CollectionControllerTests
    {
        private readonly Mock<IRecipeService> _mockRecipeService;
        private readonly Mock<IRecipeCollectionService> _mockCollectionService;
        private readonly CollectionController _controller;
        private readonly string _testUserId = "user-123";

        public CollectionControllerTests()
        {
            _mockRecipeService = new Mock<IRecipeService>();
            _mockCollectionService = new Mock<IRecipeCollectionService>();

            _controller = new CollectionController(_mockRecipeService.Object, _mockCollectionService.Object);

            // Mocking the User (ClaimsPrincipal)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void Index_ReturnsViewWithCollections_WhenUserIsLoggedIn()
        {
            // Arrange
            var collections = new List<RecipeCollection> { new RecipeCollection { Name = "Italian" } };
            _mockCollectionService.Setup(s => s.GetUserCollectionsSync(_testUserId)).Returns(collections);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeEquivalentTo(collections);
        }

        [Fact]
        public void Details_ReturnsNotFound_WhenIdIsNull()
        {
            var result = _controller.Details(null);
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void Details_ReturnsView_WhenCollectionExists()
        {
            var collectionId = Guid.NewGuid();
            var collection = new RecipeCollection { Id = collectionId, Name = "Breakfast" };
            _mockCollectionService.Setup(s => s.GetById(collectionId)).Returns(collection);

            var result = _controller.Details(collectionId);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(collection);
        }

        [Fact]
        public void Create_Get_ReturnsViewWithDTO()
        {
            var result = _controller.Create();
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().BeOfType<RecipeCollectionDTO>();
        }

        [Fact]
        public void Create_Post_RedirectsToIndex_WhenValid()
        {
            // Arrange
            var dto = new RecipeCollectionDTO { Name = "New Collection" };

            // Act
            var result = _controller.Create(dto);

            // Assert
            _mockCollectionService.Verify(s => s.Add(It.Is<RecipeCollection>(c => c.Name == dto.Name && c.UserId == _testUserId)), Times.Once);
            var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
            redirect.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task GetUserCollectionsJson_ReturnsJsonList()
        {
            var collections = new List<RecipeCollection> {
                new RecipeCollection { Id = Guid.NewGuid(), Name = "Vegan", Recipes = new List<RecipeInCollection>() }
            };
            _mockCollectionService.Setup(s => s.GetUserCollections(_testUserId)).ReturnsAsync(collections);

            var result = await _controller.GetUserCollectionsJson();

            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task PinRecipe_ReturnsSuccess_WhenValid()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var colId = Guid.NewGuid();
            var recipe = new Recipe { Id = recipeId };
            var collection = new RecipeCollection { Id = colId, UserId = _testUserId };

            _mockRecipeService.Setup(s => s.GetById(recipeId)).Returns(recipe);
            _mockCollectionService.Setup(s => s.GetById(colId)).Returns(collection);

            // Act
            var result = await _controller.PinRecipe(recipeId, colId);

            // Assert
            _mockCollectionService.Verify(s => s.AddRecipeToCollection(colId, recipe), Times.Once);
            result.Should().BeOfType<JsonResult>();
        }

        [Fact]
        public async Task PinRecipe_ReturnsError_WhenUserDoesNotOwnCollection()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var colId = Guid.NewGuid();
            var collection = new RecipeCollection { Id = colId, UserId = "different-user" };

            _mockRecipeService.Setup(s => s.GetById(recipeId)).Returns(new Recipe());
            _mockCollectionService.Setup(s => s.GetById(colId)).Returns(collection);

            // Act
            var result = await _controller.PinRecipe(recipeId, colId);

            // Assert
            result.Should().BeOfType<JsonResult>();
            // You could further check the JSON content for success = false
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            var result = _controller.DeleteConfirmed(id);

            _mockCollectionService.Verify(s => s.DeleteById(id), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>();
        }

        [Fact]
        public void UnpinConfirmed_RedirectsToIndex()
        {
            var id = Guid.NewGuid();
            var result = _controller.UnpinConfirmed(id);

            _mockCollectionService.Verify(s => s.RemoveRecipeFromCollection(id), Times.Once);
            result.Should().BeOfType<RedirectToActionResult>();
        }
    }
}