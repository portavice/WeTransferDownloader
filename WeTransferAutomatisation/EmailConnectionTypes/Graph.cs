using Aspose.Email;
using WeTransferAutomatisation.EmailConnectionTypes.GraphHelpers;
using WeTransferAutomatisation.EmailConnectionTypes.Interface;

namespace WeTransferAutomatisation.EmailConnectionTypes;

// ReSharper disable once ClassNeverInstantiated.Global
public class Graph : IConnection
{
    private readonly GraphHelper _graphHelper;

    public Graph(string clientId, string tenantId)
    {
        _graphHelper = new GraphHelper(clientId, tenantId);
    }

    public bool Connect()
    {
        return true;
    }

    public void Disconnect()
    {
        Dispose();
    }

    public async Task<List<MailMessage>> List(string folder = "inbox")
    {
        var mails = await _graphHelper.ListEmails();
        if (mails == null) return new List<MailMessage>();
        return mails.Select(x => new MailMessage
        {
            MessageId = x.Id,
            HtmlBody = x.Body?.Content,
            Body = x.Body?.Content,
            Subject = x.Subject,
            From = x.From?.EmailAddress?.Address,
            To = new MailAddressCollection { new MailAddress(x.ToRecipients?.First().EmailAddress?.Address) },
            Date = x.ReceivedDateTime?.DateTime ?? DateTime.Now
        }).ToList();
    }

    public bool Send(MailMessage message)
    {
        try
        {
            _graphHelper.SendEmail(message);
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
        _graphHelper.DeleteEmail(message.MessageId);
    }

    public void Move(MailMessage message, string folder)
    {
        _graphHelper.MoveEmail(message.MessageId, folder);
    }

    public void Move(List<MailMessage> messages, string folder)
    {
        foreach (var message in messages) Move(message, folder);
    }

    public void Dispose()
    {
        // Nothing to dispose
    }
}