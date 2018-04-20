using System;
using System.Collections;
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
    public partial class SettingsWindow : Form, CustomEditor
    {
        public CustomEditorClass customEditorClass
        {
            get;

            set;
        }

        ListView CustomEditor.listView1
        {
            get
            {
                return listView1;
            }

            set
            {
                listView1 = value;
            }
        }

        public Button DeleteButton
        {
            get
            {
                return deleteButton;
            }

            set
            {
                deleteButton = value;
            }
        }

        public Button MoveDownButton
        {
            get
            {
                return downButton;
            }

            set
            {
                downButton = value;
            }
        }

        public Button MoveUpButton
        {
            get
            {
                return upButton;
            }

            set
            {
                upButton = value;
            }
        }

        public Button AddButton
        {
            get
            {
                return newButton;
            }

            set
            {
                newButton = value;
            }
        }

        public EditScreen editScreen
        {
            get;

            set;
        }

        public Action<List<FileExplorerWindow.UrlLocation>, bool> SaveCallback
        {
            get;

            set;
        }



        public void Remove(ListViewItem listviewItem)
        {
           // listView1.SmallImageList.Images.RemoveByKey(listviewItem.ImageKey);
            listviewItem.Remove();
            listviewItem.ImageIndex = -1;
        }

        public void EditScreen_Save(FileExplorerWindow.UrlLocation newDataItem, int index)
        {
            customEditorClass.EditScreen_Save(newDataItem, index);
            //ImageSanityCheck();
        }

        public void swapListViewIndexes(ListViewItem first, ListViewItem second)
        {
            customEditorClass.swapListViewIndexes(first, second);


            //ImageSanityCheck();
        }

        public void addItem(FileExplorerWindow.UrlLocation dataItem, int index)
        {
            /*
             listViewItem = new System.Windows.Forms.ListViewItem(new string[] {
                    data[0], data[1], data[2]}, -1);

                listViewItem.Tag = urlLocation;

                listView1.Items.Add(listViewItem);
                */


            ListViewItem item1 = new ListViewItem(dataItem.Name);
            item1.SubItems.Add(dataItem.Index.ToString());
            item1.SubItems.Add(dataItem.BaseAddress);
            item1.SubItems.Add(dataItem.Url);

            item1.Name = dataItem.Index.ToString();
            item1.Tag = dataItem.Index;

            string name;
            if ((name = customEditorClass.FindListViewNameByIndex(dataItem.Index)) != null && listView1.Items.ContainsKey(name))
            {
                listView1.Items[listView1.Items.IndexOfKey(name)] = item1;
            }
            else
                listView1.Items.Add(item1);

            //listView1.Items[listView1.Items.IndexOfKey(dataItem.Name)].EnsureVisible();
            listView1.Sort();
        }

        public string GetCustom(EditScreen edit)
        {
            return edit.GetBaseAddressTextBoxText();
        }

        public void AddButton_Click(object sender, EventArgs e)
        {
            customEditorClass.AddButton_Click("Reached Maximum Locations!", 20, ((Button)sender).Text);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count - listView1.CheckedItems.Count < 1)
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                MessageBox.Show("You Have to Have at Least One Location!", "ERROR", button, icon);
            }
            else
            {
                customEditorClass.DeleteButton_Click();

                //ImageSanityCheck();
            }
        }

        public void OnNewTextChanged(TextEventArgs e)
        {
            // Create a copy of the event to work with
            EventHandler<TextEventArgs> eh = NewTextChanged;
            /* If there are no subscribers, eh will be null so we need to check
             * to avoid a NullReferenceException. */
            if (eh != null)
                eh(this, e);
        }



        public void CloseScreen()
        {
            this.Close();
        }


        public void RegisterEvents()
        {
            this.listView1.ItemChecked += customEditorClass.listView1_ItemChecked;
            this.listView1.ItemDrag += customEditorClass.listView1_ItemDrag;
            this.listView1.DragDrop += customEditorClass.listView1_DragDrop;
            this.listView1.DragEnter += customEditorClass.listView1_DragEnter;
            this.listView1.ColumnClick += customEditorClass.ColumnClick;

            this.upButton.Click += customEditorClass.MoveButton_Click;
            this.downButton.Click += customEditorClass.MoveButton_Click;

            //this.SaveCallback += deskOps.deskOpsTools.ManagerTools_Save_SearchProviders;
        }

      
        public SettingsWindow(Action<List<FileExplorerWindow.UrlLocation>, bool> SaveCallback)
        {
            customEditorClass = new CustomEditorClass(this);
            InitializeComponent();
            RegisterEvents();

            this.SaveCallback += SaveCallback;

            this.listView1.ListViewItemSorter = new CustomEditorClass.ListViewItemComparer(1);

            checkBox1.Checked = Properties.Settings.Default.REMEMBER_ME;
            checkBox2.Checked = Properties.Settings.Default.REMEMBER_LOCATION;

            comboBox1.SelectedItem = Properties.Settings.Default.DOUBLE_CLICK_ACTION;
            //System.Windows.Forms.ListViewItem listViewItem;

            //int index = 0;
            foreach (string location in Properties.Settings.Default.LOCATIONS)
            {
                string[] data = location.Split(';');

                if (data.Count() < 4)
                {
                    MessageBox.Show(string.Format("Could Not Parse {0}...", location), "Invalid Location");
                    continue;
                }

                FileExplorerWindow.UrlLocation urlLocation = new FileExplorerWindow.UrlLocation(data[0], int.Parse(data[1]), data[2], data[3]);

                addItem(urlLocation, -1);
                //index++;
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        public event EventHandler<TextEventArgs> NewTextChanged;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //if(((CheckBox)sender).Checked)
            checkBox2.Enabled = (((CheckBox)sender).Checked);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.REMEMBER_ME = checkBox1.Checked;
            Properties.Settings.Default.REMEMBER_LOCATION = checkBox1.Checked && checkBox2.Checked;
            Properties.Settings.Default.DOUBLE_CLICK_ACTION = (string)comboBox1.SelectedItem;

            customEditorClass.SaveButton_Click(sender, e);
        }
    }
}
