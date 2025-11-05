using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vttaScraper;

namespace vttaScraper
{
    public class Scraper
    {
        private readonly bool headless;

        public Scraper(bool headless = true)
        {
            this.headless = headless;
        }

        public async Task<List<VttaRow>> ScrapeAsync(string distance)
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = headless });
            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            await page.GotoAsync("https://www.vtta.org.uk/standards", new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
            await page.ClickAsync("a[href=\"#custom\"]");
            await page.WaitForSelectorAsync("#custom", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            await page.FillAsync("input[name=\"custom\"]", distance);
            await page.SelectOptionAsync("select[name='units']", new[] { "miles" });
            await page.ClickAsync("button.update");
            await page.WaitForSelectorAsync("table tbody tr");

            var headers = (await page.Locator("table thead th").AllInnerTextsAsync()).ToList();
            var rows = await page.Locator("table tbody tr").AllAsync();

            var result = new List<VttaRow>();
            int ageCol = headers.IndexOf("Age");
            int openCol = headers.IndexOf("Open");
            int femaleCol = headers.IndexOf("Female");

            foreach (var row in rows)
            {
                var cells = await row.Locator("td").AllInnerTextsAsync();
                if (int.TryParse(cells[ageCol], out int age) && age >= 50)
                {
                    result.Add(new VttaRow
                    {
                        Age = age,
                        Open = cells[openCol],
                        Female = cells[femaleCol]
                    });
                }
            }

            return result;
        }
    }
}
