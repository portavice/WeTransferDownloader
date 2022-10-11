using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using WeTransferDownloader.Enums;
using WeTransferDownloader.Utils.BrowserClients;

namespace WeTransferDownloader.Handler
{
    public class DownloadHandler
    {
        private readonly BrowserType? type = null;
        private readonly IWebDriver? driver = null;
        private readonly string downloadpath = "";
        private static readonly DownloadHandler? instance = null;
        public DownloadHandler()
        {
            if (instance == null) return;
            type = instance.type;
            driver = instance.driver;
            downloadpath = instance.downloadpath;
        }

        public DownloadHandler(Chrome chrome)
        {
            type = BrowserType.Chrome;
            downloadpath = chrome.DownloadPath;
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.EnableVerboseLogging = false;
            service.EnableAppendLog = false;
            ChromeOptions options = new();
            options.AddUserProfilePreference("download.default_directory", chrome.DownloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            driver = new ChromeDriver(service, options);
        }

        public DownloadHandler(Firefox firefox)
        {
            type = BrowserType.Firefox;
            downloadpath = firefox.DownloadPath; 
            FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            FirefoxProfile profile = new();
            profile.SetPreference("browser.download.dir", firefox.DownloadPath);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "*");
            driver = new FirefoxDriver(service, new FirefoxOptions() { Profile = profile });
        }

        public DownloadHandler(Edge edge)
        {
            type = BrowserType.Edge;
            downloadpath = edge.DownloadPath;
            EdgeDriverService service = EdgeDriverService.CreateDefaultService();
            service.SuppressInitialDiagnosticInformation = true;
            service.EnableVerboseLogging = false;
            service.EnableAppendLog = false;
            EdgeOptions options = new();
            options.AddUserProfilePreference("download.default_directory", edge.DownloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            driver = new EdgeDriver(service, options);
        }

        public IWebDriver? GetDriver() => driver;

        public async Task DownloadWeTransfer(string url)
        {
            if (driver == null || type == null) return;
            try
            {
                driver.Navigate().GoToUrl(url);

                await Task.Delay(500);
                await ClickButton("welcome__button--decline-experiment");
                await ClickButton("transfer__button");
                await ClickButton("transfer__button");

                await Task.Delay(1000);

                await CheckIfDownloadisFinished();
            }
            catch (Exception) { }
            Stop();
        }

        public void Stop()
        {
            if (driver == null) return;
            try
            {
                driver.Close();
                driver.Dispose();
            }
            catch (Exception) { }
        }


        private async Task ClickButton(string classname)
        {
            if (driver == null) return;
            IWebElement element = driver.FindElement(By.ClassName(classname));
            if (element == null) return;

            bool buttonenabled = element.Enabled;
            while (!buttonenabled)
            {
                await Task.Delay(500);
                buttonenabled = element.Enabled;
            }
            element.Click();
        }

        public async Task CheckIfDownloadisFinished()
        {
            bool downloadFinished = false;
            switch (type)
            {
                case BrowserType.Chrome:
                case BrowserType.Edge:
                    while (!downloadFinished)
                    {
                        foreach (string fi in Directory.GetFiles(downloadpath))
                        {
                            downloadFinished = !Path.GetExtension(fi).Contains("crdownload");
                            if (downloadFinished)
                            {
                                break;
                            }
                        }
                        await Task.Delay(1000);
                    }
                    break;
                case BrowserType.Firefox:
                    while (!downloadFinished)
                    {
                        foreach (string fi in Directory.GetFiles(downloadpath))
                        {
                            downloadFinished = !Path.GetExtension(fi).Contains("part");
                            if (downloadFinished)
                            {
                                break;
                            }
                        }
                        await Task.Delay(1000);
                    }
                    break;
            }
        }
    }
}
