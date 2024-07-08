using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Base;
using Aspose.Email.Clients.Imap;
using Interface_IConnection = WeTransferAutomatisation.EmailConnectionTypes.Interface.IConnection;

namespace WeTransferAutomatisation.EmailConnectionTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Imap : Interface_IConnection
{
    private readonly EncryptionProtocols _encryptionProtocols = EncryptionProtocols.Tls;
    private readonly string _folder;

    private readonly string _host;
    private readonly string _password;
    private readonly int _port = 143;
    private readonly SecurityOptions _securityOptions = SecurityOptions.None;
    private readonly string _username;

    private ImapClient? _clientImap;

    public Imap(string host, string username, string password, string folder = "inbox")
    {
        _host = host;
        _username = username;
        _password = password;
        _folder = folder;
    }

    public Imap(string host, int port, string username, string password, string folder = "inbox")
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _folder = folder;
    }

    public Imap(string host, int port, string username, string password, string folder = "inbox",
        SecurityOptions securityOptions = SecurityOptions.None,
        EncryptionProtocols encryptionProtocols = EncryptionProtocols.Tls)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _securityOptions = securityOptions;
        _folder = folder;
        _encryptionProtocols = encryptionProtocols;
    }

    public bool Connect()
    {
        _clientImap = new ImapClient
        {
            Host = _host,
            Port = _port,
            Username = _username,
            Password = _password,
            SecurityOptions = _securityOptions,
            SupportedEncryption = _encryptionProtocols
        };
        return _clientImap != null;
    }

    public void Disconnect()
    {
        _clientImap?.StopMonitoring();
        Dispose();
    }

    public Task<List<MailMessage>> List(string? folder = null)
    {
        return Task.FromResult(
            _clientImap?.ListMessages(folder ?? _folder).ToList()
                .Select(x => _clientImap.FetchMessage(x.SequenceNumber))
                .ToList() ?? new List<MailMessage>());
    }

    public bool Send(MailMessage message)
    {
        return false;
    }

    public void Delete(MailMessage message)
    {
        var messageInfo = GetMessageInfo(message);
        if (messageInfo == null) return;
        _clientImap?.DeleteMessage(messageInfo.SequenceNumber);
    }

    public void Move(MailMessage message, string folder)
    {
        var messageInfo = GetMessageInfo(message);
        if (messageInfo == null) return;
        _clientImap?.MoveMessage(messageInfo.SequenceNumber, folder);
    }

    public void Move(List<MailMessage> messages, string folder)
    {
        var messageInfos = _clientImap?.ListMessages().Where(x => messages.Any(y => y.MessageId == x.MessageId))
            .ToList();
        if (messageInfos == null) return;
        foreach (var messageInfo in messageInfos) _clientImap?.MoveMessage(messageInfo.SequenceNumber, folder);
    }

    public void Dispose()
    {
        _clientImap?.Dispose();
    }

    private ImapMessageInfo? GetMessageInfo(MailMessage message)
    {
        return _clientImap?.ListMessages().FirstOrDefault(x => x.MessageId == message.MessageId);
    }
}