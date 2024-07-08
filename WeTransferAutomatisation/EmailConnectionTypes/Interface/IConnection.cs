using Aspose.Email;

namespace WeTransferAutomatisation.EmailConnectionTypes.Interface;

public interface IConnection : IDisposable
{
    public bool Connect();
    public void Disconnect();
    public Task<List<MailMessage>> List(string folder = "inbox");
    public bool Send(MailMessage message);
    public void Delete(MailMessage message);
    public void Move(MailMessage message, string folder);
    public void Move(List<MailMessage> messages, string folder);
}