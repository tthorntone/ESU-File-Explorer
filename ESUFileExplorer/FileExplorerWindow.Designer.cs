namespace ESUFileExplorer
{
    partial class FileExplorerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public class ToolStripSpringTextBox : System.Windows.Forms.ToolStripTextBox
        {
            public override System.Drawing.Size GetPreferredSize(System.Drawing.Size constrainingSize)
            {
                // Use the default size if the text box is on the overflow menu
                // or is on a vertical ToolStrip.
                if (IsOnOverflow || Owner.Orientation == System.Windows.Forms.Orientation.Vertical)
                {
                    return DefaultSize;
                }

                // Declare a variable to store the total available width as 
                // it is calculated, starting with the display width of the 
                // owning ToolStrip.
                System.Int32 width = Owner.DisplayRectangle.Width;

                // Subtract the width of the overflow button if it is displayed. 
                if (Owner.OverflowButton.Visible)
                {
                    width = width - Owner.OverflowButton.Width -
                        Owner.OverflowButton.Margin.Horizontal;
                }

                // Declare a variable to maintain a count of ToolStripSpringTextBox 
                // items currently displayed in the owning ToolStrip. 
                System.Int32 springBoxCount = 0;

                foreach (System.Windows.Forms.ToolStripItem item in Owner.Items)
                {
                    // Ignore items on the overflow menu.
                    if (item.IsOnOverflow) continue;

                    if (item is ToolStripSpringTextBox)
                    {
                        // For ToolStripSpringTextBox items, increment the count and 
                        // subtract the margin width from the total available width.
                        springBoxCount++;
                        width -= item.Margin.Horizontal;
                    }
                    else
                    {
                        // For all other items, subtract the full width from the total
                        // available width.
                        width = width - item.Width - item.Margin.Horizontal;
                    }
                }

                // If there are multiple ToolStripSpringTextBox items in the owning
                // ToolStrip, divide the total available width between them. 
                if (springBoxCount > 1) width /= springBoxCount;

                // If the available width is less than the default width, use the
                // default width, forcing one or more items onto the overflow menu.
                if (width < DefaultSize.Width) width = DefaultSize.Width;

                // Retrieve the preferred size from the base class, but change the
                // width to the calculated width. 
                System.Drawing.Size size = base.GetPreferredSize(constrainingSize);
                size.Width = width;
                return size;
            }
        }


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listView1 = new System.Windows.Forms.ListView();
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.urlColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dateColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.backToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.forwardToolStripButton = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripTextBox1 = new ESUFileExplorer.FileExplorerWindow.ToolStripSpringTextBox();
            this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sortToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.urlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.imageViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectFoldersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.fileToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolsToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regularFindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recursiveFindToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.bookmarksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.changeServiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addBookmarkRightClickToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.bookmarksFileSystemWatcher = new System.IO.FileSystemWatcher();
            this.toolStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bookmarksFileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.AllowColumnReorder = true;
            this.listView1.AllowDrop = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.urlColumnHeader,
            this.dateColumnHeader,
            this.sizeColumnHeader});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 54);
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(446, 450);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Tag = "";
            this.nameColumnHeader.Text = "Name";
            // 
            // urlColumnHeader
            // 
            this.urlColumnHeader.Tag = "";
            this.urlColumnHeader.Text = "Url";
            // 
            // dateColumnHeader
            // 
            this.dateColumnHeader.Tag = "";
            this.dateColumnHeader.Text = "Date";
            // 
            // sizeColumnHeader
            // 
            this.sizeColumnHeader.Tag = "";
            this.sizeColumnHeader.Text = "Size";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backToolStripButton,
            this.forwardToolStripButton,
            this.toolStripSeparator1,
            this.toolStripTextBox1,
            this.closeToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(446, 27);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // backToolStripButton
            // 
            this.backToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backToolStripButton.Image = global::ESUFileExplorer.Properties.Images.back_black;
            this.backToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backToolStripButton.Name = "backToolStripButton";
            this.backToolStripButton.Size = new System.Drawing.Size(39, 24);
            this.backToolStripButton.Text = "Back";
            // 
            // forwardToolStripButton
            // 
            this.forwardToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.forwardToolStripButton.Image = global::ESUFileExplorer.Properties.Images.forward_black;
            this.forwardToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.forwardToolStripButton.Name = "forwardToolStripButton";
            this.forwardToolStripButton.Size = new System.Drawing.Size(39, 24);
            this.forwardToolStripButton.Text = "Forward";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(10, 0, 15, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(266, 27);
            // 
            // closeToolStripButton
            // 
            this.closeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.closeToolStripButton.Image = global::ESUFileExplorer.Properties.Images.close_black;
            this.closeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.closeToolStripButton.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.closeToolStripButton.Name = "closeToolStripButton";
            this.closeToolStripButton.Size = new System.Drawing.Size(24, 24);
            this.closeToolStripButton.Text = "Close";
            this.closeToolStripButton.ToolTipText = "Close";
            this.closeToolStripButton.Click += new System.EventHandler(this.closeToolStripButton_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortToolStripMenuItem1,
            this.toolStripSeparator5,
            this.imageViewToolStripMenuItem,
            this.detailsViewToolStripMenuItem,
            this.toolStripSeparator6,
            this.selectAllToolStripMenuItem,
            this.selectNoneToolStripMenuItem,
            this.selectFilesToolStripMenuItem,
            this.selectFoldersToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(199, 198);
            // 
            // sortToolStripMenuItem1
            // 
            this.sortToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nameToolStripMenuItem,
            this.dateToolStripMenuItem,
            this.urlToolStripMenuItem,
            this.sizeToolStripMenuItem});
            this.sortToolStripMenuItem1.Name = "sortToolStripMenuItem1";
            this.sortToolStripMenuItem1.Size = new System.Drawing.Size(198, 26);
            this.sortToolStripMenuItem1.Text = "Sort";
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Checked = true;
            this.nameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(124, 26);
            this.nameToolStripMenuItem.Tag = "Name";
            this.nameToolStripMenuItem.Text = "Name";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.nameToolStripMenuItem_Click);
            // 
            // dateToolStripMenuItem
            // 
            this.dateToolStripMenuItem.Name = "dateToolStripMenuItem";
            this.dateToolStripMenuItem.Size = new System.Drawing.Size(124, 26);
            this.dateToolStripMenuItem.Tag = "Date";
            this.dateToolStripMenuItem.Text = "Date";
            this.dateToolStripMenuItem.Click += new System.EventHandler(this.dateToolStripMenuItem_Click);
            // 
            // urlToolStripMenuItem
            // 
            this.urlToolStripMenuItem.Name = "urlToolStripMenuItem";
            this.urlToolStripMenuItem.Size = new System.Drawing.Size(124, 26);
            this.urlToolStripMenuItem.Tag = "Url";
            this.urlToolStripMenuItem.Text = "Url";
            this.urlToolStripMenuItem.Click += new System.EventHandler(this.urlToolStripMenuItem_Click);
            // 
            // sizeToolStripMenuItem
            // 
            this.sizeToolStripMenuItem.Name = "sizeToolStripMenuItem";
            this.sizeToolStripMenuItem.Size = new System.Drawing.Size(124, 26);
            this.sizeToolStripMenuItem.Tag = "Size";
            this.sizeToolStripMenuItem.Text = "Size";
            this.sizeToolStripMenuItem.Click += new System.EventHandler(this.sizeToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
            // 
            // imageViewToolStripMenuItem
            // 
            this.imageViewToolStripMenuItem.Checked = true;
            this.imageViewToolStripMenuItem.CheckOnClick = true;
            this.imageViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.imageViewToolStripMenuItem.Name = "imageViewToolStripMenuItem";
            this.imageViewToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.imageViewToolStripMenuItem.Text = "Image View";
            this.imageViewToolStripMenuItem.Click += new System.EventHandler(this.imageViewToolStripMenuItem_Click);
            // 
            // detailsViewToolStripMenuItem
            // 
            this.detailsViewToolStripMenuItem.CheckOnClick = true;
            this.detailsViewToolStripMenuItem.Name = "detailsViewToolStripMenuItem";
            this.detailsViewToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.detailsViewToolStripMenuItem.Text = "Details View";
            this.detailsViewToolStripMenuItem.Click += new System.EventHandler(this.detailsViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(195, 6);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // selectNoneToolStripMenuItem
            // 
            this.selectNoneToolStripMenuItem.Name = "selectNoneToolStripMenuItem";
            this.selectNoneToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.selectNoneToolStripMenuItem.Text = "Select None";
            this.selectNoneToolStripMenuItem.Click += new System.EventHandler(this.selectNoneToolStripMenuItem_Click);
            // 
            // selectFilesToolStripMenuItem
            // 
            this.selectFilesToolStripMenuItem.Name = "selectFilesToolStripMenuItem";
            this.selectFilesToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.selectFilesToolStripMenuItem.Text = "Select Files";
            this.selectFilesToolStripMenuItem.Click += new System.EventHandler(this.selectFilesToolStripMenuItem_Click);
            // 
            // selectFoldersToolStripMenuItem
            // 
            this.selectFoldersToolStripMenuItem.Name = "selectFoldersToolStripMenuItem";
            this.selectFoldersToolStripMenuItem.Size = new System.Drawing.Size(198, 26);
            this.selectFoldersToolStripMenuItem.Text = "Select Folders";
            this.selectFoldersToolStripMenuItem.Click += new System.EventHandler(this.selectFoldersToolStripMenuItem_Click);
            // 
            // toolStrip2
            // 
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripDropDownButton,
            this.editToolStripDropDownButton,
            this.toolsToolStripDropDownButton});
            this.toolStrip2.Location = new System.Drawing.Point(0, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.ShowItemToolTips = false;
            this.toolStrip2.Size = new System.Drawing.Size(446, 27);
            this.toolStrip2.TabIndex = 2;
            this.toolStrip2.Text = "toolStrip2";
            // 
            // fileToolStripDropDownButton
            // 
            this.fileToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.downloadToolStripMenuItem,
            this.toolStripSeparator3,
            this.exitToolStripMenuItem});
            this.fileToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fileToolStripDropDownButton.Name = "fileToolStripDropDownButton";
            this.fileToolStripDropDownButton.ShowDropDownArrow = false;
            this.fileToolStripDropDownButton.Size = new System.Drawing.Size(36, 24);
            this.fileToolStripDropDownButton.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(200, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripDropDownButton
            // 
            this.editToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.editToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.editToolStripDropDownButton.Name = "editToolStripDropDownButton";
            this.editToolStripDropDownButton.ShowDropDownArrow = false;
            this.editToolStripDropDownButton.Size = new System.Drawing.Size(39, 24);
            this.editToolStripDropDownButton.Text = "Edit";
            // 
            // toolsToolStripDropDownButton
            // 
            this.toolsToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.toolStripSeparator2,
            this.bookmarksToolStripMenuItem,
            this.toolStripSeparator7,
            this.changeServiceToolStripMenuItem,
            this.logoutToolStripMenuItem,
            this.toolStripSeparator4,
            this.settingsToolStripMenuItem});
            this.toolsToolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolsToolStripDropDownButton.Name = "toolsToolStripDropDownButton";
            this.toolsToolStripDropDownButton.ShowDropDownArrow = false;
            this.toolsToolStripDropDownButton.Size = new System.Drawing.Size(48, 24);
            this.toolsToolStripDropDownButton.Text = "Tools";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.regularFindToolStripMenuItem,
            this.recursiveFindToolStripMenuItem});
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // regularFindToolStripMenuItem
            // 
            this.regularFindToolStripMenuItem.Name = "regularFindToolStripMenuItem";
            this.regularFindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.regularFindToolStripMenuItem.Size = new System.Drawing.Size(276, 26);
            this.regularFindToolStripMenuItem.Text = "Regular Find";
            this.regularFindToolStripMenuItem.Click += new System.EventHandler(this.regularFindToolStripMenuItem_Click);
            // 
            // recursiveFindToolStripMenuItem
            // 
            this.recursiveFindToolStripMenuItem.Name = "recursiveFindToolStripMenuItem";
            this.recursiveFindToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.F)));
            this.recursiveFindToolStripMenuItem.Size = new System.Drawing.Size(276, 26);
            this.recursiveFindToolStripMenuItem.Text = "Advanced Search";
            this.recursiveFindToolStripMenuItem.Click += new System.EventHandler(this.recursiveFindToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // bookmarksToolStripMenuItem
            // 
            this.bookmarksToolStripMenuItem.Name = "bookmarksToolStripMenuItem";
            this.bookmarksToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.bookmarksToolStripMenuItem.Text = "Bookmarks";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(182, 6);
            // 
            // changeServiceToolStripMenuItem
            // 
            this.changeServiceToolStripMenuItem.Name = "changeServiceToolStripMenuItem";
            this.changeServiceToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.changeServiceToolStripMenuItem.Text = "Change Service";
            this.changeServiceToolStripMenuItem.Click += new System.EventHandler(this.changeServiceToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(182, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(185, 26);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addBookmarkRightClickToolStripMenuItem,
            this.toolStripSeparator9,
            this.openToolStripMenuItem1,
            this.downloadToolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(184, 88);
            // 
            // addBookmarkRightClickToolStripMenuItem
            // 
            this.addBookmarkRightClickToolStripMenuItem.Name = "addBookmarkRightClickToolStripMenuItem";
            this.addBookmarkRightClickToolStripMenuItem.Size = new System.Drawing.Size(183, 26);
            this.addBookmarkRightClickToolStripMenuItem.Text = "Add Bookmark";
            this.addBookmarkRightClickToolStripMenuItem.Click += new System.EventHandler(this.addBookmarkRightClickToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(180, 6);
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(183, 26);
            this.openToolStripMenuItem1.Text = "Open";
            // 
            // downloadToolStripMenuItem1
            // 
            this.downloadToolStripMenuItem1.Name = "downloadToolStripMenuItem1";
            this.downloadToolStripMenuItem1.Size = new System.Drawing.Size(183, 26);
            this.downloadToolStripMenuItem1.Text = "Download";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // bookmarksFileSystemWatcher
            // 
            this.bookmarksFileSystemWatcher.EnableRaisingEvents = true;
            this.bookmarksFileSystemWatcher.SynchronizingObject = this;
            // 
            // FileExplorerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 504);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.toolStrip2);
            this.Name = "FileExplorerWindow";
            this.Text = "File Explorer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.contextMenuStrip2.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bookmarksFileSystemWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSplitButton backToolStripButton;
        private System.Windows.Forms.ToolStripSplitButton forwardToolStripButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripDropDownButton editToolStripDropDownButton;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripDropDownButton fileToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton toolsToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripSpringTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem recursiveFindToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regularFindToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader urlColumnHeader;
        private System.Windows.Forms.ColumnHeader dateColumnHeader;
        private System.Windows.Forms.ColumnHeader sizeColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sortToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem urlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem imageViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFilesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectFoldersToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeServiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem bookmarksToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem addBookmarkRightClickToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.IO.FileSystemWatcher bookmarksFileSystemWatcher;
    }
}

