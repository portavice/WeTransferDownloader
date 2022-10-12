# WeTransfer Automatisation

Der WeTransfer-Downloader kümmert sich vollautomatisch um ihre empfangenen WeTransfers.
Das Tool liest voll automatisch ein Emailpostfach aus und nimmt sich alle WeTransfer-Emails.
Aus denen wird der Download-Link ausgelesen und die Daten herunter geladen.

Diese Version des WeTransfer-Downloaders ist zur Nutzung als Nuget-Packet erstellt.
Die Emails können über EWS- und IMAP-Verbindungen abgerufen werden.
Die Konfiguration erfolgt hier im Mailhandler.
Der WeTransfer-Downloader kann als Standalone Applikation genutzt werden.
In dem Packet sind Treiber für die Browser Edge/ Firefox/ Chrome installiert.


## Usage
Create a EmailHandler choose between IMAP and EWS
```
EmailHandler emailHandler = new EmailHandler(new IMAP("host", 143 /*port*/, "username", "password"));
EmailHandler emailHandler = new EmailHandler(new EWS("host", "domain", "username", "password"));
```
Create a DownloadHandler choose between Chrome, Firefox and Edge
```
DownloadHandler downloadHandler = new DownloadHandler(new Chrome("downloadpath"));
DownloadHandler downloadHandler = new DownloadHandler(new Firefox("downloadpath"));
DownloadHandler downloadHandler = new DownloadHandler(new Edge("downloadpath"));
```
Start the Wetransfer Downloader with
```
emailHandler.DownloadWeTransfer(downloadhandler);
```
