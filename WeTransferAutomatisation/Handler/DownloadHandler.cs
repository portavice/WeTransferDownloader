using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using OpenQA.Selenium;
using WeTransferAutomatisation.BrowserTypes;
using WeTransferAutomatisation.BrowserTypes.Interface;

namespace WeTransferAutomatisation.Handler;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
// ReSharper disable once ClassNeverInstantiated.Global
public class DownloadHandler
{
    private static DownloadHandler? _instance;
    public readonly IBrowserType? Type;

    public DownloadHandler()
    {
        if (_instance == null) return;
        Type = _instance.Type;
    }

    public DownloadHandler(Chrome chrome)
    {
        Type = chrome;
        _instance = this;
    }

    public DownloadHandler(Edge edge)
    {
        Type = edge;
        _instance = this;
    }

    public DownloadHandler(Firefox firefox)
    {
        Type = firefox;
        _instance = this;
    }

    public IWebDriver? GetDriver()
    {
        return Type?.GetDriver();
    }

    public async Task DownloadWeTransfer(string url)
    {
        if (Type == null) return;
        try
        {
            var driver = Type.GetDriver();
            driver.Navigate().GoToUrl(url);
            driver.Manage().Window.Size = new Size(1212, 824);
            await Task.Delay(1500);

            await ClickButton(".fides-banner-button.fides-banner-button-primary.fides-reject-all-button");
            await Task.Delay(500);

            await ClickButton(".transfer__button");
            await Task.Delay(500);

            await ClickButton(".transfer__button");
            await Task.Delay(1000);

            await CheckIfDownloadIsFinished();
        }
        catch (Exception)
        {
            // ignored
        }

        Stop();
    }

    public void Stop()
    {
        Type?.Stop();
    }


    private async Task ClickButton(string classname)
    {
        var element = Type?.GetDriver().FindElement(By.CssSelector(classname));
        if (element == null) return;

        var elementEnabled = element.Enabled;
        while (!elementEnabled)
        {
            await Task.Delay(500);
            elementEnabled = element.Enabled;
        }

        element.Click();
    }

    public async Task CheckIfDownloadIsFinished()
    {
        if (Type == null) return;
        await Type.CheckIfDownloadIsFinished();
    }
}