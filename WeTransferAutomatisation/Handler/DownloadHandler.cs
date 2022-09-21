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
            ChromeOptions options = new();
            options.AddUserProfilePreference("download.default_directory", chrome.DownloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            driver = new ChromeDriver(options);
        }

        public DownloadHandler(Firefox firefox)
        {
            type = BrowserType.Firefox;
            downloadpath = firefox.DownloadPath;
            FirefoxProfile profile = new();
            profile.SetPreference("browser.download.dir", firefox.DownloadPath);
            profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", "*");
            driver = new FirefoxDriver(new FirefoxOptions() { Profile = profile });
        }

        public DownloadHandler(Edge edge)
        {
            type = BrowserType.Chrome;
            downloadpath = edge.DownloadPath;
            EdgeOptions options = new();
            options.AddUserProfilePreference("download.default_directory", edge.DownloadPath);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            driver = new EdgeDriver(options);
        }

        public IWebDriver? GetDriver() => driver;

        public async void DownloadWeTransfer(string url)
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
                driver.Dispose();
                driver.Close();
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

        public async Task<bool> CheckIfDownloadisFinished()
        {
            string[] files = Directory.GetFiles(downloadpath);
            switch (type)
            {
                case BrowserType.Chrome:
                case BrowserType.Edge:
                    while (true)
                    {
                        foreach (string file in files)
                        {
                            if (!Path.GetExtension(file).EndsWith("crdownload"))
                            {
                                return true;
                            }
                        }
                        await Task.Delay(1000);
                    }
                case BrowserType.Firefox:
                    while (true)
                    {
                        foreach (string file in files)
                        {
                            if (!Path.GetExtension(file).EndsWith("part"))
                            {
                                return true;
                            }
                        }
                        await Task.Delay(1000);
                    }
            }
            return false;
        }
    }
}
