using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    public interface CustomEditor
    {
        CustomEditorClass customEditorClass { get; set; }

        //Panel panel1 { get; set; }
        ListView listView1 { get; set; }
        //SplitContainer parent { get; set; }
        Button DeleteButton { get; set; }
        Button MoveDownButton { get; set; }
        Button MoveUpButton { get; set; }
        Button AddButton { get; set; }

        EditScreen editScreen { get; set; }

        Action<List<FileExplorerWindow.UrlLocation>, bool> SaveCallback { get; set; }

        void Remove(ListViewItem listviewItem);

        void EditScreen_Save(FileExplorerWindow.UrlLocation newDataItem, int index);

        //void EditScreen_Close(DeskOps.FileExplorerWindow.UrlLocation newDataItem);

        void swapListViewIndexes(ListViewItem first, ListViewItem second);

        void addItem(FileExplorerWindow.UrlLocation dataItem, int index);

        string GetCustom(EditScreen edit);

        void AddButton_Click(object sender, EventArgs e);

        void OnNewTextChanged(TextEventArgs e);

        //void RestoreButton_Click(object sender, EventArgs e);

        void CloseScreen();

        event EventHandler<TextEventArgs> NewTextChanged;
    }

    public class CustomEditorClass : Form
    {
        public CustomEditor customEditor;

        public CustomEditorClass(CustomEditor customEditor)
        {
            this.customEditor = customEditor;
        }

        public ListView GetListViewClone()
        {
            ListView listView2 = new ListView();

            listView2.Items.AddRange((from ListViewItem item in customEditor.listView1.Items
                                      select (ListViewItem)item.Clone()).ToArray());

            return listView2;
        }

        public int GetListViewSize()
        {
            return customEditor.listView1.Items.Count;
        }

        public ListViewItem FindListViewByIndex(int index)
        {
            foreach (ListViewItem listviewitem in customEditor.listView1.Items)
            {
                if ((int)listviewitem.Tag == index)
                    return listviewitem;
            }

            return null;
        }

        public string FindListViewNameByIndex(int index)
        {
            ListViewItem item = FindListViewByIndex(index);

            if (item != null)
                return item.Name;
            else
                return null;
        }

        public bool CheckConfiguredIndexes(ListView.ListViewItemCollection List)
        {
            if (List.Count == 0)
                return true;

            bool good = false;

            for (int i = 0; i < List.Count; i++)
            {
                good = false;
                foreach (ListViewItem item in List)
                {
                    if ((int)item.Tag == i)
                    {
                        good = true;
                        break;
                    }
                }

                if (!good)
                {
                    MessageBoxButtons button = MessageBoxButtons.OK;
                    MessageBoxIcon icon = MessageBoxIcon.Warning;
                    MessageBox.Show(String.Format("Index {0} is Missing!", i), "ERROR", button, icon);
                    break;
                }
            }

            return good;
        }


        public void EditScreen_Save(FileExplorerWindow.UrlLocation newDataItem, int index)
        {
            ListViewItem listviewitem;

            if ((listviewitem = FindListViewByIndex(newDataItem.Index)) != null)
                customEditor.Remove(listviewitem);

            if (customEditor.editScreen != null)
            {
                customEditor.editScreen.Close();
            }

            customEditor.addItem(newDataItem, index);
            customEditor.OnNewTextChanged(new TextEventArgs(index, newDataItem));

            itemChecked();
        }



        public void swapListViewIndexes(ListViewItem first, ListViewItem second)
        {
            ListViewItem secondTemp = new ListViewItem();
            secondTemp = (ListViewItem)second.Clone();
            secondTemp.Name = second.Name;

            second.SubItems[1].Text = first.SubItems[1].Text;
            second.Tag = first.Tag;

            first.SubItems[1].Text = secondTemp.SubItems[1].Text;
            first.Tag = secondTemp.Tag;
        }

        /*
        public void closeAddEditPanel()
        {

            if (customEditor.editScreen != null)
            {
                customEditor.editScreen.Close();
                //editScreen = null;
            }
            customEditor.parent.Panel2.Controls.Clear();
            customEditor.parent.Panel2Collapsed = true;
        }

    */

        /*
public void addAddEditPanel(EditScreen edit, bool setEditScreen)
        {

            if (setEditScreen) customEditor.editScreen = edit;

            edit.TopLevel = false;
            edit.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            customEditor.parent.Panel2MinSize = edit.MinimumSize.Height;
            customEditor.parent.SplitterDistance = customEditor.parent.Height - edit.MinimumSize.Height;

            customEditor.parent.Panel2.Controls.Add(edit);
            edit.Dock = DockStyle.Fill;

            customEditor.parent.Panel2Collapsed = false;

        }
        */

        public void itemChecked()
        {
            if (customEditor.listView1.CheckedItems.Count > 0)
            {
                if (customEditor.listView1.CheckedItems.Count <= 1)
                {
                    customEditor.MoveUpButton.Enabled = true;
                    customEditor.MoveDownButton.Enabled = true;
                }
                else
                {
                    customEditor.MoveUpButton.Enabled = false;
                    customEditor.MoveDownButton.Enabled = false;
                }

                /*
                if (customEditor.editScreen == null)
                {
                    customEditor.editScreen = new EditScreen(customEditor.customEditorClass, new FileExplorerWindow.UrlLocation(customEditor.listView1.CheckedItems[0]), int.Parse(customEditor.listView1.CheckedItems[0].Name), "Edit");

                    addAddEditPanel(customEditor.editScreen, false);

                    customEditor.editScreen.Show();
                }
                */

                customEditor.DeleteButton.Enabled = true;
                customEditor.AddButton.Text = "Edit";
            }
            else
            {
                customEditor.MoveUpButton.Enabled = false;
                customEditor.MoveDownButton.Enabled = false;
                customEditor.DeleteButton.Enabled = false;
                customEditor.AddButton.Text = "Add";

                //closeAddEditPanel();
            }
        }


        public void EditScreen_Close(FileExplorerWindow.UrlLocation newDataItem)
        {
            ListViewItem listviewitem = FindListViewByIndex(newDataItem.Index);

            if (listviewitem == null || (int)listviewitem.Tag == newDataItem.Index)
            {
                customEditor.editScreen = null;
            }
            /*
            else
            {
                throw new NotImplementedException("Wowza");
            }
            */

            if (listviewitem != null)
            {
                listviewitem.Checked = false;

            }
            else
            {
                //closeAddEditPanel();
                itemChecked();
            }
        }

        public void AddButton_Click(string errorMessage, int Max, string Title)
        {
            if (customEditor.listView1.CheckedItems.Count == 0)
            {
                if (Max >= 0 && customEditor.listView1.Items.Count >= Max)
                {
                    MessageBoxButtons button = MessageBoxButtons.OK;
                    MessageBoxIcon icon = MessageBoxIcon.Warning;
                    MessageBox.Show(errorMessage, "ERROR", button, icon);
                }
                else
                {
                    EditScreen edit = new EditScreen(customEditor.customEditorClass, new FileExplorerWindow.UrlLocation(), -1, Title);

                    /*
                    if (customEditor.editScreen == null)
                    {
                        addAddEditPanel(edit, true);
                    }
                    else
                    {
                        closeAddEditPanel();
                    }
                    */

                    edit.Show();
                }
            }
            else
            {
                foreach (ListViewItem listviewItem in customEditor.listView1.CheckedItems)
                {
                    listviewItem.Checked = false;
                    EditScreen newEditScreen = new EditScreen(customEditor.customEditorClass, new FileExplorerWindow.UrlLocation(listviewItem), int.Parse(listviewItem.Name), Title);

                    newEditScreen.Show();
                }
            }
        }

        public void MoveButton_Click(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                foreach (ListViewItem checkedItem in customEditor.listView1.CheckedItems)
                {
                    int convertedInt = (int)checkedItem.Tag + (int)((Button)sender).Tag;

                    if (convertedInt >= 0 && convertedInt < customEditor.listView1.Items.Count)
                    {
                        foreach (ListViewItem iterateItem in customEditor.listView1.Items)
                        {
                            if ((int)iterateItem.Tag == convertedInt)
                            {
                                customEditor.swapListViewIndexes(checkedItem, iterateItem);

                                customEditor.OnNewTextChanged(new TextEventArgs(int.Parse(checkedItem.Name), new FileExplorerWindow.UrlLocation(checkedItem)));
                                customEditor.OnNewTextChanged(new TextEventArgs(int.Parse(iterateItem.Name), new FileExplorerWindow.UrlLocation(iterateItem)));

                                break;
                            }
                        }
                    }
                }
            }

            customEditor.listView1.Sort();
        }

        public void DeleteButton_Click()
        {
            List<int> list = new List<int>();
            foreach (ListViewItem listviewItem in customEditor.listView1.CheckedItems)
            {
                int name = (int)listviewItem.Tag;
                int leftovername;

                customEditor.OnNewTextChanged(new TextEventArgs(name, null));

                foreach (ListViewItem leftoverItems in customEditor.listView1.Items)
                {
                    leftovername = (int)leftoverItems.Tag;
                    if (leftovername > name)
                    {
                        customEditor.swapListViewIndexes(listviewItem, leftoverItems);

                        customEditor.OnNewTextChanged(new TextEventArgs(int.Parse(leftoverItems.Name), new FileExplorerWindow.UrlLocation(leftoverItems)));
                    }
                }

                customEditor.Remove(listviewItem);
            }

            itemChecked();
        }

        public void SaveButton_Click(object sender, EventArgs e)
        {
            List<FileExplorerWindow.UrlLocation> dataItems = new List<FileExplorerWindow.UrlLocation>();

            foreach (ListViewItem listviewitem in customEditor.listView1.Items)
            {
                listviewitem.Tag = listviewitem.Index;

                dataItems.Add(new FileExplorerWindow.UrlLocation(listviewitem));
            }

            customEditor.SaveCallback(dataItems, true);

            customEditor.OnNewTextChanged(new TextEventArgs(-1, null));

            MessageBox.Show("Settings Saved!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            customEditor.CloseScreen();

            /*
            new AutoClosingMessageBox("Saved!", 5000, MessageBoxButtons.OK, null, false, SystemIcons.Information).Show_StartTimer();
            */

            //customEditor.CloseScreen();
        }

        public void CloseButton_Click(object sender, EventArgs e)
        {
            customEditor.OnNewTextChanged(new TextEventArgs(-1, null));
            customEditor.CloseScreen();
        }


        public void listView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            customEditor.customEditorClass.DoDragDrop(e.Item, DragDropEffects.Move);
        }

        public void listView1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        public void listView1_DragDrop(object sender, DragEventArgs e)
        {
            IDataObject data = e.Data;
            ListViewItem startingListViewItem = (ListViewItem)data.GetData(typeof(ListViewItem));


            Point droppedPoint = new Point(e.X, e.Y);

            ListViewItem endingListViewItem = startingListViewItem;
            for (int i = 0; i < customEditor.listView1.Items.Count; i++)
            {
                int thisIndex = customEditor.listView1.PointToScreen(customEditor.listView1.Items[i].Position).Y + customEditor.listView1.Items[i].Bounds.Height;
                int nextIndex = customEditor.listView1.PointToScreen(endingListViewItem.Position).Y + endingListViewItem.Bounds.Height;

                if (Math.Abs(thisIndex - droppedPoint.Y) <= Math.Abs(nextIndex - droppedPoint.Y))
                {
                    endingListViewItem = customEditor.listView1.Items[i];
                }
            }

            if (!startingListViewItem.Equals(endingListViewItem))
            {
                swapListViewIndexes(startingListViewItem, endingListViewItem);

                customEditor.OnNewTextChanged(new TextEventArgs(int.Parse(startingListViewItem.Name), new FileExplorerWindow.UrlLocation(startingListViewItem)));
                customEditor.OnNewTextChanged(new TextEventArgs(int.Parse(endingListViewItem.Name), new FileExplorerWindow.UrlLocation(endingListViewItem)));

            }

            customEditor.listView1.Sort();
        }



        public void ColumnClick(object o, ColumnClickEventArgs e)
        {
            customEditor.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column);
        }

        public class ListViewItemComparer : IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }


        public void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            itemChecked();
        }
    }





    public class TextEventArgs : EventArgs
    {
        // Private field exposed by the Text property
        private FileExplorerWindow.UrlLocation text;
        private int index;

        public TextEventArgs(int index, FileExplorerWindow.UrlLocation text)
        {
            this.text = text;
            this.index = index;
        }

        // Read only property.
        public FileExplorerWindow.UrlLocation Text
        {
            get { return text; }
        }

        public int Index
        {
            get { return index; }
        }
    }
}
