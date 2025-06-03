using GeneralStore.Mobile.Tests.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

namespace GeneralStore.Mobile.Tests.Pages;

public class CartPage : BasePage
{
    private readonly By _productPrices = By.Id("com.androidsample.generalstore:id/productPrice");
    private readonly By _totalAmount = By.Id("com.androidsample.generalstore:id/totalAmountLbl");
    private readonly By _productTitles = By.Id("com.androidsample.generalstore:id/productName");
    private readonly By _cartContainer = By.Id("com.androidsample.generalstore:id/rvCartProductList");

    public CartPage(AndroidDriver<IWebElement> driver) : base(driver) { }

    public decimal GetTotalAmount()
    {
        var totalText = WaitAndFindElement(_totalAmount).Text;
        return decimal.Parse(totalText.Replace("$", ""));
    }

    public List<decimal> GetProductPrices()
    {
        Console.WriteLine("[CartPage] Getting product prices from cart");
        
        // Wait for cart container to be present and stable
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
        wait.Until(d => {
            try {
                var container = d.FindElement(_cartContainer);
                return container != null && container.Displayed && container.Size.Height > 0;
            }
            catch {
                return false;
            }
        });
        
        Thread.Sleep(2000); // Additional wait for cart stability
        
        // Try to scroll through cart using UIAutomator
        try
        {
            Console.WriteLine("[CartPage] Attempting to scroll through cart");
            var selector = "new UiScrollable(new UiSelector().resourceId(\"com.androidsample.generalstore:id/rvCartProductList\"))" +
                          ".setAsVerticalList().scrollToEnd(1)";
            Driver.FindElementByAndroidUIAutomator(selector);
            Thread.Sleep(1000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CartPage] UIAutomator scroll failed: {ex.Message}");
            // Fallback to manual scroll
            try
            {
                Console.WriteLine("[CartPage] Attempting manual scroll");
                ((IJavaScriptExecutor)Driver).ExecuteScript("mobile: scroll", 
                    new Dictionary<string, string> { { "direction", "down" }, { "percent", "0.8" } });
                Thread.Sleep(1000);
            }
            catch (Exception scrollEx)
            {
                Console.WriteLine($"[CartPage] Manual scroll failed: {scrollEx.Message}");
            }
        }
        
        // Get all products with retries
        var maxAttempts = 3;
        var attempt = 0;
        IReadOnlyCollection<IWebElement> prices = null;
        IReadOnlyCollection<IWebElement> titles = null;
        var allPrices = new List<IWebElement>();
        var allTitles = new List<IWebElement>();
        var previousCount = 0;
        
        while (attempt < maxAttempts)
        {
            try
            {
                prices = Driver.FindElements(_productPrices);
                titles = Driver.FindElements(_productTitles);
                
                // Add any new elements we haven't seen before
                foreach (var price in prices)
                {
                    if (!allPrices.Any(p => p.Text == price.Text))
                    {
                        allPrices.Add(price);
                    }
                }
                
                foreach (var title in titles)
                {
                    if (!allTitles.Any(t => t.Text == title.Text))
                    {
                        allTitles.Add(title);
                    }
                }
                
                // If we haven't found any new elements and we have some elements, we can stop
                if (allPrices.Count == previousCount && allPrices.Count > 0)
                {
                    break;
                }
                
                previousCount = allPrices.Count;
                
                // Try scrolling again if we haven't found all items
                if (allPrices.Count < 3)
                {
                    Console.WriteLine($"[CartPage] Found {allPrices.Count} items, attempting to scroll for more");
                    try
                    {
                        ((IJavaScriptExecutor)Driver).ExecuteScript("mobile: scroll", 
                            new Dictionary<string, string> { { "direction", "down" }, { "percent", "0.5" } });
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[CartPage] Scroll attempt {attempt + 1} failed: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"[CartPage] Attempt {attempt + 1}: Found total of {allPrices.Count} prices and {allTitles.Count} titles");
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CartPage] Error on attempt {attempt + 1}: {ex.Message}");
            }
            attempt++;
        }

        Console.WriteLine($"[CartPage] Found {allPrices.Count} products in cart");
        for (int i = 0; i < allPrices.Count; i++)
        {
            Console.WriteLine($"[CartPage] Product {i + 1}: {allTitles[i].Text} - {allPrices[i].Text}");
        }
        
        return allPrices.Select(p => decimal.Parse(p.Text.Replace("$", ""))).ToList();
    }
}