using System.Diagnostics.CodeAnalysis;
using Aspose.Email;
using WeTransferAutomatisation.EmailConnectionTypes;
using IConnection = WeTransferAutomatisation.EmailConnectionTypes.Interface.IConnection;

namespace WeTransferAutomatisation.Handler;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class EmailHandler
{
    private static EmailHandler? _instance;
    public readonly IConnection? Client;

    public EmailHandler()
    {
        if (_instance == null) return;
        Client = _instance.Client;
    }

    public EmailHandler(Imap imap)
    {
        Client = imap;
        _instance = this;
    }

    public EmailHandler(Ews ews)
    {
        Client = ews;
        _instance = this;
    }

    public EmailHandler(Graph graph)
    {
        Client = graph;
        _instance = this;
    }

    public EmailHandler(Pop pop)
    {
        Client = pop;
        _instance = this;
    }


    public async Task<List<MailMessage>> GetWeTransferMails()
    {
        if (_instance == null || Client == null) return new List<MailMessage>();
        return (await Client.List()).Where(msgInfo => msgInfo.From.Address.ToLower().Contains("noreply@wetransfer.com"))
            .ToList();
    }

    public static string GetWeTransferDownloadLink(MailMessage mail)
    {
        if (!mail.HtmlBody.Contains("wetransfer.com/downloads/")) return "";
        return "https://wetransfer.com/downloads/" + mail.HtmlBody.Split("wetransfer.com/downloads/")[1].Split("\"")[0];
    }

    public async Task<List<string>> DownloadWeTransfer(DownloadHandler downloader)
    {
        var messages = await GetWeTransferMails();
        List<string> result = new();
        if (messages.Count < 1) return result;
        foreach (var msgInfo in messages)
            try
            {
                var downloadUrl = GetWeTransferDownloadLink(msgInfo);
                if (downloadUrl == "") continue;
                await downloader.DownloadWeTransfer(downloadUrl);
                result.Add(msgInfo.MessageId);
            }
            catch (Exception)
            {
                // ignored
            }

        return result;
    }

    public void Delete(MailMessage mail)
    {
        Client?.Delete(mail);
    }

    public bool Send(MailMessage message)
    {
        return Client?.Send(message) ?? false;
    }
}