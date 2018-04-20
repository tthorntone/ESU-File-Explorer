using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public partial class DownloadWindow : Form
    {
        //private static List<DownloadWindow> instances;
        private HttpClient httpClient;
        private List<WebClient> clients = new List<WebClient>();
        private String downloadLocation;
        int downloadCount = 0;
        KeyValuePair<string, string> user;

        /*
        public static DownloadWindow Register(WebClient client, String downloadLocation)
        {
            DownloadWindow downloadWindow;

            if (instances == null)
            {
                downloadWindow = new DownloadWindow(client, downloadLocation);
                instances = new List<DownloadWindow>();
                instances.Add(downloadWindow);
                return downloadWindow;
            }
            else if ((downloadWindow = instances.Find(c => c.client == client && c.downloadLocation == downloadLocation)) != null)
                return downloadWindow;
            else
            {
                downloadWindow = new DownloadWindow(client, downloadLocation);
                instances.Add(downloadWindow);
                return downloadWindow;
            }   
        }
        */

        public void Download(/*TaggedWebClient client, */FileExplorerWindow.LinkInfo linkInfo, CancellationTokenSource cts, bool open)
        {
            string path = System.Web.HttpUtility.UrlDecode((Path.Combine(downloadLocation, linkInfo.Url.Substring(1).Replace("/", "\\"))));
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            if (linkInfo.Size != null && linkInfo.Size.Equals("Directory"))
            {
                List<FileExplorerWindow.LinkInfo> directoryFiles = FileExplorerWindow.GetDirectoryFiles((new FileExplorerWindow.HttpInfo(httpClient, linkInfo, user, cts)));

                foreach(FileExplorerWindow.LinkInfo newLinkInfo in directoryFiles)
                    Download(/*FileExplorerWindow.CreateClient(linkInfo.BaseAddress, user), */newLinkInfo, cts, false);
            }
            else
            {
                TaggedWebClient client = FileExplorerWindow.CreateClient(linkInfo.BaseInfo.BaseAddress, user);

                clients.Add(client);
                FileDownloadForm Form2 = new FileDownloadForm(client, linkInfo);
                //client.Tag = Form2;
                client.Tag = path;
                Form2.MdiParent = this;

                flowLayoutPanel1.Controls.Add(Form2);
                Form2.Show();

                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadFileAsync(new Uri(linkInfo.FullAddress), path, open);

                downloadCount++;
                this.Show();
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                downloadCount--;

                TaggedWebClient webClient = (TaggedWebClient)sender;

                clients.Remove(webClient);

                if((bool)e.UserState)
                {
                    Process.Start((string)webClient.Tag);
                }
            }
           // tableLayoutPanel1.resi
           // tableLayoutPanel1.Controls.Remove((FileDownloadForm)webClient.Tag);

            if (downloadCount <= 0)
                this.Close();
        }

        public DownloadWindow(HttpClient client, String downloadLocation, KeyValuePair<string, string> user)
        {
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;


            this.IsMdiContainer = true;
            this.httpClient = client;
            this.downloadLocation = downloadLocation;
            this.user = user;

            InitializeComponent();

            this.FormClosed += DownloadWindow_FormClosed;
        }

        private void DownloadWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            //instances.Remove(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(WebClient client in clients)
            {
                client.CancelAsync();
            }

            this.Close();
        }
    }
}
