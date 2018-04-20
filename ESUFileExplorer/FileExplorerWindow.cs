using OC.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using System.Reflection;
using System.Collections;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Threading;
using System.Text.RegularExpressions;

namespace ESUFileExplorer
{
    public partial class FileExplorerWindow : Form
    {
        //private KeyValuePair<string, string> user;
        //private CancellationTokenSource cts;
        //public readonly HttpClient httpClient;




        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);

        public static List<FileExplorerWindow.UrlLocation> GetLocations()
        {
            List<UrlLocation> urlLocations = new List<UrlLocation>();
            foreach (string location in Properties.Settings.Default.LOCATIONS)
            {
                string[] data = location.Split(';');

                if (data.Count() < 4 || data.Count() > 4)
                {
                    MessageBox.Show(string.Format("Could Not Parse {0}...", location), "Invalid Location");
                    continue;
                }

                urlLocations.Add(new UrlLocation(data[0], int.Parse(data[1]), data[2], data[3]));
            }

            return urlLocations;
        }

        public void Settings_Save_Locations(List<UrlLocation> dataitems, bool save)
        {

            if (save)
            {
                Properties.Settings.Default.LOCATIONS.Clear();
                foreach (UrlLocation url in dataitems)
                {
                    Properties.Settings.Default.LOCATIONS.Add(url.Parse());
                }

                Properties.Settings.Default.Save();
            }

            /*
            this.SearchEngine_ComboBox.Items.Clear();
            SetImages(dataitems);
            SearchEngine_ComboBox.Items.AddRange(dataitems.ToArray());

            if (save) deskOps.SaveSearchProviders(dataitems);

            setComboBox_Status();
            */
        }

        public class UrlLocation
        {
            private string name;
            private int index;
            private string baseAddress;
            private string url;

            private const int BaseAddressIndex = 2;
            private const int UrlIndex = 3;

            public UrlLocation()
            {
                index = -1;
            }

            public UrlLocation(string name, int index, string baseAddress, string url)
            {
                this.name = name;
                this.index = index;
                this.baseAddress = baseAddress;
                //this.url = url;

                if (!url.EndsWith("/")) this.url = url + "/";
                else
                    this.url = url;
            }

            public UrlLocation(ListViewItem listviewitem)
            {
                name = listviewitem.Text;
                index = (int)listviewitem.Tag;
                baseAddress = listviewitem.SubItems[BaseAddressIndex].Text;
                url = listviewitem.SubItems[UrlIndex].Text;

                if (!url.EndsWith("/")) url = url + "/";
            }

            public string Name { get { return name; } }
            public int Index { get { return index; } }
            public string BaseAddress { get { return baseAddress; } }
            public string Url { get { return url; } }

            public string FolderName
            {
                get
                {

                    string[] split = url.Split('/');


                    return split[split.Length - 2];
                }
            }

            public string Parse()
            {
                return name + ";" + index + ";" + baseAddress + ";" + url;
            }

            public override string ToString()
            {
                return string.Format("{0} ({1})", name, baseAddress);
            }
        }

        public class LinkInfo
        {
            string url;
            //string baseAddress;
            UrlLocation baseInfo;
            string name;
            string fileExt;
            DateTime? date;
            string size;

            string filter;
            FilterType filterType;

            public LinkInfo(LinkInfo linkInfo)
            {
                if (linkInfo == null)
                    return;

                this.url = linkInfo.url;
                this.baseInfo = linkInfo.baseInfo;
                this.name = linkInfo.name;
                this.fileExt = linkInfo.fileExt;
                this.date = linkInfo.date;
                this.size = linkInfo.size;
                this.filter = linkInfo.filter;
                this.filterType = linkInfo.filterType;
            }

            public LinkInfo(string url, UrlLocation baseInfo, /*string baseAddress,*/ string name, string fileExt, DateTime? date, string size, string filter = null, FilterType filterType = FilterType.NONE)
            {
                this.url = url;
                this.baseInfo = baseInfo;
                this.name = name;
                this.fileExt = fileExt;
                this.date = date;
                this.size = size;
                this.filter = filter;
                this.filterType = filterType;
            }

            public string Url { get { return url; } }
            public UrlLocation BaseInfo { get { return baseInfo; } }
            public string FullAddress { get { return new Uri(new Uri(baseInfo.BaseAddress), url).AbsoluteUri; } }

            public string ParentAddress
            {
                get
                {
                    string[] uri = url.Split('/');

                    string parsedUrl = "/" + string.Join("/", uri.Skip(1).Take(uri.Length - (url.EndsWith("/") ? 3 : 2)).Select(x => x.ToString()).ToArray()) + "/";

                    return parsedUrl;
                }
            }

            public string Name { get { return name; } }
            public string FileExt { get { return fileExt; } }
            public DateTime? Date { get { return date; } }
            public string Size { get { return size; } set { size = value; } }

            public string Filter
            {
                get { return filter; }
                set { filter = value; }
            }

            public FilterType FilterType
            {
                get { return filterType; }
                set { filterType = value; }
            }

            public override string ToString()
            {
                return ((!string.IsNullOrWhiteSpace(filter) && filterType != FilterType.NONE) ? filterType.ToDescriptionString() + url + filter : url);
            }

            public override bool Equals(object obj)
            {
                LinkInfo linkInfo = obj as LinkInfo;

                if (linkInfo == null)
                    return false;

                return linkInfo.url == this.url && linkInfo.baseInfo == this.baseInfo && linkInfo.filterType == this.filterType && linkInfo.filter == this.filter;
            }


            public override int GetHashCode()
            {
                int hash = url.GetHashCode() ^ baseInfo.GetHashCode() ^ filterType.GetHashCode();
                if (filter != null) hash ^= filter.GetHashCode();

                return hash;
            }
        }

        //TaggedWebClient client;

        public class HttpInfo
        {
            public HttpClient httpClient;
            public CancellationTokenSource cts;
            public LinkInfo linkInfo;
            public KeyValuePair<string, string> user;

            public HttpInfo(HttpClient httpClient, LinkInfo linkInfo, KeyValuePair<string, string> user, CancellationTokenSource cancellationTokenSource = null)
            {
                this.httpClient = httpClient;
                this.cts = cancellationTokenSource;
                this.linkInfo = linkInfo;
                this.user = user;
            }
        }

