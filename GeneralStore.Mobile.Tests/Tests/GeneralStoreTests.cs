using FluentAssertions;
using GeneralStore.Mobile.Tests.Core;
using GeneralStore.Mobile.Tests.Pages;
using Xunit;
using OpenQA.Selenium.Support.UI;

namespace GeneralStore.Mobile.Tests.Tests;

public class GeneralStoreTests : BaseTest
{
  [Fact]
  public void ShouldSubmitFormSuccessfully()
  {
    var formPage = new FormPage(Driver);
    var productsPage = formPage
        .SelectCountry("Brazil")
        .EnterName("John Doe")
        .SelectGender("male")
        .ClickLetsShop();

    // Verify we're on the products page by checking if we can find a product
    productsPage.AddProductToCart("Jordan 6 Rings").Should().NotBeNull();
  }

  [Fact]
  public void ShouldShowErrorWhenNameIsMissing()
  {
    var formPage = new FormPage(Driver);
    formPage
        .SelectCountry("Brazil")
        .SelectGender("female")
        .ClickLetsShop();

    formPage.GetToastMessage().Should().Be("Please enter your name");
  }

  [Fact]
  public void ShouldCalculateCartTotalCorrectly()
  {
    var formPage = new FormPage(Driver);
    var productsPage = formPage
        .SelectCountry("Brazil")
        .EnterName("John Doe")
        .SelectGender("male")
        .ClickLetsShop();

    productsPage
        .AddProductToCart("Jordan 6 Rings")
        .AddProductToCart("Nike SFB Jungle");

    var cartPage = productsPage.GoToCart();
    var prices = cartPage.GetProductPrices();
    var expectedTotal = prices.Sum();
    var actualTotal = cartPage.GetTotalAmount();

    actualTotal.Should().Be(expectedTotal);
  }

  [Fact]
  public void ShouldAddMultipleItemsToCart()
  {
    Console.WriteLine("[Test] Starting ShouldAddMultipleItemsToCart test");
    var formPage = new FormPage(Driver);
    var productsPage = formPage
        .SelectCountry("Brazil")
        .EnterName("John Doe")
        .SelectGender("male")
        .ClickLetsShop();

    // Add first item and verify
    Console.WriteLine("[Test] Adding Jordan 6 Rings to cart");
    productsPage.AddProductToCart("Jordan 6 Rings");
    Thread.Sleep(2000); // Wait for animation
    
    // Add second item and verify
    Console.WriteLine("[Test] Adding Nike SFB Jungle to cart");
    productsPage.AddProductToCart("Nike SFB Jungle");
    Thread.Sleep(2000); // Wait for animation
    
    // Add third item and verify
    Console.WriteLine("[Test] Adding Converse All Star to cart");
    productsPage.AddProductToCart("Converse All Star");
    Thread.Sleep(2000); // Wait for animation

    Console.WriteLine("[Test] Going to cart to verify products");
    var cartPage = productsPage.GoToCart();
    
    // Add extra wait for cart to fully load
    Thread.Sleep(3000);
    
    var productPrices = cartPage.GetProductPrices();
    Console.WriteLine($"[Test] Found {productPrices.Count} products in cart");
    
    // If we don't find 3 products, try refreshing the cart page
    if (productPrices.Count != 3)
    {
        Console.WriteLine("[Test] Did not find 3 products, waiting longer and trying again");
        Thread.Sleep(3000);
        productPrices = cartPage.GetProductPrices();
        Console.WriteLine($"[Test] After retry found {productPrices.Count} products in cart");
    }
    
    productPrices.Count.Should().Be(3, "because we added three different products to the cart");
  }
}