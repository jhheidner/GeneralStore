using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

namespace GeneralStore.Mobile.Tests.Core;

public class BasePage
{
  protected readonly AndroidDriver<IWebElement> Driver;
  protected readonly WebDriverWait Wait;

  protected BasePage(AndroidDriver<IWebElement> driver)
  {
    Driver = driver;
    Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
  }

  protected IWebElement WaitAndFindElement(By by)
  {
    return Wait.Until(d => d.FindElement(by));
  }

  protected void ScrollToText(string text)
  {
    Driver.FindElementByAndroidUIAutomator(
        $"new UiScrollable(new UiSelector().scrollable(true)).scrollIntoView(new UiSelector().text(\"{text}\"))");
  }

  protected bool IsElementPresent(By by)
  {
    try
    {
      Driver.FindElement(by);
      return true;
    }
    catch (NoSuchElementException)
    {
      return false;
    }
  }

  protected void WaitForElementToDisappear(By by)
  {
    Wait.Until(d => !IsElementPresent(by));
  }
}