using Aspose.Email.Clients;
using Aspose.Email.Clients.Imap;
using Aspose.Email.Clients.Exchange;
using WeTransferDownloader.Enums;
using Aspose.Email.Clients.Exchange.WebService;
using System.Net;
using Aspose.Email;
using WeTransferDownloader.Utils.EmailClients;

namespace WeTransferDownloader.Handler
{
    public class EmailHandler
    {
        public readonly EmailType? type = null;
        public readonly EmailClients? clientConfig = null;
        private readonly ImapClient? imapclient = null;
        private readonly IEWSClient? ewsclient = null;
        private static EmailHandler? instance = null;

        public EmailHandler()
        {
            if (instance == null) return;
            imapclient = instance.imapclient;
            ewsclient = instance.ewsclient;
            type = instance.type;
            clientConfig = instance.clientConfig;
        }

        public EmailHandler(IMAP imap)
        {
            type = EmailType.IMAP;
            imapclient = new ImapClient(imap.host, imap.port, imap.username, imap.password, imap.securityOptions);
            imapclient.CreateConnection();
            clientConfig = imap;
            instance = this;
        }

        public EmailHandler(EWS ews)
        {
            type = EmailType.EWS;
            ewsclient = EWSClient.GetEWSClient(ews.host, new NetworkCredential(ews.username, ews.password, ews.domain));
            clientConfig = ews;
            instance = this;
        }

        public ExchangeMessageInfoCollection GetEWSEmails()
        {
            if (instance == null || type == null || ewsclient == null || clientConfig == null) return new();
            if(((EWS)clientConfig).emailAdress == "")
            {
                return ewsclient.ListMessages(ewsclient.GetMailboxInfo().InboxUri);
            }
            return ewsclient.ListMessages(ewsclient.GetMailboxInfo(((EWS)clientConfig).emailAdress).InboxUri);
        }

        public ImapMessageInfoCollection GetIMAPEmails()
        {
            if (instance == null || type == null || imapclient == null || imapclient.ConnectionState != ConnectionState.Open || clientConfig == null) return new();
            if(clientConfig.folder == "")
            {
                return imapclient.ListMessages();
            }
            return imapclient.ListMessages(clientConfig.folder);
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
                        if (!msginfo.From.Address.ToLower().Contains("noreply@wetransfer.com")) continue;
                        msgs.Add(msginfo);
                    }
                    return msgs;
                case EmailType.EWS:
                    foreach (ExchangeMessageInfo msginfo in GetEWSEmails())
                    {
                        if (!msginfo.From.Address.ToLower().Contains("noreply@wetransfer.com")) continue;
                        msgs.Add(msginfo);
                    }
                    return msgs;
            }
            return msgs;
        }

        public string GetWeTransferDownloadLink(MessageInfoBase msginfo)
        {
            MailMessage? mail = type switch
            {
                EmailType.IMAP => imapclient?.FetchMessage(((ImapMessageInfo)msginfo).SequenceNumber),
                EmailType.EWS => ewsclient?.FetchMessage(((ExchangeMessageInfo)msginfo).UniqueUri),
                _ => null
            };
            if (mail == null || !mail.HtmlBody.Contains("wetransfer.com/downloads/"))
            {
                return "";
            }
            foreach (string brakeEmail in mail.HtmlBody.Split("<a href="))
            {
                if (brakeEmail.Contains("wetransfer.com/downloads/"))
                {
                    return brakeEmail.Split("\"")[1];
                }
            }
            return "";
        }

        public string GetUniqueID(MessageInfoBase msginfo)
        {
            return type switch
            {
                EmailType.IMAP => ((ImapMessageInfo)msginfo).SequenceNumber.ToString(),
                EmailType.EWS => ((ExchangeMessageInfo)msginfo).UniqueUri,
                _ => msginfo.MessageId.ToString()
            };
        }

        public async Task<List<string>> DownloadWeTransfer(DownloadHandler downloader)
        {
            List<MessageInfoBase> msgs = GetWeTransferMails();
            List<string> result = new();
            if (msgs == null || msgs.Count < 1) return result;
            foreach (MessageInfoBase msginfo in msgs)
            {
                try
                {
                    string downloadURL = GetWeTransferDownloadLink(msginfo);
                    if(downloadURL == "") continue;
                    await downloader.DownloadWeTransfer(downloadURL);
                    result.Add(GetUniqueID(msginfo));
                }
                catch (Exception) { }
            }
            return result;
        }

        public void Delete(MessageInfoBase messageInfo)
        {
            dynamic? client = GetClient();
            if (client is ImapClient client1)
            {
                client1.DeleteMessage(((ImapMessageInfo)messageInfo).SequenceNumber);
            }
            else if (client is IEWSClient client2)
            {
                client2.MoveItem(((ExchangeMessageInfo)messageInfo).UniqueUri, client2.GetMailboxInfo().DeletedItemsUri);
            }
        }

        public bool Send(MailMessage message)
        {
            if (GetClient() is IEWSClient client2)
            {
                try
                {
                    client2.Send(message);
                    return true;
                }
                catch (Exception) {}
            }
            return false;
        }

        public MailMessage? GetMail(MessageInfoBase messageInfo)
        {
            return type switch
            {
                EmailType.IMAP => imapclient?.FetchMessage(((ImapMessageInfo)messageInfo).SequenceNumber),
                EmailType.EWS => ewsclient?.FetchMessage(((ExchangeMessageInfo)messageInfo).UniqueUri),
                _ => null,
            };
        }
    }
}
