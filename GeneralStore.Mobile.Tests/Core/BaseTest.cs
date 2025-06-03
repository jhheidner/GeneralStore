using GeneralStore.Mobile.Tests.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace GeneralStore.Mobile.Tests.Core;

public class BaseTest : IDisposable
{
    protected AndroidDriver<IWebElement> Driver { get; set; } = null!;
    protected AppSettings Settings { get; set; } = null!;

    public BaseTest()
    {
        Settings = AppSettings.Load();
        InitializeDriver();
    }

    private void InitializeDriver()
    {
        var appiumOptions = new AppiumOptions();
        appiumOptions.AddAdditionalCapability("platformName", Settings.PlatformName);
        appiumOptions.AddAdditionalCapability("appium:deviceName", Settings.DeviceName);
        appiumOptions.AddAdditionalCapability("appium:automationName", Settings.AutomationName);
        appiumOptions.AddAdditionalCapability("appium:app", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Settings.App));
        appiumOptions.AddAdditionalCapability("appium:autoGrantPermissions", true);
        appiumOptions.AddAdditionalCapability("appium:noReset", false);
        appiumOptions.AddAdditionalCapability("appium:fullReset", true);
        appiumOptions.AddAdditionalCapability("appium:enforceAppInstall", true);
        appiumOptions.AddAdditionalCapability("appium:androidInstallTimeout", 90000);
        appiumOptions.AddAdditionalCapability("appium:intentAction", "android.intent.action.MAIN");
        appiumOptions.AddAdditionalCapability("appium:intentCategory", "android.intent.category.LAUNCHER");
        appiumOptions.AddAdditionalCapability("appium:forceAppLaunch", true);

        // These will be set by Appium after app installation
        // appiumOptions.AddAdditionalCapability("appium:appPackage", Settings.AppPackage);
        // appiumOptions.AddAdditionalCapability("appium:appActivity", Settings.AppActivity);

        Driver = new AndroidDriver<IWebElement>(new Uri(Settings.AppiumUrl), appiumOptions);
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}