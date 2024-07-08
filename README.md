# WeTransfer Automatisation

The WeTransfer downloader takes care of your received WeTransfers fully automatically.
The tool reads an email inbox fully automatically and takes all WeTransfer emails.
The download link is read from these and the data is downloaded.

This version of the WeTransfer downloader is designed for use as a Nuget package.
The emails can be retrieved via EWS | IMAP | Graph | Pop3 connections.
The configuration is done here in the mail handler.
The WeTransfer-Downloader can be used as a standalone application.
Drivers for the Edge | Firefox | Chrome browsers are installed in the package.

## Usage

Create a EmailHandler choose between EWS | IMAP | Graph | Pop3

```
EmailHandler emailHandler = new EmailHandler(new Imap("host", 143 /*port*/, "username", "password"));
EmailHandler emailHandler = new EmailHandler(new EWS("host", "domain", "username", "password"));
EmailHandler emailHandler = new EmailHandler(new Graph("clientId", "tenantId"));
EmailHandler emailHandler = new EmailHandler(new Pop("host", 110 /*port*/, "username", "password"));
```

Create a DownloadHandler choose between Chrome, Firefox and Edge

```
DownloadHandler downloadHandler = new DownloadHandler(new Chrome("downloadpath", true /*headless*/));
DownloadHandler downloadHandler = new DownloadHandler(new Firefox("downloadpath", true /*headless*/));
DownloadHandler downloadHandler = new DownloadHandler(new Edge("downloadpath", true /*headless*/));
```

Start the Wetransfer Downloader with

```
emailHandler.DownloadWeTransfer(downloadhandler);
```

Please note that the DownloadWeTransfer() runs asynchronously and therefore does not wait for the download.
