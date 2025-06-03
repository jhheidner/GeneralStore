using Microsoft.Extensions.Configuration;

namespace GeneralStore.Mobile.Tests.Configuration;

public class AppSettings
{
  public string PlatformName { get; set; } = "Android";
  public string DeviceName { get; set; } = "Pixel_5";
  public string AutomationName { get; set; } = "UiAutomator2";
  public string AppPackage { get; set; } = "com.androidsample.generalstore";
  public string AppActivity { get; set; } = "com.androidsample.generalstore.MainActivity";
  public string App { get; set; } = "General-Store.apk";
  public string AppiumUrl { get; set; } = "http://127.0.0.1:4723/wd/hub";

  public static AppSettings Load()
  {
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json")
        .Build();

    return config.Get<AppSettings>() ?? new AppSettings();
  }
}