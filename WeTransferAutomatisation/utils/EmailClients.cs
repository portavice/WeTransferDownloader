using Aspose.Email.Clients;

namespace WeTransferDownloader.Utils.EmailClients
{
    public abstract class EmailClients
    {
        public string host = "127.0.0.1";
        public string username = "root";
        public string password = "";
        public string folder = "inbox";

        public EmailClients(string host, string username, string password, string folder = "inbox")
        {
            this.host = host;
            this.username = username;
            this.password = password;
            this.folder = folder;
        }
    }
    public class IMAP: EmailClients
    {
        public int port = 143;
        public SecurityOptions securityOptions = SecurityOptions.None;

        public IMAP(string host, string username, string password, string folder = "inbox"): base(host, username, password, folder)
        {
        }

        public IMAP(string host, int port, string username, string password, SecurityOptions securityOptions = SecurityOptions.None, string folder = "inbox") : base(host, username, password, folder)
        {
            this.port = port;
            this.securityOptions = securityOptions;
        }
    }

    public class EWS : EmailClients
    {
        public string domain = "";

        public EWS(string host, string domain, string username, string password, string folder = "inbox") : base(host, username, password, folder)
        {
            this.domain = domain;
        }
    }
}
