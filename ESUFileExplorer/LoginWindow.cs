using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public partial class LoginWindow : Form
    {
        public readonly string CERT = ESUFileExplorer.Properties.Settings.Default.CERT;

        public LoginWindow(KeyValuePair<string, string> user)
        {
            InitializeComponent();

            System.Net.ServicePointManager.ServerCertificateValidationCallback
                            = ((sender, certificate, chain, errors) =>
                            certificate != null && certificate.Subject.Contains(CERT, StringComparison.OrdinalIgnoreCase));

            pictureBox1.LoadAsync("http://www4.esu.edu/images/esu_logo_large.png");

            if (!user.Equals(default(KeyValuePair<string, string>)))
            {
                textBox2.Text = user.Key;
                textBox1.Text = user.Value;

                //textBox2.ReadOnly = true;
                //textBox1.ReadOnly = true;
            }

            // int index = 0;

            var urlLocations = FileExplorerWindow.GetLocations();

            if (urlLocations.Count <= 0)
            {
                MessageBox.Show("Found No Valid Locations...", "No Locations");
                this.Close();
                return;
            }

            urlLocations.Sort((a, b) => a.Index.CompareTo(b.Index));

            comboBox1.Items.AddRange(urlLocations.ToArray());

            comboBox1.SelectedIndex = 0;

            //textBox2.Text = "tthornton1";
            //textBox1.Text = "Tst12368";

            // button1.PerformClick();
        }

        private KeyValuePair<string, string> user;

        public KeyValuePair<string, string> User { get { return user; } }

        private FileExplorerWindow.UrlLocation url;

        public FileExplorerWindow.UrlLocation Url { get { return url; } }


        private void button1_Click(object sender, EventArgs e)
        {
            //AddBookmarkWindow bookmarkWindow = new AddBookmarkWindow();
            //bookmarkWindow.Show();

            //return;
            user = new KeyValuePair<string, string>(textBox2.Text, textBox1.Text);
            url = ((FileExplorerWindow.UrlLocation)comboBox1.SelectedItem);
            // url = new KeyValuePair<string, string>(((UrlLocation)comboBox1.SelectedItem).BaseAddress, String.Format(((UrlLocation)comboBox1.SelectedItem).Url, textBox2.Text));

            using (HttpClient client = FileExplorerWindow.CreateHttpClient(url.BaseAddress, user))
                if (FileExplorerWindow.CheckIfExists(new FileExplorerWindow.HttpInfo(client, new FileExplorerWindow.LinkInfo(String.Format(url.Url, textBox2.Text), url, url.FolderName, "Directory", null, "Directory"), user)))
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username and/or Password Were Incorrect.\nPlease Try Again...", "Invalid Login");

                }
        }
    }
}
