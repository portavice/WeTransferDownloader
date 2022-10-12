namespace WeTransferDownloader.Utils.BrowserClients
{
    public class Chrome
    {
        public string DownloadPath = "";
        public bool Silent = false;
        public Chrome(string DownloadPath = "", bool silent = false)
        {
            this.DownloadPath = DownloadPath;
            Silent = silent;
        }
    }

    public class Firefox
    {
        public string DownloadPath = "";
        public bool Silent = false;
        public Firefox(string DownloadPath = "", bool silent = false)
        {
            this.DownloadPath = DownloadPath;
            Silent = silent;
        }
    }

    public class Edge
    {
        public string DownloadPath = "";
        public bool Silent = false;
        public Edge(string DownloadPath = "", bool silent = false)
        {
            this.DownloadPath = DownloadPath;
            Silent = silent;
        }
    }
}
