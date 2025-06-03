using GeneralStore.Mobile.Tests.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;

namespace GeneralStore.Mobile.Tests.Pages;

public class FormPage : BasePage
{
  private readonly By _countryDropdown = By.Id("com.androidsample.generalstore:id/spinnerCountry");
  private readonly By _nameField = By.Id("com.androidsample.generalstore:id/nameField");
  private readonly By _femaleRadioButton = By.Id("com.androidsample.generalstore:id/radioFemale");
  private readonly By _maleRadioButton = By.Id("com.androidsample.generalstore:id/radioMale");
  private readonly By _shopButton = By.Id("com.androidsample.generalstore:id/btnLetsShop");
  private readonly By _toastMessage = By.XPath("//android.widget.Toast");

  public FormPage(AndroidDriver<IWebElement> driver) : base(driver) { }

  public FormPage SelectCountry(string country)
  {
    WaitAndFindElement(_countryDropdown).Click();
    ScrollToText(country);
    Driver.FindElementByXPath($"//android.widget.TextView[@text='{country}']").Click();
    return this;
  }

  public FormPage EnterName(string name)
  {
    WaitAndFindElement(_nameField).SendKeys(name);
    Driver.HideKeyboard();
    return this;
  }

  public FormPage SelectGender(string gender)
  {
    var radioButton = gender.ToLower() == "female" ? _femaleRadioButton : _maleRadioButton;
    WaitAndFindElement(radioButton).Click();
    return this;
  }

  public ProductsPage ClickLetsShop()
  {
    WaitAndFindElement(_shopButton).Click();
    return new ProductsPage(Driver);
  }

  public string GetToastMessage()
  {
    return WaitAndFindElement(_toastMessage).Text;
  }
}