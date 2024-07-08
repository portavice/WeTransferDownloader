using OpenQA.Selenium;

namespace WeTransferAutomatisation.BrowserTypes.Interface;

public interface IBrowserType : IDisposable
{
    public IWebDriver GetDriver();
    public Task CheckIfDownloadIsFinished();
    public void Stop();
}