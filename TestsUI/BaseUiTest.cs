using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace TestsUI
{
    public class BaseUiTest : PageTest
    {
        protected string BaseUrl = "https://localhost:7203";

        [SetUp]
        public async Task BaseSetup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = TestContext.CurrentContext.Test.Name,
                Screenshots = true,
                Snapshots = true
            });
        }

        protected async Task LoginAsTestUser()
        {
            await Page.GotoAsync($"{BaseUrl}/Identity/Account/Login");

            await Page.Locator("input[name='Input.Email']").FillAsync("test@example.com");
            await Page.Locator("input[name='Input.Password']").FillAsync("Password123!");

            await Page.RunAndWaitForNavigationAsync(async () =>
            {
                await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
            });

            await Expect(Page.Locator("text=Logout")).ToBeVisibleAsync();
        }

        [TearDown]
        public async Task BaseTearDown()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(TestContext.CurrentContext.WorkDirectory, "traces", $"{TestContext.CurrentContext.Test.Name}.zip")
            });
        }
    }
}
