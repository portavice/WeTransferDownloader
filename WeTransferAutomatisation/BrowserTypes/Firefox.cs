using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using WeTransferAutomatisation.BrowserTypes.Interface;

namespace WeTransferAutomatisation.BrowserTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Firefox : IBrowserType
{
    private readonly FirefoxDriver _driver;

    public Firefox(string downloadPath, bool silent = true)
    {
        DownloadPath = downloadPath;
        var service = FirefoxDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        FirefoxProfile profile = new();
        profile.SetPreference("browser.download.dir", DownloadPath);
        profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "*");
        FirefoxOptions options = new() { Profile = profile };
        if (silent)
        {
            options.AddArguments("--disable-extensions");
            options.AddArgument("test-type");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("no-sandbox");
            options.AddArgument("headless");
            options.AddArgument("--silent");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--log-level=3");
        }

        _driver = new FirefoxDriver(service, options);
    }

    public string DownloadPath { get; }

    public IWebDriver GetDriver()
    {
        return _driver;
    }

    public async Task CheckIfDownloadIsFinished()
    {
        var downloadFinished = false;
        while (!downloadFinished)
        {
            foreach (var fi in Directory.GetFiles(DownloadPath))
            {
                downloadFinished = !Path.GetExtension(fi).Contains("part");
                if (downloadFinished) break;
            }

            await Task.Delay(1000);
        }
    }

    public void Stop()
    {
        Dispose();
    }

    public void Dispose()
    {
        _driver.Close();
        _driver.Quit();
        _driver.Dispose();
        GC.SuppressFinalize(this);
    }
}