        public static List<LinkInfo> GetDirectoryFiles(HttpInfo httpInfo, BackgroundWorker bw = null, ListView listView = null, MEFHelper mefHelper = null/*BackgroundWorker bw = null*/)
        {
            List<LinkInfo> list = new List<LinkInfo>();

            if (httpInfo.linkInfo.FileExt != "Directory")
                return null;

            //TaggedWebClient client = CreateClient(linkInfo.BaseAddress, user);

            //Uri uri = new Uri(linkInfo.FullAddress);
            // UriBuilder uriBuilder = new UriBuilder(uri.Scheme, uri.Host, uri.Port, linkInfo.Url);

            //UriBuilder uriBuilder = new UriBuilder(new Uri(linkInfo.FullAddress));

            string hi;

            try
            {
                var response = httpInfo.httpClient.GetAsync(httpInfo.linkInfo.Url, (httpInfo.cts != null) ? httpInfo.cts.Token : CancellationToken.None).Result;
                hi = response.Content.ReadAsStringAsync().Result;
            }
            catch (WebException e)
            {
                return null;
            }
            catch (AggregateException e)
            {
                e.Handle((x) =>
                {
                    if (x is TaskCanceledException)
                    {
                        return true;
                    }

                    return false;
                });

                return null;
            }

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            //HtmlAgilityPack.HtmlNode.ElementsFlags.Remove("form");

            try
            {
                doc.LoadHtml(hi);
            }
            catch (Exception)
            {
                // UpdateWindowLog("Could Not Load Lesson List Page to Parse.\n", Color.Red);
                return null;
            }

            var workingNode = doc.DocumentNode.SelectSingleNode("html/body/pre");

            if (workingNode == null)
                return null;

            HtmlAgilityPack.HtmlNodeCollection nodeCollection = workingNode.SelectNodes("./a");

            if (nodeCollection == null || nodeCollection.Count <= 0)
            {
                return list;
            }
            else
            {
                if (bw != null && bw.CancellationPending)
                {
                    return null;
                }

                List<ListViewItem> itemList = new List<ListViewItem>();


                /*
                if (listView != null)listView.Invoke(new Action(() =>
                {
                    // listView.LargeImageList = imageList;
                   
                }));
                */


                string name;

                int i;


                if (httpInfo.linkInfo.FilterType == FilterType.SEARCH)
                {
                    i = listView.Items.Count;
                }
                else
                    i = 0;


                //bool matchJustFiles = (string.IsNullOrWhiteSpace(httpInfo.linkInfo.Filter)) ? false : Regex.IsMatch(httpInfo.linkInfo.Filter, @"^.*?((^|[^\\])\.)");

                foreach (HtmlAgilityPack.HtmlNode node in nodeCollection)
                {
                    if (bw != null && bw.CancellationPending)
                    {
                        return list;
                    }


                    HtmlAgilityPack.HtmlNode nodeInfo = node.SelectSingleNode(node.XPath + "/preceding-sibling::text()[1]");
                    if (nodeInfo == null)
                        continue; //should be parent, we can skip...

                    name = node.InnerText;
                    string nodeInfoString = nodeInfo.InnerText.Trim();
                    int timeIndex = nodeInfoString.IndexOf('M') + 1;
                    string date = nodeInfoString.Substring(0, timeIndex).Trim();
                    string size = nodeInfoString.Substring(timeIndex).Trim();

                    DateTime dateTime = DateTime.Parse(date);

                    if (size.Contains("dir"))
                        size = "Directory";

                    LinkInfo newLinkInfo;
                    if (!string.IsNullOrWhiteSpace(httpInfo.linkInfo.Filter) && (httpInfo.linkInfo.FilterType == FilterType.SEARCH || httpInfo.linkInfo.Filter != "*"))
                    {
                        bool match;

                        if (IsValidRegex(httpInfo.linkInfo.Filter))
                        {
                            match = Regex.IsMatch(name, httpInfo.linkInfo.Filter, RegexOptions.ExplicitCapture);
                            //match = regexMatch.Success;
                        }
                        else
                            match = Microsoft.VisualBasic.CompilerServices.Operators.LikeString(name, httpInfo.linkInfo.Filter, Microsoft.VisualBasic.CompareMethod.Text);

                        if (httpInfo.linkInfo.FilterType == FilterType.SEARCH)
                        {
                            if (size != "Directory" && !match) //not directory AND not match
                                continue;

                            if (size == "Directory"/* && !match*/) //directory
                            {
                                newLinkInfo = new LinkInfo(Uri.UnescapeDataString(node.Attributes["HREF"].Value), httpInfo.linkInfo.BaseInfo, Uri.UnescapeDataString(name), size.Equals("Directory") ? "Directory" : Path.GetExtension(node.InnerText), dateTime, size, httpInfo.linkInfo.Filter, FilterType.SEARCH);

                                list.Add(newLinkInfo);
                                if (!match) continue;
                            }
                        }
                        else
                        {
                            if (!match)
                            {
                                continue;
                            }

                            /*if (matchJustFiles && size == "Directory")
                            {
                                continue; //we are looking for files, skip directories!!!
                            }*/
                        }
                    }

                    newLinkInfo = new LinkInfo(Uri.UnescapeDataString(node.Attributes["HREF"].Value), httpInfo.linkInfo.BaseInfo, Uri.UnescapeDataString(name), size.Equals("Directory") ? "Directory" : Path.GetExtension(node.InnerText), dateTime, size/*, linkInfo.Filter, linkInfo.FilterType*/);

                    if (httpInfo.linkInfo.FilterType != FilterType.SEARCH)
                        list.Add(newLinkInfo);
                    //innerText == NAME
                    //attribute HREF == URL
                    //

                    if (listView != null)
                    {
                        // int i = startingIndex;

                        if (itemList.Count >= 50)
                        {
                            listView.Invoke(new Action(() =>
                            {
                                //Form1.AddLinkInfoToListView(listView, queueLinkInfoList, startingIndex);
                                listView.Items.AddRange(itemList.ToArray());
                            }));

                            itemList.Clear();
                            // imageList.Images.Clear();
                            // startingIndex += 50;
                        }
                        /*
                        else
                        {
                        */

                        // imageList.Add(imageLoader.GetImage(newLinkInfo.FileExt));



                        Image image = null;

                        if (listView != null && mefHelper != null && mefHelper.imageLoaderPlugin != null)
                        {
                            ImageData imageData;
                            imageData = new ImageData(i, newLinkInfo.FileExt.ToLower(), newLinkInfo.FullAddress, /*httpInfo.linkInfo.GetHashCode(),*/ httpInfo.httpClient, (httpInfo.cts != null) ? httpInfo.cts.Token : CancellationToken.None/*CreateClient(httpInfo.linkInfo.BaseAddress, httpInfo.user)*/, new Action<int, Image>((index, newImage) =>
                                                {
                                                    //if (bw.CancellationPending)
                                                    //    return;

                                                    listView.Invoke(new Action(() =>
                                                    {
                                                        /*if (httpInfo.linkInfo.GetHashCode() != ((LinkInfo)listView.Tag).GetHashCode())
                                                            return;*/

                                                        listView.BeginUpdate();

                                                        int newIndex = listView.LargeImageList.Images.Count;
                                                        listView.LargeImageList.Images.Add(index.ToString(), newImage);
                                                        listView.LargeImageList.Images[index] = listView.LargeImageList.Images[newIndex];
                                                        listView.LargeImageList.Images.RemoveAt(newIndex);

                                                        listView.EndUpdate();
                                                    }));

                                                    return;
                                                }));



                            image = mefHelper.imageLoaderPlugin.GetImage(imageData);
                        }
                        else
                            image = ESUFileExplorer.Properties.Images.empty;


                        if (bw != null && bw.CancellationPending)
                            return list;

                        listView.Invoke(new Action(() =>
                        {
                            if (bw == null || !bw.CancellationPending)
                                listView.LargeImageList.Images.Add(i.ToString(), image);
                        }));


                        ListViewItem lvi = new ListViewItem(newLinkInfo.Name);
                        lvi.SubItems.Add(newLinkInfo.Url);
                        lvi.SubItems.Add((newLinkInfo.Date != null) ? newLinkInfo.Date.ToString() : "           ");
                        lvi.SubItems.Add((newLinkInfo.Size != "Directory") ? newLinkInfo.Size : "               ");
                        lvi.ImageIndex = i;//.ToString();
                        lvi.Tag = newLinkInfo;
                        lvi.ToolTipText = newLinkInfo.Url;
                        if (newLinkInfo.Size != null && newLinkInfo.Size != "Directory") lvi.ToolTipText += "\n" + "Size: " + SizeSuffixTools.SizeSuffix(long.Parse(newLinkInfo.Size));
                        itemList.Add(lvi);
                        i++;
                        //}

                    }
                }


                if (listView != null)
                {

                    listView.Invoke(new Action(() =>
                    {
                        //Form1.AddLinkInfoToListView(listView, queueLinkInfoList, startingIndex);
                        // listView.LargeImageList.Images.AddRange(tempListView.LargeImageList.Images.Cast<Image>().ToArray());
                        if (itemList.Count > 0)
                        {
                            listView.Items.AddRange(itemList.ToArray());
                        }

                        //listView.LargeImageList = imageList;
                    }));

                    itemList.Clear();
                    //tempListView.LargeImageList.Images.Clear();
                }
            }

            return list;
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.IsMatch("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static TaggedWebClient CreateClient(string baseAddress, KeyValuePair<string, string> user)
        {
            TaggedWebClient client = new TaggedWebClient();
            client.BaseAddress = baseAddress;
            NetworkCredential cred = new NetworkCredential(user.Key, user.Value);
            client.Credentials = cred;

            return client;
        }

        public static HttpClient CreateHttpClient(string baseAddress, KeyValuePair<string, string> user)
        {
            HttpClient client;

            NetworkCredential cred = new NetworkCredential(user.Key, user.Value);

            HttpClientHandler handler = new HttpClientHandler() { Credentials = cred };

            client = new HttpClient(handler) { BaseAddress = new Uri(baseAddress) };

            return client;
        }

        private void InitiateWindow(LinkInfo linkInfo)
        {
            // string path = new Uri(new Uri(baseURL), folderURL).AbsolutePath;

            this.listView1.Items.Clear();
            this.listView1.LargeImageList.Images.Clear();
            //this.listView1.Sorting = SortOrder.None;

            this.closeToolStripButton.Enabled = true;

            if (history.CurrentItem == null || !history.CurrentItem.Equals(linkInfo))
                history.CurrentItem = linkInfo;

            toolStripTextBox1.Text = linkInfo.ToString();
            toolStripTextBox1.Tag = linkInfo;

            // ImageList ilist = new ImageList();

            listView1.Tag = linkInfo;
            //ImageData.currentHashCode = linkInfo.GetHashCode();
            //if (mefHelper != null && mefHelper.appStatus != null) mefHelper.appStatus.HashCode = linkInfo.GetHashCode(); REMOVED: 9/23/2016
            //if (homeInfo.cts != null)
            //    homeInfo.cts.Cancel();

            CancelBackgroundOperations();

            homeInfo.cts = new CancellationTokenSource();

            if (backgroundWorker1.IsBusy)
            {
                //backgroundWorker1.CancelAsync();


                RunWorkerCompletedEventHandler runWorkerCompleted = null;           
                runWorkerCompleted = new RunWorkerCompletedEventHandler((sender, e) =>
                {
                    backgroundWorker1.RunWorkerCompleted -= runWorkerCompleted;
                    SetFolder(linkInfo);
                });

                backgroundWorker1.RunWorkerCompleted += runWorkerCompleted;
            }
            else
                backgroundWorker1.RunWorkerAsync(linkInfo);

            // if (toolStripButton1.Enabled != true)
            //     toolStripButton1.Enabled = true;
        }

        private void SearchFile(LinkInfo linkInfo)
        {
            InitiateWindow(linkInfo);
        }

        private void SetFolder(LinkInfo linkInfo)
        {
            InitiateWindow(linkInfo);
        }

        /*
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerCompleted -= BackgroundWorker1_RunWorkerCompleted;

            SetFolder(new LinkInfo(toolStripTextBox1.Text, ((LinkInfo)(toolStripTextBox1.Tag)).BaseInfo, Path.GetFileName(toolStripTextBox1.Text), "Directory", null, "Directory"));
        }
        */

        Button acceptButton = new Button(); //We can change this to an official button if we ever need it

        private void textBox_Enter(object sender, EventArgs e)
        {
            AcceptButton = acceptButton; // Button1 will be 'clicked' when user presses return
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            AcceptButton = null; // remove "return" button behavior

        }

        private void CreateResources()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //path = Path.Combine(path, "Resources");

            // if (!Directory.Exists(path))
            //    Directory.CreateDirectory(path);

            if (!File.Exists(Path.Combine(path, "Interface.dll")))
            {
                File.WriteAllBytes(Path.Combine(path, "Interface.dll"), ESUFileExplorer.Properties.DLLs.Interface);
                //File.SetAttributes(Path.Combine(path, "Interface.dll"), FileAttributes.Hidden);
            }

            if (!File.GetAttributes(Path.Combine(path, "Interface.dll")).HasFlag(FileAttributes.Hidden))
                File.SetAttributes(Path.Combine(path, "Interface.dll"), File.GetAttributes(Path.Combine(path, "Interface.dll")) | FileAttributes.Hidden);

            if (!File.Exists(Path.Combine(path, "HtmlAgilityPack.dll")))
            {
                File.WriteAllBytes(Path.Combine(path, "HtmlAgilityPack.dll"), ESUFileExplorer.Properties.DLLs.HtmlAgilityPack);
            }

            if (!File.GetAttributes(Path.Combine(path, "HtmlAgilityPack.dll")).HasFlag(FileAttributes.Hidden))
                File.SetAttributes(Path.Combine(path, "HtmlAgilityPack.dll"), File.GetAttributes(Path.Combine(path, "HtmlAgilityPack.dll")) | FileAttributes.Hidden);

            //SetDllDirectory(path);
            //Assembly.LoadFrom(Path.Combine(path, "Interface.dll"));

            if (AppExtensions.FileCount(path, "*.dll") <= 2)
            {
                if (!File.Exists(Path.Combine(path, "ImageLoader.dll")))
                {
                    File.WriteAllBytes(Path.Combine(path, "ImageLoader.dll"), ESUFileExplorer.Properties.DLLs.ImageLoader);
                    File.SetAttributes(Path.Combine(path, "ImageLoader.dll"), File.GetAttributes(Path.Combine(path, "ImageLoader.dll")) | FileAttributes.Hidden);
                    //File.SetAttributes(Path.Combine(path, "ImageLoader.dll"), FileAttributes.Hidden);
                }
            }


            //System.Reflection.Assembly.LoadFrom(Path.Combine(path, "Interface.dll"));

            mefHelper = new MEFHelper(path);
        }

        private void RegisterEvents()
        {
            acceptButton.Click += Button1_Click;

            toolStripTextBox1.Enter += textBox_Enter;
            toolStripTextBox1.Leave += textBox_Leave;
            toolStripTextBox1.KeyUp += ToolStripTextBox1_KeyUp;

            this.Load += FileExplorerWindow_Load;
            this.FormClosing += FileExplorerWindow_FormClosing;

            this.listView1.ColumnClick += ListView1_ColumnClick;
            this.listView1.MouseDoubleClick += ListView1_MouseDoubleClick;
            // this.listView1.MouseClick += ListView1_MouseClick;
            this.listView1.MouseUp += ListView1_MouseUp;
            //this.listView1.Tag = linkInfo.BaseAddress;

            openToolStripMenuItem1.Click += openToolStripMenuItem_Click;
            downloadToolStripMenuItem1.Click += downloadToolStripMenuItem_Click;

            history.GotoItem += new EventHandler<HistoryEventArgs<LinkInfo>>(history_GotoItem);
        }


        private void CheckDirectories(string bookMarkPath)
        {
            Directory.CreateDirectory(Path.Combine(System.IO.Path.GetTempPath(), "ESUFileExplorer"));
            Directory.CreateDirectory(bookMarkPath);
        }

        private void FileExplorerWindow_Load(object sender, EventArgs e)
        {
            string bookMarkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Applicat‌​ionData), "Trailblazer101", "ESUFileExplorer", "Bookmarks", homeInfo.linkInfo.BaseInfo.Name);

