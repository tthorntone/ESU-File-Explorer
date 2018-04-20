using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public partial class FileDownloadForm : Form
    {
        public FileDownloadForm(WebClient client, FileExplorerWindow.LinkInfo linkInfo)
        {
            InitializeComponent();

            textBox1.AppendText("Downloading: " + linkInfo.Name);

            if(linkInfo.Size != null)
                textBox1.AppendText(Environment.NewLine
                + "\t(Size: " + linkInfo.Size + ")");


            //WebClient webClient = new WebClient();
            client.DownloadProgressChanged += (s, e) =>
            {
                progressBar1.Value = e.ProgressPercentage;
            };

            client.DownloadFileCompleted += (s, e) =>
            {
                progressBar1.Visible = false;

                if (e.Error != null)
                {
                    textBox2.Text = "ERROR: " + e.Error.Message.Replace("The remote server returned an error: ", "");
                    textBox2.Visible = true;
                } else
                {
                    this.Close();
                }

                
                // any other code to process the file
            };
        }
    }
}
