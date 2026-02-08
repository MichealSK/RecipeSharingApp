using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace TestsUI
{
    public class CollectionPage
    {
        private readonly IPage _page;
        public CollectionPage(IPage page) => _page = page;

        public ILocator CreateButton => _page.GetByRole(AriaRole.Link).Filter(new() { HasText = "Create New Collection" });
        public ILocator NameInput => _page.Locator("input[name='Name']");
        public ILocator SubmitCreateButton => _page.GetByRole(AriaRole.Button, new() { Name = "Create Collection" });

        public async Task CreateCollection(string name)
        {
            await CreateButton.ClickAsync();
            await NameInput.FillAsync(name);
            await SubmitCreateButton.ClickAsync();
        }
    }

    [TestFixture]
    public class CollectionUITests : BaseUiTest
    {
        [Test]
        public async Task User_Can_Create_And_View_Collection()
        {
            await LoginAsTestUser();
            var collectionPage = new CollectionPage(Page);
            string collectionName = "UI Test Collection " + Guid.NewGuid().ToString()[..5];

            await Page.GotoAsync($"{BaseUrl}/Collection");
            await collectionPage.CreateCollection(collectionName);

            await Expect(Page.GetByText(collectionName).First).ToBeVisibleAsync();
        }

        [Test]
        public async Task User_Can_Pin_Recipe_To_Collection()
        {
            await LoginAsTestUser();

            await Page.GotoAsync($"{BaseUrl}/RecipeView");

            var pinButton = Page.GetByRole(AriaRole.Button, new() { Name = "Pin This Recipe" }).First;
            await pinButton.ClickAsync();

            var collectionsList = Page.Locator("#collections-list");
            await Expect(collectionsList).ToBeVisibleAsync();

            var collectionSelectBtn = Page.Locator(".collection-select-btn").First;

            if (await collectionSelectBtn.CountAsync() == 0)
            {
                Assert.Ignore("No collections found in the modal to pin to.");
                return;
            }

            await collectionSelectBtn.ClickAsync();

            await Expect(Page.Locator("#pinToCollectionModal")).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task Delete_Collection_Removes_Card_From_UI()
        {
            await LoginAsTestUser();
            var collectionPage = new CollectionPage(Page);

            await Page.GotoAsync($"{BaseUrl}/Collection");
            await collectionPage.CreateCollection("Delete Me " + Guid.NewGuid().ToString()[..4]);

            var deleteButtons = Page.Locator("button:has-text('Delete')");

            await Expect(deleteButtons.First).ToBeVisibleAsync();

            int initialCount = await deleteButtons.CountAsync();

            await deleteButtons.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            int newCount = await deleteButtons.CountAsync();

            Assert.That(newCount, Is.LessThan(initialCount));
        }
    }
}