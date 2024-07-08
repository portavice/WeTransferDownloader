using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WeTransferAutomatisation.BrowserTypes.Interface;

namespace WeTransferAutomatisation.BrowserTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Chrome : IBrowserType
{
    private readonly ChromeDriver _driver;

    public Chrome(string downloadPath, bool silent = true)
    {
        DownloadPath = downloadPath;
        var service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        service.EnableVerboseLogging = false;
        service.HideCommandPromptWindow = true;
        service.EnableAppendLog = false;
        ChromeOptions options = new();
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

        options.AddUserProfilePreference("download.default_directory", downloadPath);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        _driver = new ChromeDriver(service, options);
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
                downloadFinished = !Path.GetExtension(fi).Contains("crdownload");
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