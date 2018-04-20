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
    public partial class EditScreen : Form
    {
        public FileExplorerWindow.UrlLocation dataItem = null;
        //public SettingsWindow.CustomEditor customEditor = null;

        public CustomEditorClass customEditorClass = null;
        //public EditScreen<T> editscreen = null;
        //ListView.ListViewItemCollection searchProviderList = ManagerTools.managerTools.GetListView().Items;


        private void initializeComponent(Type type)
        {
                if (type == typeof(SettingsWindow))
                {
                    InitializeComponent();
                }
                else
                    throw new Exception("WTF!");
        }

        public EditScreen(CustomEditorClass customEditorClass, FileExplorerWindow.UrlLocation customClass, int index, string name)
        {
            this.Tag = index;
            //this.customEditorClass = customeditorclass;

            this.customEditorClass = customEditorClass;
            initializeComponent(customEditorClass.customEditor.GetType());

            RegisterEvents();

            SetData(customClass, name);
        }

        private void RegisterEvents()
        {
            this.Load += EditScreen_Load;
            this.FormClosed += EditScreen_FormClosed;
            customEditorClass.customEditor.NewTextChanged += new EventHandler<TextEventArgs>(form2_NewTextChanged);
        }

        void EditScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            customEditorClass.EditScreen_Close(dataItem);

            customEditorClass.customEditor.NewTextChanged -= form2_NewTextChanged;
        }

        private void form2_NewTextChanged(object sender, TextEventArgs e)
        {
            if ( e.Text == null && (e.Index == -1 || e.Index == (int)this.IndexTextBox.Tag ))
            {
                this.Close();
            }
            else if (e.Text != null && ((int)e.Index == (int)this.Tag ))
            {
                SetData(e.Text, "Edit");
            }
        }

        void EditScreen_Load(object sender, System.EventArgs e)
        {
            //editscreen = this;
        }

        public string GetBaseAddressTextBoxText()
        {
            return BaseAddressTextBox.Text;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            int index = int.Parse(IndexTextBox.Text);

            ListView.ListViewItemCollection searchProviderList = new ListView.ListViewItemCollection(customEditorClass.GetListViewClone());
            
            foreach(ListViewItem listviewitem in searchProviderList)
            {
                if((int)listviewitem.Tag == dataItem.Index)
                {
                    listviewitem.Tag = index;
                    break;
                }
            }

            if (index >= 0 && index <= searchProviderList.Count)
            {
                if (customEditorClass.CheckConfiguredIndexes(searchProviderList))
                    if (NameTextBox.Text != "" && BaseAddressTextBox.Text != "" && URLTextBox.Text != "")
                    {
                        customEditorClass.customEditor.EditScreen_Save(new FileExplorerWindow.UrlLocation( NameTextBox.Text, index, customEditorClass.customEditor.GetCustom(this), URLTextBox.Text), (int)this.Tag);
                        this.Close();
                    }
                    else
                    {
                        MessageBoxButtons button = MessageBoxButtons.OK;
                        MessageBoxIcon icon = MessageBoxIcon.Warning;
                        MessageBox.Show("Name, Base Address, and URL Can't be Empty!", "ERROR", button, icon);
                    }
            }
            else
            {
                MessageBoxButtons button = MessageBoxButtons.OK;
                MessageBoxIcon icon = MessageBoxIcon.Warning;
                MessageBox.Show("Index is Invalid!", "ERROR", button, icon); 
            }
        
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
