using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Base;
using Aspose.Email.Clients.Pop3;
using IConnection = WeTransferAutomatisation.EmailConnectionTypes.Interface.IConnection;

namespace WeTransferAutomatisation.EmailConnectionTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Pop : IConnection
{
    private readonly EncryptionProtocols _encryptionProtocols;
    private readonly string _host;
    private readonly string _password;
    private readonly int _port;
    private readonly SecurityOptions _securityOptions;
    private readonly string _username;
    private Pop3Client? _client;


    public Pop(string host, int port, string username, string password)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _securityOptions = SecurityOptions.SSLAuto;
        _encryptionProtocols = EncryptionProtocols.Tls;
    }

    public Pop(string host, int port, string username, string password,
        SecurityOptions securityOptions = SecurityOptions.SSLAuto,
        EncryptionProtocols supportedEncryption = EncryptionProtocols.Tls)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _securityOptions = securityOptions;
        _encryptionProtocols = supportedEncryption;
    }

    public bool Connect()
    {
        _client = new Pop3Client
        {
            Host = _host,
            Port = _port,
            Username = _username,
            Password = _password,
            SecurityOptions = _securityOptions,
            SupportedEncryption = _encryptionProtocols
        };
        if (_client.ConnectionState == ConnectionState.Open) return true;
        return _client != null;
    }

    public void Disconnect()
    {
        Dispose();
    }

    public Task<List<MailMessage>> List(string? folder = null)
    {
        return Task.FromResult(
            _client?.ListMessages().ToList().Select(x => _client.FetchMessage(x.SequenceNumber)).ToList() ??
            new List<MailMessage>());
    }

    public bool Send(MailMessage message)
    {
        return false;
    }

    public void Delete(MailMessage message)
    {
        _client?.DeleteMessage(message.MessageId);
    }

    public void Move(MailMessage message, string folder)
    {
    }

    public void Move(List<MailMessage> messages, string folder)
    {
        foreach (var message in messages) Move(message, folder);
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}