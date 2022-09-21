using Aspose.Email.Clients;

namespace WeTransferDownloader.Utils
{
    internal class IMAP
    {
        public string host = "127.0.0.1";
        public int port = 143;
        public string username = "root";
        public string password = "";
        public SecurityOptions securityOptions;

        public string folder = "inbox";
    }

    internal class EWS
    {
        public string host = "127.0.0.1";
        public string domain = "";
        public string username = "root";
        public string password = "";
    }
}
