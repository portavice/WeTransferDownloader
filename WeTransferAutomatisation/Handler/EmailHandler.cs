using Aspose.Email.Clients;
using Aspose.Email.Clients.Imap;
using Aspose.Email.Clients.Exchange;
using WeTransferDownloader.Enums;
using WeTransferDownloader.Utils;
using Aspose.Email.Clients.Exchange.WebService;
using System.Net;
using Aspose.Email;

namespace WeTransferDownloader.Handler
{
    public class EmailHandler
    {
        private readonly EmailType? type = null;
        private readonly ImapClient? imapclient = null;
        private readonly IEWSClient? ewsclient = null;
        private static EmailHandler? instance = null;

        public EmailHandler()
        {
            if (instance == null) return;
            imapclient = instance.imapclient;
            ewsclient = instance.ewsclient;
            type = instance.type;
        }

        public EmailHandler(IMAP imap)
        {
            type = EmailType.IMAP;
            imapclient = new ImapClient(imap.host, imap.port, imap.username, imap.password, imap.securityOptions);
            imapclient.CreateConnection();
            instance = this;
        }

        public EmailHandler(EWS ews)
        {
            type = EmailType.EWS;
            ewsclient = EWSClient.GetEWSClient(ews.host, new NetworkCredential(ews.username, ews.password, ews.domain));
            instance = this;
        }

        public ExchangeMessageInfoCollection GetEWSEmails()
        {
            if (instance == null || type == null || ewsclient == null) return new();
            return ewsclient.ListMessages(ewsclient.GetMailboxInfo().InboxUri);
        }

        public ImapMessageInfoCollection GetIMAPEmails()
        {
            if (instance == null || type == null || imapclient == null || imapclient.ConnectionState != ConnectionState.Open) return new();
            return imapclient.ListMessages();
        }

        public dynamic? GetClient() => type switch
        {
            EmailType.IMAP => imapclient,
            EmailType.EWS => ewsclient,
            _ => null
        };

        public List<MessageInfoBase> GetWeTransferMails()
        {
            if (instance == null || type == null || (ewsclient == null && imapclient == null)) return new();
            List<MessageInfoBase> msgs = new();
            switch (type)
            {
                case EmailType.IMAP:
                    foreach (ImapMessageInfo msginfo in GetIMAPEmails())
                    {
                        if (!msginfo.From.Address.Contains("wetransfer")) continue;
                        msgs.Add(msginfo);
                    }
                    return msgs;
                case EmailType.EWS:
                    foreach (ExchangeMessageInfo msginfo in GetEWSEmails())
                    {
                        if (!msginfo.From.Address.Contains("wetransfer")) continue;
                        msgs.Add(msginfo);
                    }
                    return msgs;
            }
            return new();
        }

        public List<string> DownloadWeTransfer(DownloadHandler downloader)
        {
            List<MessageInfoBase> msgs = GetWeTransferMails();
            List<string> result = new();
            if (msgs == null || msgs.Count < 1) return result;
            foreach (MessageInfoBase msginfo in msgs)
            {
                try
                {
                    MailMessage? mail = type switch
                    {
                        EmailType.IMAP => imapclient?.FetchMessage(((ImapMessageInfo)msginfo).SequenceNumber),
                        EmailType.EWS => ewsclient?.FetchMessage(((ExchangeMessageInfo)msginfo).UniqueUri),
                        _ => null
                    };
                    if (mail == null) continue;
                    if (!mail.HtmlBody.Contains("<span style=\"color:#5268ff;font-weight:normal;text-decoration:underline;word-wrap:break-word\">")) continue;
                    string downloadURL = mail.HtmlBody.Split("<span style=\"color:#5268ff;font-weight:normal;text-decoration:underline;word-wrap:break-word\">")[1].
                        Split("</span>")[0];
                    downloader.DownloadWeTransfer(downloadURL);
                    result.Add(type switch
                    {
                        EmailType.IMAP => ((ImapMessageInfo)msginfo).SequenceNumber.ToString(),
                        EmailType.EWS => ((ExchangeMessageInfo)msginfo).UniqueUri,
                        _ => msginfo.MessageId.ToString()
                    });
                }
                catch (Exception) { }
            }
            return new();
        }
    }
}