            CheckDirectories(bookMarkPath);
            AddBookmarksToMenu(bookMarkPath);

            RegisterFolderWatcher(bookMarkPath);
        }

        private void RegisterFolderWatcher(string bookMarkPath)
        {
            bookmarksFileSystemWatcher.Path = bookMarkPath;
            bookmarksFileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                         | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            bookmarksFileSystemWatcher.Filter = "*.*";
            bookmarksFileSystemWatcher.IncludeSubdirectories = true;

            bookmarksFileSystemWatcher.Changed += Watcher_Changed;
            bookmarksFileSystemWatcher.Deleted += Watcher_Changed;
            bookmarksFileSystemWatcher.Created += Watcher_Changed;
            bookmarksFileSystemWatcher.Renamed += Watcher_Changed;

            bookmarksFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            AddBookmarksToMenu(bookmarksFileSystemWatcher.Path);
        }

        private void ListView1_MouseUp(object sender, MouseEventArgs e)
        {
            ListView1_MouseClick(sender, e);
        }

        private readonly HttpInfo homeInfo;

        public MEFHelper mefHelper;
        public FileExplorerWindow(LinkInfo linkInfo, KeyValuePair<string, string> user)
        {
            InitializeComponent();

            addBookmarkToolStripMenuItem = new ToolStripMenuItem("Add Bookmark");
            addBookmarkToolStripMenuItem.Image = Properties.Icons.create_folder.ToBitmap();
            addBookmarkToolStripMenuItem.Click += addBookmarkToolStripMenuItem_Click;

            separatorToolStripMenuItem = new ToolStripSeparator();

            //bookmarksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addBookmarkToolStripMenuItem, separatorToolStripMenuItem});

            homeInfo = new HttpInfo(CreateHttpClient(linkInfo.BaseInfo.BaseAddress, user), linkInfo, user);

            //this.user = user;
            //httpClient = CreateHttpClient(linkInfo.BaseAddress, user);

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(92, 92); //92, 92 seems to be the best so far!
            imageList.TransparentColor = Color.Black;
            imageList.ColorDepth = ColorDepth.Depth24Bit;
            listView1.LargeImageList = imageList;

            history = new History<LinkInfo>(backToolStripButton, forwardToolStripButton, 0);

            nameColumnHeader.Tag = nameToolStripMenuItem;
            dateColumnHeader.Tag = dateToolStripMenuItem;
            urlColumnHeader.Tag = urlToolStripMenuItem;
            sizeColumnHeader.Tag = sizeToolStripMenuItem;

            if (sortColumn == -1)
            {
                sortColumn = 0;
                listView1.Sorting = SortOrder.Ascending;

                listView1.Columns[0].Text = nameToolStripMenuItem.Tag + " ↑";
                nameToolStripMenuItem.Text = nameToolStripMenuItem.Tag + " ↑";
            }

            ToolStripItem[] list = new ToolStripItem[contextMenuStrip2.Items.Count];
            contextMenuStrip2.Items.CopyTo(list, 0);

            editToolStripDropDownButton.DropDownItems.AddRange(list);

            //contextMenuStrip2.Items.AddRange(list);
            contextMenuStrip2.Closed += ContextMenuStrip2_Closed;
            RegisterEvents();

            if (mefHelper == null)
            {
                CreateResources();
            }


            /*
            if(imageLoader == null)
            {
                MessageBox.Show("Could Not Load ImageLoader.dll, Can't Continue.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            */

            backgroundWorker1.DoWork += (o, args) =>
            {
                LinkInfo key = (LinkInfo)args.Argument;

                if (key.FilterType == FilterType.SEARCH)
                {
                    GetAllFiles(key);
                }
                else
                {
                    args.Result = GetDirectoryFiles(new HttpInfo(homeInfo.httpClient, key, user, homeInfo.cts), backgroundWorker1, listView1, mefHelper);
                    if (args.Result == null || backgroundWorker1.CancellationPending)
                        args.Cancel = true;
                }
            };

            backgroundWorker1.RunWorkerCompleted += (o, args) =>
            {
                if (!args.Cancelled)
                {
                    /*
                    if (listView1.Sorting == SortOrder.Ascending)
                    {
                        listView1.Columns[sortColumn].Text = listView1.Columns[sortColumn].Tag + " ↑";
                    }
                    else
                    {
                        listView1.Columns[sortColumn].Text = listView1.Columns[sortColumn].Tag + " ↓";
                    }
                    */

                    //listView1.ListViewItemSorter = new CustomEditorClass.ListViewItemComparer(sortColumn);
                }

                listView1.Sort();

                //listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.);
                closeToolStripButton.Enabled = false;
            };


            //  toolStrip1.Resize += toolStrip1_Resize;


            //Uri baseUri = new Uri(SHEMP_URL);

            //this.toolStripTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;


            // trust sender


            // TaggedWebClient client = CreateClient();

            //client.Login(SHEMP_URL, cred);


            SetFolder(linkInfo);

            // OC.Windows.Forms.Form3 form3 = new OC.Windows.Forms.Form3();
            // form3.ShowDialog();


            //client.DownloadFile("https://shemp.esu.edu/tthornton1/Documents/Taylore's Documents/Don.docx", "TEST.docx");
            // if (client.CookieContainer == null)
            //     return;
        }

        private void CancelBackgroundOperations()
        {
            if (homeInfo.cts != null)
                homeInfo.cts.Cancel();

            backgroundWorker1.CancelAsync();
        }


        private void addBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBookmark();
        }

        private void ContextMenuStrip2_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ToolStripItem[] list = new ToolStripItem[((ContextMenuStrip)sender).Items.Count];
            ((ContextMenuStrip)sender).Items.CopyTo(list, 0);

            editToolStripDropDownButton.DropDownItems.AddRange(list);
        }

        private void FileExplorerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //mefHelper.appStatus.HashCode = -1; REMOVED: 9/23/2016
            CancelBackgroundOperations();
        }

        private void SetSort(int columnIndex, ToolStripMenuItem menuItem)
        {
            foreach (ColumnHeader header in listView1.Columns)
            {
                header.Text = (string)((ToolStripMenuItem)(header.Tag)).Tag;
                ((ToolStripMenuItem)(header.Tag)).Text = (string)((ToolStripMenuItem)(header.Tag)).Tag;
                ((ToolStripMenuItem)(header.Tag)).Checked = false;
            }

            // Determine whether the column is the same as the last column clicked.
            if (columnIndex != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = columnIndex;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
                listView1.Columns[columnIndex].Text = menuItem.Tag + " ↑";
                menuItem.Text = menuItem.Tag + " ↑";
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                    listView1.Columns[columnIndex].Text = menuItem.Tag + " ↓";
                    menuItem.Text = menuItem.Tag + " ↓";
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                    listView1.Columns[columnIndex].Text = menuItem.Tag + " ↑";
                    menuItem.Text = menuItem.Tag + " ↑";
                }
            }

            menuItem.Checked = true;

            // Call the sort method to manually sort.
            listView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(columnIndex,
                                                              listView1.Sorting, (listView1.Columns[columnIndex].Text.Contains("Date")));
        }

        private void ListView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //listView1.Columns[e.Column]

            SetSort(e.Column, (ToolStripMenuItem)listView1.Columns[e.Column].Tag);
        }

        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            bool dateTime;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order, bool dateTime = false)
            {
                col = column;
                this.order = order;
                this.dateTime = dateTime;
            }
            public int Compare(object x, object y)
            {
                int returnVal;
                // Determine whether the type being compared is a date type.

                if (dateTime)
                {
                    try
                    {
                        // Parse the two objects passed as a parameter as a DateTime.
                        System.DateTime firstDate =
                                DateTime.Parse(((ListViewItem)x).SubItems[col].Text);
                        System.DateTime secondDate =
                                DateTime.Parse(((ListViewItem)y).SubItems[col].Text);
                        // Compare the two dates.
                        returnVal = DateTime.Compare(firstDate, secondDate);
                    }
                    // If neither compared object has a valid date format, compare
                    // as a string.
                    catch
                    {
                        // Compare the two items as a string.
                        returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,

                                              ((ListViewItem)y).SubItems[col].Text);
                    }
                }
                else
                {
                    // Compare the two items as a string.
                    returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,

                                          ((ListViewItem)y).SubItems[col].Text);
                }

                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;

                return returnVal;
            }
        }

        private void GetAllFiles(LinkInfo linkInfo)
        {
            List<LinkInfo> files = GetDirectoryFiles(new HttpInfo(homeInfo.httpClient, linkInfo, homeInfo.user, homeInfo.cts), backgroundWorker1, listView1, mefHelper);

            if (files != null)
            {
                //allFiles.AddRange(files);

                foreach (LinkInfo file in files)
                {
                    GetAllFiles(file);
                }
            }
        }

        private void ToolStripTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.A: // all 
                        toolStripTextBox1.SelectAll();
                        break;
                }
            }
        }

        /*
        public static bool CheckIfExists(LinkInfo linkInfo, KeyValuePair<String, String> user)
        {
            //user = new KeyValuePair<string, string>(textBox2.Text, textBox1.Text);
            TaggedWebClient client = FileExplorerWindow.CreateClient(linkInfo.BaseAddress, user);

            try
            {
                using (System.IO.Stream stream = client.OpenRead(linkInfo.Url))
                {
                    // DialogResult = DialogResult.OK;
                    // this.Close();
                    //Application.Run(form1);
                    return true;
                }
            }
            catch (System.Net.WebException ex)
            {
                return false;
            }
        }*/

        public static bool CheckIfExists(HttpInfo httpInfo)
        {
            using (var message = new HttpRequestMessage(HttpMethod.Head, httpInfo.linkInfo.Url))
            //using (var client = CreateHttpClient(httpInfo.linkInfo.BaseAddress, httpInfo.user))
            {
                try
                {
                    var result = httpInfo.httpClient.SendAsync(message, (httpInfo.cts != null) ? httpInfo.cts.Token : CancellationToken.None).Result;

                    if (result.IsSuccessStatusCode)
                    {
                        if (httpInfo.linkInfo.Size == null)
                            httpInfo.linkInfo.Size = result.Content.Headers.ContentLength.Value.ToString();

                        return true;
                    }
                    else
                        return false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        private void CheckAddressString(LinkInfo previousLinkInfo, String url, FilterType filterType)
        {
            LinkInfo linkInfo = null;
            string[] uri = url.Split('/');

            if (url.Substring(filterType.ToDescriptionString().Length).StartsWith("/"))
            {
                string parsedUrl = "/" + string.Join("/", uri.Skip(1).Take(uri.Length - 2).Select(x => x.ToString()).ToArray());
                if (!parsedUrl.EndsWith("/"))
                    parsedUrl += "/";

                Uri parsedUri = new Uri(new Uri(previousLinkInfo.BaseInfo.BaseAddress), parsedUrl);

                if (previousLinkInfo.Url == null || !previousLinkInfo.Url.Equals(parsedUrl))
                {
                    linkInfo = new LinkInfo(parsedUrl, previousLinkInfo.BaseInfo, Uri.UnescapeDataString(parsedUri.Segments.Last().Substring(0, parsedUri.Segments.Last().Length - 1)), "Directory", null, "Directory");
                    //we cant skip check!

                    if (!CheckIfExists(new HttpInfo(homeInfo.httpClient, linkInfo, homeInfo.user)))
                    {
                        MessageBox.Show("Could Not Find Url.\nPlease Make Sure It Was Typed In Correctly, Then Try Again...", "Invalid Url");
                        return;
                    }
                }
                else
                    linkInfo = new LinkInfo(previousLinkInfo);

                linkInfo.Filter = uri.Last();

            }
            else
            {
                linkInfo = new LinkInfo(previousLinkInfo);
                linkInfo.Filter = uri[0].Substring(filterType.ToDescriptionString().Length);
            }

            linkInfo.FilterType = filterType;

            SetFolder(linkInfo);
        }


        private void CheckAddressBar(LinkInfo previousLinkInfo, String url)
        {
            // Uri uri = new Uri(new Uri(previousLinkInfo.BaseAddress), url);
            LinkInfo linkInfo = null;

            if (url.StartsWith(FilterInfo.SEARCH_STRING, StringComparison.OrdinalIgnoreCase))
            {
                CheckAddressString(previousLinkInfo, url, FilterType.SEARCH);
                return;
            }
            else
            if (url.StartsWith(FilterInfo.FIND_STRING, StringComparison.OrdinalIgnoreCase))
            {
                CheckAddressString(previousLinkInfo, url, FilterType.FIND);
                return;
            }

            string[] uri = url.Split('/');

            if (!url.EndsWith("/"))
            {
                linkInfo = new LinkInfo(url, previousLinkInfo.BaseInfo, uri.Last(), Path.GetExtension(url), null, null);
                if (CheckIfExists(new HttpInfo(homeInfo.httpClient, linkInfo, homeInfo.user)))
                {
                    List<LinkInfo> linkList = new List<LinkInfo>();
                    linkList.Add(linkInfo);

                    StartDownload(linkList);
                }
                else
                {
                    CheckAddressBar(previousLinkInfo, url + "/");
                }
            }
            else
            {
                linkInfo = new LinkInfo(url, previousLinkInfo.BaseInfo, uri[uri.Length - 2], "Directory", null, "Directory");

                if (CheckIfExists(new HttpInfo(homeInfo.httpClient, linkInfo, homeInfo.user)))
                {
                    SetFolder(linkInfo);
                }
                else
                {
                    MessageBox.Show("Could Not Find Url.\nPlease Make Sure It Was Typed In Correctly, Then Try Again...", "Invalid Url");
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.LostFocus -= ToolStripTextBox1_LostFocus;
            CheckAddressBar((LinkInfo)toolStripTextBox1.Tag, toolStripTextBox1.Text);
        }

        //public FileExplorerWindow() : this(new LinkInfo("/tthornton1/Documents/", SHEMP_URL, "Documents", null, DateTime.Now, "Directory"), new KeyValuePair<string, string>("tthornton1", "Tst12368")) { }

        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem != null && listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    // contextMenuStrip1.Items.Add(openToolStripMenuItem);
                    //  contextMenuStrip1.Items.Add(downloadToolStripMenuItem);
                    contextMenuStrip1.Show(Cursor.Position);
                }
                else
                {
                    if (contextMenuStrip2.Items.Count == 0)
                    {
                        ToolStripItem[] list = new ToolStripItem[editToolStripDropDownButton.DropDownItems.Count];
                        editToolStripDropDownButton.DropDownItems.CopyTo(list, 0);

                        contextMenuStrip2.Items.AddRange(list);

                        //contextMenuStrip2.Items.AddRange(list);
                    }

                    contextMenuStrip2.Show(Cursor.Position);
                }
            }
        }

        readonly History<LinkInfo> history;

        private void history_GotoItem(object sender, HistoryEventArgs<LinkInfo> e)
        {
            CancelBackgroundOperations();

            SetFolder(e.Item);
        }

        /*
        private TaggedWebClient Copy(TaggedWebClient orig)
        {
            TaggedWebClient copy = new TaggedWebClient();
            copy.BaseAddress = orig.BaseAddress;
            copy.Credentials = orig.Credentials;
            copy.Tag = orig.Tag;

            return copy;
        }
        */

        private void Download(List<LinkInfo> linkInfoList, string path, bool open)
        {
            DownloadWindow downloadWindow = new DownloadWindow(homeInfo.httpClient, path, homeInfo.user);

            foreach (LinkInfo linkInfo in linkInfoList)
            {
                //TaggedWebClient client = CreateClient(linkInfo.BaseAddress, user);

                downloadWindow.Download(/*client, */linkInfo, homeInfo.cts, open);
            };
        }

        private void StartDownload(List<LinkInfo> linkInfoList, bool open = false)
        {
            if (linkInfoList.Count >= 1)
            {
                string message;

                if (!open)
                    message = "Download";
                else
                    message = "Open";

                string all = linkInfoList.Select(i => i.Name).Aggregate((i, j) => i + "\n" + j);

                if (MessageBox.Show(string.Format("Are You Sure You Want to {0}:\n\n{1}", message, all), message, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    FolderBrowserDialog fbd = new FolderBrowserDialog();

                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        Download(linkInfoList, fbd.SelectedPath, open);
                    }
                }
            }
        }

        private void ListView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //ListView listView = (ListView)sender;

            ListView.SelectedListViewItemCollection collection = listView1.SelectedItems;

            if (collection.Count == 1 && ((LinkInfo)collection[0].Tag).Size.Equals("Directory"))
            {
                //String path = new Uri(new Uri(SHEMP_URL), ((LinkInfo)collection[0].Tag).Url).AbsolutePath;
                SetFolder(((LinkInfo)collection[0].Tag));
            }
            else if (collection.Count >= 1)
            {
                List<LinkInfo> linkInfoList = new List<LinkInfo>();

                foreach (ListViewItem listViewItem in collection)
                {
                    linkInfoList.Add((LinkInfo)listViewItem.Tag);
                }

                StartDownload(linkInfoList, Properties.Settings.Default.DOUBLE_CLICK_ACTION == "Open");
            }

            return;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.Items)
                listViewItem.Selected = true;
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.Items)
                listViewItem.Selected = false;
        }

        private void selectFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.Items)
            {
                LinkInfo linkInfo = (LinkInfo)listViewItem.Tag;
                if (!linkInfo.Size.Equals("Directory"))
                    listViewItem.Selected = true;
                else
                    listViewItem.Selected = false;
            }
        }

        private void selectFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem listViewItem in listView1.Items)
            {
                LinkInfo linkInfo = (LinkInfo)listViewItem.Tag;
                if (linkInfo.Size.Equals("Directory"))
                    listViewItem.Selected = true;
                else
                    listViewItem.Selected = false;
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection collection = listView1.SelectedItems;

            List<LinkInfo> linkInfoList = new List<LinkInfo>();

            foreach (ListViewItem listViewItem in collection)
            {
                linkInfoList.Add((LinkInfo)listViewItem.Tag);
            }

            StartDownload(linkInfoList);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection collection = listView1.SelectedItems;

            List<LinkInfo> fileLinkInfoList = new List<LinkInfo>();
            List<LinkInfo> folderLinkInfoList = new List<LinkInfo>();

            foreach (ListViewItem listViewItem in collection)
            {
                LinkInfo linkInfo = (LinkInfo)listViewItem.Tag;

                if (!linkInfo.Size.Equals("Directory"))
                    fileLinkInfoList.Add(linkInfo);
                else
                    folderLinkInfoList.Add(linkInfo);
            }

            foreach (LinkInfo folderLinkInfo in folderLinkInfoList)
            {
                var thread = new Thread(new ThreadStart(() =>
                {
                    FileExplorerWindow form1 = new FileExplorerWindow(folderLinkInfo, homeInfo.user);
                    Program.StartProgram(form1, homeInfo.user);
                }));

                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }

            StartDownload(fileLinkInfoList, true);
        }

        private void closeToolStripButton_Click(object sender, EventArgs e)
        {
            if (closeToolStripButton.Tag != null && (bool)closeToolStripButton.Tag)
            {
                listView1.Focus();
            }
            else
            {
                CancelBackgroundOperations();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(Settings_Save_Locations);
            settingsWindow.ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            this.Close();
        }

        private void changeServiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            this.Close();
        }

        private void regularFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!closeToolStripButton.Enabled)
            {
                toolStripTextBox1.Clear();
                toolStripTextBox1.Text = "find:" + ((listView1.SelectedItems.Count > 0) ? ((LinkInfo)listView1.SelectedItems[0].Tag).Name : null);

                toolStripTextBox1.Focus();
                toolStripTextBox1.SelectAll();

                toolStripTextBox1.LostFocus += ToolStripTextBox1_LostFocus;
                closeToolStripButton.Enabled = true;
                closeToolStripButton.Tag = true;
            }
        }

        private void ToolStripTextBox1_LostFocus(object sender, EventArgs e)
        {
            toolStripTextBox1.LostFocus -= ToolStripTextBox1_LostFocus;
            closeToolStripButton.Enabled = false;
            closeToolStripButton.Tag = false;
            toolStripTextBox1.Text = ((LinkInfo)toolStripTextBox1.Tag).ToString();

            return;
        }

        private void recursiveFindToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var linkInfo = (listView1.SelectedItems.Count > 0) ? ((LinkInfo)listView1.SelectedItems[0].Tag) : (LinkInfo)(toolStripTextBox1.Tag);

            AdvancedFindForm advancedFind = new AdvancedFindForm(linkInfo);
            if (advancedFind.ShowDialog() == DialogResult.OK)
            {
                AdvancedFindForm.SearchInfo searchInfo = advancedFind.GetSearchInfo();
                toolStripTextBox1.Text = searchInfo.ToString();
                toolStripTextBox1.Focus();

                CheckAddressBar(new LinkInfo(null, linkInfo.BaseInfo, null, null, null, null), searchInfo.ToString());
            }
        }

        private void imageViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            detailsViewToolStripMenuItem.Checked = false;

            listView1.View = View.LargeIcon;

            /*
            string[] keys = new string[listView1.LargeImageList.Images.Keys.Count];
            listView1.LargeImageList.Images.Keys.CopyTo(keys, 0);

            listView1.LargeImageList.im
            foreach(ImageList.ImageCollection image in listView1.LargeImageList.Images)
            {
                listView1.LargeImageList = listView1.LargeImageList;
            }
            */

            //listView1.Columns.Clear();
        }

        private int sortColumn = -1;

        private void detailsViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageViewToolStripMenuItem.Checked = false;

            //listView1.SmallImageList = listView1.LargeImageList;
            listView1.View = View.Details;


            /*listView1.Columns.Clear();

            listView1.Columns.Add(nameColumnHeader);
            listView1.Columns.Add(urlColumnHeader);
            listView1.Columns.Add(dateColumnHeader);
            listView1.Columns.Add(sizeColumnHeader);
            */

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            // CheckAddressBar((LinkInfo)toolStripTextBox1.Tag, toolStripTextBox1.Text);
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSort(listView1.Columns.IndexOf(nameColumnHeader), (ToolStripMenuItem)sender);
        }

        private void dateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSort(listView1.Columns.IndexOf(dateColumnHeader), (ToolStripMenuItem)sender);
        }

        private void urlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSort(listView1.Columns.IndexOf(urlColumnHeader), (ToolStripMenuItem)sender);
        }

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSort(listView1.Columns.IndexOf(sizeColumnHeader), (ToolStripMenuItem)sender);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private readonly ToolStripMenuItem addBookmarkToolStripMenuItem;
        private readonly ToolStripSeparator separatorToolStripMenuItem;

        private void AddBookmarksToMenu(string bookMarkPath)
        {
            SetBookmarks(AddBookmarkWindow.GetItemsTreeView(bookMarkPath, null, null));
        }

        private void SetBookmarks(IEnumerable<TreeNode> nodes, int skipNodes = 0)
        {
            bookmarksToolStripMenuItem.DropDownItems.Clear();

            bookmarksToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addBookmarkToolStripMenuItem, separatorToolStripMenuItem });

            if (nodes/*bookmark*/ != null)
            {
                //var nodes = bookmark.GetNodes();
                //nodes.RemoveAt(0);

                foreach (TreeNode node in nodes.Skip(skipNodes))
                {
                    TreeNodesToToolStripItem(node, bookmarksToolStripMenuItem.DropDownItems);
                }
            }
        }

        private void AddBookmark()
        {
            bookmarksFileSystemWatcher.EnableRaisingEvents = false;

            AddBookmarkWindow bookmark = null;

            if (listView1.SelectedItems.Count > 0)
                foreach (var items in listView1.SelectedItems)
                {
                    bookmark = new AddBookmarkWindow((LinkInfo)((ListViewItem)items).Tag);
                    bookmark.ShowDialog();
                }
            else
            {
                bookmark = new AddBookmarkWindow((LinkInfo)toolStripTextBox1.Tag/*new LinkInfo("", homeInfo.linkInfo.BaseInfo, "", "Directory", null, "Directory")*/);
                bookmark.ShowDialog();
            }

            if (bookmark != null)
                SetBookmarks(bookmark.GetNodes().Cast<TreeNode>(), 1);

            bookmarksFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void TreeNodesToToolStripItem(TreeNode startNode, ToolStripItemCollection startDropDownItems)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(startNode.Text);

            if ((bool)startNode.Tag)
            {
                menuItem.Image = Properties.Icons.folder.ToBitmap();
            }
            else
            {
                menuItem.Image = Properties.Icons.shortcut.ToBitmap();
                menuItem.Click += MenuItem_Click;
                menuItem.ToolTipText = startNode.ToolTipText;
            }

            startDropDownItems.Add(menuItem);

            foreach (TreeNode node in startNode.Nodes)
                TreeNodesToToolStripItem(node, menuItem.DropDownItems);
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            //var linkInfo = new LinkInfo(((ToolStripMenuItem)sender).ToolTipText, homeInfo.linkInfo.BaseInfo, ((ToolStripMenuItem)sender).Text, "Directory", null, "Directory");
            //SetFolder(linkInfo);
            CheckAddressBar((LinkInfo)toolStripTextBox1.Tag, ((ToolStripMenuItem)sender).ToolTipText);
        }

        private void addBookmarkRightClickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBookmark();
        }
    }

    public class TaggedWebClient : WebClient
    {
        public Object Tag;

        /*
        private static string cert;

        public void SetCertificate(String cert)
        {
            if (String.IsNullOrWhiteSpace(cert))
                CookieAwareWebClient.cert = null;
            else
                CookieAwareWebClient.cert = cert;
        }

        // callback used to validate the certificate in an SSL conversation
        private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = false;
            if (certificate.Subject.ToUpper().Contains(cert))
            {
                result = true;
            }

            return result;
        }

        public void Login(string loginPageAddress, NetworkCredential loginData)
        {
            CookieContainer container;

            if (!String.IsNullOrWhiteSpace(cert))
            {

                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                    ((sender, certificate, chain, sslPolicyErrors) => true);

                // trust sender
                System.Net.ServicePointManager.ServerCertificateValidationCallback
                                = ((sender, certificate, chain, errors) => certificate.Subject.Contains(cert));

                // validate cert by calling a function
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);
            }

            var req = (HttpWebRequest)WebRequest.Create(loginPageAddress);
            req.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            req.UseDefaultCredentials = false;
            req.PreAuthenticate = true;
            req.UnsafeAuthenticatedConnectionSharing = true;
            req.Credentials = loginData;

            container = req.CookieContainer = new CookieContainer();
            CookieContainer = container;
            

            WebResponse resp = req.GetResponse();
            /*
            StreamReader reader = new StreamReader(resp.GetResponseStream());
            var token = reader.ReadToEnd().Trim();
            */

        /*
        resp.Close();

    }

    public CookieAwareWebClient(CookieContainer container)
    {
        CookieContainer = container;
    }

    public CookieAwareWebClient()
      : this(new CookieContainer())
    { }

    public CookieContainer CookieContainer { get; private set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = (HttpWebRequest)base.GetWebRequest(address);
        request.CookieContainer = CookieContainer;
        return request;
    }
    */
    }


}
