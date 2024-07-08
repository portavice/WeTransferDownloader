using System.Net;
using Aspose.Email;
using Aspose.Email.Clients.Exchange.WebService;
using WeTransferAutomatisation.EmailConnectionTypes.Interface;

namespace WeTransferAutomatisation.EmailConnectionTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Ews : IConnection
{
    private readonly string? _domain;
    private readonly string _emailAddress;

    private readonly string _host;
    private readonly string _password;
    private readonly string _username;
    private IEWSClient? _client;

    public Ews(string host, string? domain, string username, string password, string emailAddress)
    {
        _host = host;
        _domain = domain;
        _username = username;
        _password = password;
        _emailAddress = emailAddress;
    }


    public bool Connect()
    {
        NetworkCredential credentials = new(_username, _password, _domain);
        _client = EWSClient.GetEWSClient(_host, credentials);
        return _client != null;
    }

    public void Disconnect()
    {
        Dispose();
    }

    public Task<List<MailMessage>> List(string folder = "inbox")
    {
        return Task.FromResult(_client?.ListMessages(_client.GetMailboxInfo(_emailAddress).InboxUri)
            .ToList()
            .Select(x => _client.FetchMessage(x.UniqueUri)).ToList() ?? new List<MailMessage>());
    }

    public bool Send(MailMessage message)
    {
        try
        {
            _client?.Send(message);
            return true;
        }
        catch (Exception)
        {
            //ignore
        }

        return false;
    }

    public void Delete(MailMessage message)
    {
        var messageInfo = _client?.ListMessages().FirstOrDefault(x => x.MessageId == message.MessageId);
        if (messageInfo == null) return;
        _client?.DeleteItem(messageInfo.UniqueUri, DeletionOptions.MoveToDeletedItems);
    }

    public void Move(MailMessage message, string folder)
    {
        var messageInfo = _client?.ListMessages().FirstOrDefault(x => x.MessageId == message.MessageId);
        if (messageInfo == null) return;
        _client?.MoveItem(messageInfo.UniqueUri, folder);
    }

    public void Move(List<MailMessage> messages, string folder)
    {
        var messageInfos = _client?.ListMessages().Where(x => messages.Any(y => y.MessageId == x.MessageId)).ToList();
        if (messageInfos == null) return;
        foreach (var messageInfo in messageInfos) _client?.MoveItem(messageInfo.UniqueUri, folder);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}