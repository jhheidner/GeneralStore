using GeneralStore.Mobile.Tests.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace GeneralStore.Mobile.Tests.Pages;

public class ProductsPage : BasePage
{
  private readonly By _productTitle = By.Id("com.androidsample.generalstore:id/productName");
  private readonly By _addToCartButton = By.XPath("//android.widget.TextView[@text='ADD TO CART']");
  private readonly By _cartButton = By.Id("com.androidsample.generalstore:id/appbar_btn_cart");
  private readonly By _productList = By.Id("com.androidsample.generalstore:id/rvProductList");
  private readonly By _productPrice = By.Id("com.androidsample.generalstore:id/productPrice");
  private readonly HashSet<string> _addedProducts = new HashSet<string>();

  public ProductsPage(AndroidDriver<IWebElement> driver) : base(driver) { }

  public ProductsPage AddProductToCart(string productName)
  {
    if (_addedProducts.Contains(productName))
    {
      Console.WriteLine($"[ProductsPage] Warning: Product '{productName}' has already been added to cart");
    }
    
    Console.WriteLine($"[ProductsPage] Attempting to add product: {productName}");
    
    // Wait for product list to be visible and stable
    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
    wait.Until(d => {
      try {
        var list = d.FindElement(_productList);
        return list != null && list.Displayed && list.Size.Height > 0;
      }
      catch {
        return false;
      }
    });
    
    Thread.Sleep(2000); // Additional wait for list stability

    // Try to find and add the product
    var maxAttempts = 3;
    var attempts = 0;
    var added = false;

    while (!added && attempts < maxAttempts)
    {
      try
      {
        Console.WriteLine($"[ProductsPage] Attempt {attempts + 1} to add {productName}");
        
        // First try direct search with UIAutomator
        try
        {
          var selector = $"new UiScrollable(new UiSelector().resourceId(\"com.androidsample.generalstore:id/rvProductList\"))" +
                        $".setMaxSearchSwipes(10)" +
                        $".scrollIntoView(new UiSelector().textContains(\"{productName}\"))";
          
          Console.WriteLine($"[ProductsPage] Attempting UIAutomator scroll for {productName}");
          Driver.FindElementByAndroidUIAutomator(selector);
          Thread.Sleep(1500); // Wait after scroll
          Console.WriteLine($"[ProductsPage] UIAutomator scroll successful for {productName}");
        }
        catch (Exception ex)
        {
          Console.WriteLine($"[ProductsPage] UIAutomator scroll failed for {productName}: {ex.Message}");
          // If UIAutomator scroll fails, try manual scroll
          Console.WriteLine($"[ProductsPage] Attempting manual scroll for {productName}");
          for (int i = 0; i < 3 && !added; i++)
          {
            var products = Driver.FindElements(_productTitle)
                               .Where(e => e.Displayed)
                               .ToList();
            
            Console.WriteLine($"[ProductsPage] Found {products.Count} visible products");
            foreach (var p in products)
            {
              Console.WriteLine($"[ProductsPage] Visible product: {p.Text}");
            }
            
            var foundProduct = products.FirstOrDefault(p => 
              p.Text.Contains(productName, StringComparison.OrdinalIgnoreCase));
            
            if (foundProduct != null)
            {
              Console.WriteLine($"[ProductsPage] Found product {productName} in visible products");
              break;
            }
            
            Console.WriteLine($"[ProductsPage] Product not found, scrolling down");
            ((IJavaScriptExecutor)Driver).ExecuteScript("mobile: scroll", 
              new Dictionary<string, string> { { "direction", "down" }, { "percent", "0.8" } });
            Thread.Sleep(1000);
          }
        }

        // Now try to find and click the Add to Cart button
        var visibleProducts = Driver.FindElements(_productTitle)
                                  .Where(e => e.Displayed)
                                  .ToList();
        
        var addButtons = Driver.FindElements(_addToCartButton)
                             .Where(e => e.Displayed)
                             .ToList();

        Console.WriteLine($"[ProductsPage] Found {visibleProducts.Count} visible products and {addButtons.Count} ADD TO CART buttons");

        for (var i = 0; i < visibleProducts.Count; i++)
        {
          Console.WriteLine($"[ProductsPage] Checking product {i + 1}: {visibleProducts[i].Text}");
          if (visibleProducts[i].Text.Contains(productName, StringComparison.OrdinalIgnoreCase))
          {
            if (i < addButtons.Count)
            {
              try
              {
                // Ensure button is clickable
                wait.Until(d => {
                  try {
                    return addButtons[i].Enabled && addButtons[i].Displayed;
                  }
                  catch {
                    return false;
                  }
                });

                Console.WriteLine($"[ProductsPage] Clicking ADD TO CART button for {productName}");
                addButtons[i].Click();
                Thread.Sleep(2000); // Wait longer for animation
                added = true;
                _addedProducts.Add(productName);
                Console.WriteLine($"[ProductsPage] Successfully added {productName} to cart");
                break;
              }
              catch (Exception ex)
              {
                Console.WriteLine($"[ProductsPage] Direct click failed for {productName}: {ex.Message}");
                // Fallback to Actions if direct click fails
                Console.WriteLine($"[ProductsPage] Attempting Actions click for {productName}");
                var actions = new Actions(Driver);
                actions.MoveToElement(addButtons[i])
                       .Click()
                       .Perform();
                Thread.Sleep(2000);
                added = true;
                _addedProducts.Add(productName);
                Console.WriteLine($"[ProductsPage] Successfully added {productName} to cart using Actions");
                break;
              }
            }
            else
            {
              Console.WriteLine($"[ProductsPage] Found product {productName} but no matching ADD TO CART button");
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ProductsPage] Attempt {attempts + 1} failed for {productName}: {ex.Message}");
        attempts++;
        if (attempts >= maxAttempts)
        {
          throw new Exception($"Failed to add product '{productName}' after {maxAttempts} attempts", ex);
        }
        Thread.Sleep(1500);
      }
    }

    if (!added)
    {
      throw new NoSuchElementException($"Could not add product '{productName}' to cart");
    }

    return this;
  }

  public decimal GetProductPrice(string productName)
  {
    ScrollToText(productName);
    var priceText = Driver.FindElementById("com.androidsample.generalstore:id/productPrice").Text;
    return decimal.Parse(priceText.Replace("$", ""));
  }

  public CartPage GoToCart()
  {
    Console.WriteLine("[ProductsPage] Attempting to go to cart");
    Console.WriteLine($"[ProductsPage] Added products before going to cart: {string.Join(", ", _addedProducts)}");
    Thread.Sleep(2000); // Wait longer for any pending cart updates
    var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
    var cartButton = wait.Until(d => {
      var elem = d.FindElement(_cartButton);
      return elem.Displayed ? elem : null;
    });
    Console.WriteLine("[ProductsPage] Found cart button, clicking it");
    cartButton?.Click();
    Thread.Sleep(2000); // Wait for cart page to load
    Console.WriteLine("[ProductsPage] Navigated to cart page");
    return new CartPage(Driver);
  }
}