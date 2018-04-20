using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public partial class AdvancedFindForm : Form
    {
        public class SearchInfo
        {
            //private FileExplorerWindow.UrlLocation baseAddress;
            private string url;
            private string filter;
            private bool recursive;

            public SearchInfo(/*FileExplorerWindow.UrlLocation baseAddress, */string url, string filter, bool recursive)
            {
                //this.baseAddress = baseAddress;
                this.url = url;
                this.filter = filter;
                this.recursive = recursive;
            }

            //public FileExplorerWindow.UrlLocation BaseAddress { get { return baseAddress; } }

            public string Url { get { return url; } }

            public string Filter { get { return filter; } }

            public bool Recursive { get { return recursive; } }

            public override string ToString()
            {
                string type = (recursive) ? FilterInfo.SEARCH_STRING : FilterInfo.FIND_STRING;

                return (!string.IsNullOrWhiteSpace(filter) ? type + url + (!url.EndsWith("/") ? "/" : null) + filter : url + (!url.EndsWith("/") ? "/" : null));
            }
        }

        public SearchInfo GetSearchInfo()
        {
            return new SearchInfo(/*baseAddress, */textBox1.Text, textBox2.Text, checkBox1.Checked);
        }

        //private readonly FileExplorerWindow.UrlLocation baseAddress;

        public AdvancedFindForm(FileExplorerWindow.LinkInfo linkInfo)
        {
            InitializeComponent();

            if (linkInfo == null)
            {
                MessageBox.Show("Link Information Cannot be Empty...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            //baseAddress = linkInfo.BaseInfo;

            if (linkInfo.FileExt == "Directory")
            {
                textBox1.Text = linkInfo.Url;
                textBox2.Text = ".*";
            }
            else
            {
                textBox1.Text = linkInfo.ParentAddress;
                textBox2.Text = linkInfo.Name;
            }
        }
    }
}
