namespace WeTransferDownloader.Utils.BrowserClients
{
    public abstract class BrowserClients
    {
        public string DownloadPath = "";
        public bool Silent = false;

        public BrowserClients(string downloadPath, bool silent)
        {
            DownloadPath = downloadPath;
            Silent = silent;
        }
    }
    public class Chrome : BrowserClients
    {
        public Chrome(string downloadPath, bool silent) : base(downloadPath, silent)
        {
        }
    }

    public class Firefox : BrowserClients
    {
        public Firefox(string downloadPath, bool silent) : base(downloadPath, silent)
        {
        }
    }

    public class Edge : BrowserClients
    {
        public Edge(string downloadPath, bool silent) : base(downloadPath, silent)
        {
        }
    }
}
