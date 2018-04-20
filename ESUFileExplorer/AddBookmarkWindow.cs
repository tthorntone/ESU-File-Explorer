using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public partial class AddBookmarkWindow : Form
    {
        private readonly string bookmarksFolder;

        private readonly ContextMenu createFolderContextMenu;
        private readonly ContextMenu folderContextMenu;
        private readonly ContextMenu shortcutContextMenu;
        private readonly ContextMenu selectedContextMenu;

        private const int TVIF_STATE = 0x8;
        private const int TVIS_STATEIMAGEMASK = 0xF000;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETITEM = TV_FIRST + 63;

        [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
        private struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam,
                                                 ref TVITEM lParam);

        /// <summary>
        /// Hides the checkbox for the specified node on a TreeView control.
        /// </summary>
        private void HideCheckBox(TreeView tvw, TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            SendMessage(tvw.Handle, TVM_SETITEM, IntPtr.Zero, ref tvi);
        }

        public TreeNodeCollection GetNodes()
        {
            return treeView1.Nodes;
        }

        private void CreateFolder()
        {
            TreeNode newFolder = new TreeNode("New Folder");
            newFolder.Tag = -1;

            TreeNode parentNode = treeView1.SelectedNode;
            if (parentNode != null && parentNode.Name == "createFolderNode")
                parentNode = null;

            if (parentNode != null)
            {
                parentNode.Nodes.Add(newFolder);
                newFolder.Checked = parentNode.Checked;
                parentNode.Expand();
            }
            else
                treeView1.Nodes.Add(newFolder);

            treeView1.LabelEdit = true;

            //treeView1.Sort();

            newFolder.BeginEdit();
        }

        private void TreeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            treeView1.LabelEdit = false;

            if (e.Node.Tag is int && string.IsNullOrWhiteSpace(e.Label))
                treeView1.Nodes.Remove(e.Node);
            else if (!string.IsNullOrWhiteSpace(e.Label) && e.Node.Text != e.Label)
            {
                string path = Path.Combine(Path.GetDirectoryName(e.Node.FullPath), e.Label);
                if (((e.Node.Tag is int || (bool)e.Node.Tag) && Directory.Exists(Path.Combine(bookmarksFolder, path))) ||
                    ((e.Node.Tag is bool && !(bool)e.Node.Tag) && File.Exists(Path.Combine(bookmarksFolder, path + ".lnk"))))
                {
                    if (e.Node.Tag is int || (bool)e.Node.Tag)
                        MessageBox.Show(string.Format("'{0}' Folder Already Exists!\n", path), "ERROR", MessageBoxButtons.OK);
                    else
                        MessageBox.Show(string.Format("'{0}' Link Already Exists!\n", path), "ERROR", MessageBoxButtons.OK);

                    e.CancelEdit = true;

                    treeView1.LabelEdit = true;
                    e.Node.BeginEdit();
                    return;
                }

                bookmarksFileSystemWatcher.EnableRaisingEvents = false;

                if (e.Node.Tag is int)
                {
                    Directory.CreateDirectory(Path.Combine(bookmarksFolder, Path.GetDirectoryName(e.Node.FullPath), e.Label));
                    e.Node.ContextMenu = folderContextMenu;
                    e.Node.Tag = true;
                }
                else
                {
                    if ((bool)e.Node.Tag)
                    {
                        Directory.Move(Path.Combine(bookmarksFolder, e.Node.FullPath), Path.Combine(bookmarksFolder, path));
                    }
                    else
                    {
                        File.Move(Path.Combine(bookmarksFolder, e.Node.FullPath + ".lnk"), Path.Combine(bookmarksFolder, path + ".lnk"));
                    }
                }

                bookmarksFileSystemWatcher.EnableRaisingEvents = true;

                treeView1.BeginInvoke(new MethodInvoker(() =>
                {
                    treeView1.Sort();
                    HideCheckBox(treeView1, treeView1.Nodes[0]);
                }));
            }
        }

        List<TreeNode> CheckedNodes = new List<TreeNode>();

        private enum DeleteFlag { Folder, Shortcut, Multiple }

        private void DeleteSelected()
        {
            if (CheckedNodes.Count > 0)
            {
                CheckedNodes.Sort((new TreeNodeComparer(SortOrder.Ascending, true, true)));

                if (MessageBox.Show(string.Format("Are You Sure You Want to Delete:\n\t{0}", string.Join("\n\t", CheckedNodes.Select((x) => "'" + x.FullPath + "'" + (((bool)x.Tag) ? " Folder" : " Link")))), "ERROR", MessageBoxButtons.OKCancel) != DialogResult.OK)
                    return;

                bookmarksFileSystemWatcher.EnableRaisingEvents = false;

                TreeNode node;
                for (int i = CheckedNodes.Count - 1; i >= 0; i--)
                {
                    node = CheckedNodes[i];

                    if ((bool)node.Tag)
                        Directory.Delete(Path.Combine(bookmarksFolder, node.FullPath), true);
                    else
                        File.Delete(Path.Combine(bookmarksFolder, node.FullPath + ".lnk"));

                    node.Remove();
                    CheckedNodes.RemoveAt(i);
                }

                bookmarksFileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        private void Delete(bool folder = true)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null || node.Name == "createFolderNode")
                return;

            if (MessageBox.Show(string.Format("Are You Sure You Want to Delete '{0}'?", node.Text), "ERROR", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            bookmarksFileSystemWatcher.EnableRaisingEvents = false;

            if (folder)
            {
                Directory.Delete(Path.Combine(bookmarksFolder, node.FullPath), true);
                CheckedNodes.RemoveAll((x) => x.FullPath.StartsWith(node.FullPath + "\\"));
            }
            else
                File.Delete(Path.Combine(bookmarksFolder, node.FullPath + ".lnk"));

            node.Remove();
            CheckedNodes.Remove(node);

            bookmarksFileSystemWatcher.EnableRaisingEvents = true;
        }

        private void Edit()
        {
            TreeNode parentNode = treeView1.SelectedNode;
            if (parentNode == null || parentNode.Name == "createFolderNode")
                return;

            treeView1.LabelEdit = true;

            parentNode.BeginEdit();
        }

        private void SelectAll()
        {
            foreach (TreeNode node in treeView1.Nodes)
                node.Checked = true;
        }

        private void SelectNone()
        {
            foreach (TreeNode node in treeView1.Nodes)
                node.Checked = false;
        }

        public AddBookmarkWindow(FileExplorerWindow.LinkInfo linkInfo)
        {
            InitializeComponent();

            createFolderContextMenu = new ContextMenu(
                new MenuItem[] { new MenuItem("Create Folder", (x, y) => { CreateFolder(); }), new MenuItem("-"),
                new MenuItem("Select All", (x, y) => {SelectAll(); }), new MenuItem("Select None", (x, y) => { SelectNone(); }), new MenuItem("-"),
                new MenuItem("Delete Selected", (x, y) => { DeleteSelected(); })});

            folderContextMenu = new ContextMenu(
                new MenuItem[] { new MenuItem("Create Folder", (x, y) => { CreateFolder(); }), new MenuItem("Edit", (x, y) => { Edit(); }),
                new MenuItem("Delete", (x, y) => { Delete(true); }), new MenuItem("-"),
                new MenuItem("Select All", (x, y) => {SelectAll(); }), new MenuItem("Select None", (x, y) => { SelectNone(); }), new MenuItem("-"),
                new MenuItem("Delete Selected", (x, y) => { DeleteSelected(); })});

            shortcutContextMenu = new ContextMenu(
                new MenuItem[] { new MenuItem("Edit", (x, y) => { Edit(); }), new MenuItem("Delete", (x, y) => { Delete(false); }), new MenuItem("-"),
                new MenuItem("Select All", (x, y) => {SelectAll(); }), new MenuItem("Select None", (x, y) => { SelectNone(); }), new MenuItem("-"),
                new MenuItem("Delete Selected", (x, y) => { DeleteSelected(); })});

            selectedContextMenu = new ContextMenu(
                new MenuItem[] {new MenuItem("Select All", (x, y) => {SelectAll(); }), new MenuItem("Select None", (x, y) => { SelectNone(); }), new MenuItem("-"),
                new MenuItem("Delete Selected", (x, y) => { DeleteSelected(); }) });

            /*
            var urlLocations = FileExplorerWindow.GetLocations();

            if (urlLocations.Count <= 0)
            {
                MessageBox.Show("Found No Valid Locations...", "No Locations");
                this.Close();
                return;
            }

            urlLocations.Sort((a, b) => a.Index.CompareTo(b.Index));

            serviceComboBox.Items.AddRange(urlLocations.ToArray());

            int index = urlLocations.FindIndex((x) => x.BaseAddress == linkInfo.BaseInfo);

            serviceComboBox.SelectedIndex = index;
            */


            NameTextBox.Text = linkInfo.Name;
            addressTextBox.Text = linkInfo.ToString();

            serviceComboBox.Items.Add(linkInfo.BaseInfo);
            serviceComboBox.SelectedIndex = 0;

            bookmarksFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Applicat‌​ionData), "Trailblazer101", "ESUFileExplorer", "Bookmarks", linkInfo.BaseInfo.Name /*urlLocations[index].Name*/);

            if (!Directory.Exists(bookmarksFolder))
            {
                Directory.CreateDirectory(bookmarksFolder);
            }

            //serviceComboBox.SelectedIndex = 0;

            treeView1.ContextMenu = selectedContextMenu;

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("folder", Properties.Icons.folder);
            treeView1.ImageList.Images.Add("create_folder", Properties.Icons.create_folder);
            treeView1.ImageList.Images.Add("shortcut", Properties.Icons.shortcut);
            treeView1.ImageList.ColorDepth = ColorDepth.Depth32Bit;
            treeView1.ImageList.ImageSize = new Size(24, 24);

            AddCreateFolderNode();

            treeView1.Nodes.AddRange(GetItemsTreeView(bookmarksFolder, folderContextMenu, shortcutContextMenu));

            treeView1.TreeViewNodeSorter = new TreeNodeComparer(SortOrder.Ascending);

            treeView1.Sort();

            this.Load += AddBookmarkWindow_Load;
            this.FormClosing += AddBookmarkWindow_FormClosing;
            treeView1.BeforeCheck += TreeView1_BeforeCheck;
            treeView1.AfterCheck += TreeView1_AfterCheck;
            treeView1.AfterLabelEdit += TreeView1_AfterLabelEdit;
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;
        }

        private void AddBookmarkWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            bookmarksFileSystemWatcher.EnableRaisingEvents = false;
            bookmarksFileSystemWatcher.Dispose();
        }

        private TreeNode AddCreateFolderNode()
        {
            TreeNode createFolderNode = new TreeNode("Create Folder");
            createFolderNode.Name = "createFolderNode";
            createFolderNode.ImageKey = "create_folder";
            createFolderNode.SelectedImageKey = "create_folder";
            createFolderNode.ContextMenu = createFolderContextMenu;
            createFolderNode.Tag = -1;

            treeView1.Nodes.Add(createFolderNode);

            return createFolderNode;
        }


        private void TreeView1_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is int && (int)e.Node.Tag == -1)
                e.Cancel = true;
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is bool)
            {

                e.Node.GetChildren().ToList().ForEach((x) =>
            {
                x.Tag = ((bool)x.Tag ? 1 : 0) + 2;
                x.Checked = e.Node.Checked;
            });

                //foreach (TreeNode node in e.Node.Nodes)
                //     node.Checked = e.Node.Checked;

                if ((bool)e.Node.Tag)
                    CheckedNodes.RemoveAll((x) => /*((bool)x.Tag == (bool)e.Node.Tag) && */ x.FullPath.StartsWith(e.Node.FullPath + "\\"));

                if (e.Node.Checked)
                {
                    TreeNode parentNode = e.Node.Parent;

                    if (parentNode != null && !parentNode.Checked)
                    {
                        if (parentNode.Nodes.Cast<TreeNode>().All((x) => x.Checked))
                        {
                            //CheckedNodes.Add(parentNode);

                            //parentNode.Tag = ((bool)parentNode.Tag ? 1 : 0) + 2;
                            parentNode.Checked = true;
                            return;
                        }
                    }

                    if (!CheckedNodes.Exists((x) => e.Node == x))
                        CheckedNodes.Add(e.Node);
                    //else
                    //   MessageBox.Show("HMMM");
                }
                else if (!e.Node.Checked)
                {
                    CheckedNodes.Remove(e.Node);

                    TreeNode parentNode = e.Node.Parent;

                    while (parentNode != null && parentNode.Checked)
                    {
                        parentNode.Tag = ((bool)parentNode.Tag ? 1 : 0) + 2;
                        parentNode.Checked = false;

                        CheckedNodes.AddRange(parentNode.Nodes.Cast<TreeNode>().Where((x) => x.Checked));
                        CheckedNodes.Remove(parentNode);

                        parentNode = parentNode.Parent;
                    }
                }
            }
            else// if ((int)e.Node.Tag != -1)
                e.Node.Tag = ((int)e.Node.Tag == 3) ? true : false;

        }

        private void AddBookmarkWindow_Load(object sender, EventArgs e)
        {
            HideCheckBox(treeView1, treeView1.Nodes[0]);
            RegisterFolderWatcher();
        }

        private void RegisterFolderWatcher()
        {
            bookmarksFileSystemWatcher.Path = bookmarksFolder;
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
            CheckedNodes.Clear();
            treeView1.Nodes.Clear();

            var node = AddCreateFolderNode();

            treeView1.Nodes.AddRange(GetItemsTreeView(bookmarksFolder, folderContextMenu, shortcutContextMenu));
            treeView1.Sort();

            HideCheckBox(treeView1, node);
        }

        public class TreeNodeComparer : IComparer, IComparer<TreeNode>
        {
            private SortOrder order;
            bool foldersFirst;
            bool fullPath;

            public TreeNodeComparer(SortOrder order, bool foldersFirst = true, bool fullPath = false)
            {
                this.order = order;
                this.foldersFirst = foldersFirst;
                this.fullPath = fullPath;
            }

            public int Compare(TreeNode thisNode, TreeNode otherNode)
            {
                int returnVal;

                //TreeNode thisNode = thisObj as TreeNode;
                //TreeNode otherNode = otherObj as TreeNode;

                if (thisNode.Name == "createFolderNode")
                    return -1;

                if (otherNode.Name == "createFolderNode")
                    return 1;

                // Compare the types of the tags, returning the difference.
                if (foldersFirst)
                {
                    if (((thisNode.Tag is bool && (bool)thisNode.Tag) || (thisNode.Tag is int && (int)thisNode.Tag == -1)) && (otherNode.Tag is bool && !(bool)otherNode.Tag))
                        return -1;
                    else if (((otherNode.Tag is bool && (bool)otherNode.Tag) || (otherNode.Tag is int && (int)otherNode.Tag == -1)) && (thisNode.Tag is bool && !(bool)thisNode.Tag))
                        return 1;
                }

                string thisPath;
                string otherPath;

                if (fullPath)
                    try
                    {
                        thisPath = thisNode.FullPath;
                    }
                    catch (InvalidOperationException)
                    {
                        thisPath = thisNode.Text;
                    }
                else
                    thisPath = thisNode.Text;

                if (fullPath)
                    try
                    {
                        otherPath = otherNode.FullPath;
                    }
                    catch (InvalidOperationException)
                    {
                        otherPath = otherNode.Text;
                    }
                else
                    otherPath = otherNode.Text;

                //alphabetically sorting
                returnVal = thisPath.CompareTo(otherPath);

                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;

                return returnVal;
            }

            public int Compare(object thisNode, object otherNode)
            {
                return Compare((TreeNode)thisNode, (TreeNode)otherNode);
            }
        }


        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Index == 0)
                CreateFolder();
        }

        public static TreeNode[] GetItemsTreeView(string currentPath, ContextMenu folderContextMenu, ContextMenu shortcutContextMenu)
        {
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }

            var tree = Directory.EnumerateDirectories(currentPath).OrderBy((x) => x).Select((y) =>
            {
                TreeNode node = new TreeNode(Path.GetFileName(y));
                node.Tag = true;
                node.ContextMenu = folderContextMenu;
                    //node.Name = y;
                    node.ImageKey = "folder";
                node.SelectedImageKey = "folder";
                node.Nodes.AddRange(GetItemsTreeView(y, folderContextMenu, shortcutContextMenu));
                return node;
            }).ToList();

            tree.AddRange(Directory.EnumerateFiles(currentPath, "*.lnk").OrderBy((x) => x).Select((y) =>
            {
                TreeNode node = new TreeNode(Path.GetFileNameWithoutExtension(y));
                node.Tag = false;
                node.ContextMenu = shortcutContextMenu;
                node.ToolTipText = File.ReadAllText(y);
                    //node.Name = y;
                    node.ImageKey = "shortcut";
                node.SelectedImageKey = "shortcut";

                return node;
            }));

            return tree.ToArray();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            if (node == null || node.Name == "createFolderNode" || (bool)node.Tag)
            {
                string path;

                if (node == null || node.Name == "createFolderNode")
                    path = Path.Combine(bookmarksFolder, NameTextBox.Text + ".lnk");
                else
                    path = Path.Combine(bookmarksFolder, node.FullPath, NameTextBox.Text + ".lnk");

                if (File.Exists(path))
                {
                    if (MessageBox.Show("Bookmark Already Exists!\nWould You Like to Overwrite?", "ERROR", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        return;
                }

                bookmarksFileSystemWatcher.EnableRaisingEvents = false;

                File.WriteAllText(path, addressTextBox.Text);

                TreeNode newNode = new TreeNode(NameTextBox.Text) { Tag = false, ToolTipText = addressTextBox.Text };

                if (node == null || node.Name == "createFolderNode")
                    treeView1.Nodes.Add(newNode);
                else
                    node.Nodes.Add(newNode);

                bookmarksFileSystemWatcher.EnableRaisingEvents = true;

                this.Close();
            }
            else
            {
                MessageBox.Show("Please Select a Folder!", "ERROR", MessageBoxButtons.OK);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void folderButton_Click(object sender, EventArgs e)
        {
            Process.Start(bookmarksFolder);
        }
    }
}
