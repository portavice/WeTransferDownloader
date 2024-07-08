using Aspose.Email;
using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Me.Messages.Item.Move;
using Microsoft.Graph.Me.SendMail;
using Microsoft.Graph.Models;

namespace WeTransferAutomatisation.EmailConnectionTypes.GraphHelpers;

public class GraphHelper
{
    //Scope for Mail Read, Mail Send, Mail Delete, Mail Move
    private static readonly string[] Scopes = { "user.read", "mail.read", "mail.send", "mail.readwrite" };
    private readonly string _clientId;
    private readonly string _tenantId;


    private DeviceCodeCredential? _deviceCodeCredential;

    private GraphServiceClient? _userClient;

    public GraphHelper(string clientId, string tenantId)
    {
        _clientId = clientId;
        _tenantId = tenantId;
    }

    private GraphServiceClient UserClient
    {
        get
        {
            if (_userClient == null) InitializeGraphForUserAuth();
            if (_userClient == null) throw new Exception("Graph client not initialized");
            return _userClient;
        }
    }

    private void InitializeGraphForUserAuth()
    {
        _deviceCodeCredential = new DeviceCodeCredential(new DeviceCodeCredentialOptions
        {
            ClientId = _clientId,
            TenantId = _tenantId
        });
        _userClient = new GraphServiceClient(_deviceCodeCredential, Scopes);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        if (_deviceCodeCredential == null) InitializeGraphForUserAuth();
        if (_deviceCodeCredential == null) return null;
        var result = await _deviceCodeCredential.GetTokenAsync(new TokenRequestContext(Scopes));
        return result.Token;
    }

    public Task<User?> GetUserAsync()
    {
        return UserClient.Me.GetAsync(config =>
        {
            config.QueryParameters.Select = new[] { "displayName", "mail", "userPrincipalName" };
        });
    }

    public async Task<List<Message>?> ListEmails(string folder = "Inbox")
    {
        return (await UserClient.Me.MailFolders[folder].Messages
            .GetAsync(config =>
            {
                //Select all messages from the inbox
                config.QueryParameters.Select = new[]
                    { "id", "subject", "body", "from", "toRecipients", "receivedDateTime" };
                config.QueryParameters.Orderby = new[] { "receivedDateTime DESC" };
            }))?.Value;
    }

    public async void SendEmail(MailMessage message)
    {
        var recipients = message.To
            .Select(to => new Recipient { EmailAddress = new EmailAddress { Address = to.Address } }).ToList();
        var ccRecipients = message.CC
            .Select(cc => new Recipient { EmailAddress = new EmailAddress { Address = cc.Address } }).ToList();
        var email = new Message
        {
            Subject = message.Subject,
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = message.HtmlBody
            },
            CcRecipients = ccRecipients,
            ToRecipients = recipients
        };
        await UserClient.Me.SendMail.PostAsync(new SendMailPostRequestBody { Message = email, SaveToSentItems = true });
    }

    public async void DeleteEmail(string messageId)
    {
        await UserClient.Me.Messages[messageId].Move
            .PostAsync(new MovePostRequestBody { DestinationId = "deleteditems" });
    }

    public async void MoveEmail(string folder, string messageId)
    {
        await UserClient.Me.Messages[messageId].Move.PostAsync(new MovePostRequestBody { DestinationId = folder });
    }
}