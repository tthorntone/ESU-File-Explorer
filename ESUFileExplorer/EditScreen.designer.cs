using System.Collections.Generic;
using System.Windows.Forms;

namespace ESUFileExplorer
{
    partial class EditScreen : Form
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

        public bool SetData(FileExplorerWindow.UrlLocation dataItems, string name)
        {
            this.dataItem = dataItems;
            this.Text = name + " Screen";

            NameTextBox.Text = dataItems.Name;
            BaseAddressTextBox.Text = dataItems.BaseAddress;
            URLTextBox.Text = dataItems.Url;
   

            if (dataItems.Index >= 0)
            {
                IndexTextBox.Text = dataItems.Index.ToString();
                IndexTextBox.Tag = dataItems.Index;

                return true;
            }
            else
            {
                IndexTextBox.Text = (customEditorClass.GetListViewSize()).ToString();
                IndexTextBox.Tag = customEditorClass.GetListViewSize();

                return false;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.NameLabel = new System.Windows.Forms.Label();
            this.BaseAddressLabel = new System.Windows.Forms.Label();
            this.IndexLabel = new System.Windows.Forms.Label();
            this.URLLabel = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.URLTextBox = new System.Windows.Forms.TextBox();
            this.BaseAddressTextBox = new System.Windows.Forms.TextBox();
            this.IndexTextBox = new System.Windows.Forms.TextBox();
            this.SaveButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(4, 16);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(49, 17);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name:";
            // 
            // BaseAddressLabel
            // 
            this.BaseAddressLabel.AutoSize = true;
            this.BaseAddressLabel.Location = new System.Drawing.Point(4, 75);
            this.BaseAddressLabel.Name = "BaseAddressLabel";
            this.BaseAddressLabel.Size = new System.Drawing.Size(100, 17);
            this.BaseAddressLabel.TabIndex = 1;
            this.BaseAddressLabel.Text = "Base Address:";
            // 
            // IndexLabel
            // 
            this.IndexLabel.AutoSize = true;
            this.IndexLabel.Location = new System.Drawing.Point(4, 47);
            this.IndexLabel.Name = "IndexLabel";
            this.IndexLabel.Size = new System.Drawing.Size(45, 17);
            this.IndexLabel.TabIndex = 2;
            this.IndexLabel.Text = "Index:";
            // 
            // URLLabel
            // 
            this.URLLabel.AutoSize = true;
            this.URLLabel.Location = new System.Drawing.Point(4, 103);
            this.URLLabel.Name = "URLLabel";
            this.URLLabel.Size = new System.Drawing.Size(40, 17);
            this.URLLabel.TabIndex = 3;
            this.URLLabel.Text = "URL:";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.Location = new System.Drawing.Point(110, 13);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(197, 22);
            this.NameTextBox.TabIndex = 4;
            // 
            // URLTextBox
            // 
            this.URLTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.URLTextBox.Location = new System.Drawing.Point(110, 100);
            this.URLTextBox.Name = "URLTextBox";
            this.URLTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.URLTextBox.Size = new System.Drawing.Size(197, 22);
            this.URLTextBox.TabIndex = 5;
            // 
            // BaseAddressTextBox
            // 
            this.BaseAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BaseAddressTextBox.Location = new System.Drawing.Point(110, 72);
            this.BaseAddressTextBox.Name = "BaseAddressTextBox";
            this.BaseAddressTextBox.Size = new System.Drawing.Size(197, 22);
            this.BaseAddressTextBox.TabIndex = 6;
            // 
            // IndexTextBox
            // 
            this.IndexTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IndexTextBox.Location = new System.Drawing.Point(110, 44);
            this.IndexTextBox.Name = "IndexTextBox";
            this.IndexTextBox.Size = new System.Drawing.Size(197, 22);
            this.IndexTextBox.TabIndex = 7;
            // 
            // SaveButton
            // 
            this.SaveButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.SaveButton.Location = new System.Drawing.Point(72, 218);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ExitButton.Location = new System.Drawing.Point(153, 218);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 9;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // EditScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 253);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.IndexTextBox);
            this.Controls.Add(this.BaseAddressTextBox);
            this.Controls.Add(this.URLTextBox);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.URLLabel);
            this.Controls.Add(this.IndexLabel);
            this.Controls.Add(this.BaseAddressLabel);
            this.Controls.Add(this.NameLabel);
            this.MinimumSize = new System.Drawing.Size(18, 208);
            this.Name = "EditScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label BaseAddressLabel;
        private System.Windows.Forms.Label IndexLabel;
        private System.Windows.Forms.Label URLLabel;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.TextBox URLTextBox;
        private System.Windows.Forms.TextBox BaseAddressTextBox;
        private System.Windows.Forms.TextBox IndexTextBox;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Button ExitButton;
    }
}