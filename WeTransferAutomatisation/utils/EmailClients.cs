using Aspose.Email.Clients;

namespace WeTransferDownloader.Utils.EmailClients
{
    public class IMAP
    {
        public string host = "127.0.0.1";
        public int port = 143;
        public string username = "root";
        public string password = "";
        public SecurityOptions securityOptions = SecurityOptions.None;
        public string folder = "inbox";

        public IMAP(string host, string username, string password)
        {
            this.host = host;
            this.username = username;
            this.password = password;
        }

        public IMAP(string host, int port, string username, string password, SecurityOptions securityOptions = SecurityOptions.None, string folder = "inbox")
        {
            this.host = host;
            this.port = port;
            this.username = username;
            this.password = password;
            this.securityOptions = securityOptions;
            this.folder = folder;
        }
    }

    public class EWS
    {
        public string host = "127.0.0.1";
        public string domain = "";
        public string username = "root";
        public string password = "";

        public EWS(string host, string domain, string username, string password)
        {
            this.host = host;
            this.domain = domain;
            this.username = username;
            this.password = password;
        }
    }
}
