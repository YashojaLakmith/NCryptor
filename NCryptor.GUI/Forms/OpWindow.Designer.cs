namespace NCryptor.GUI.Forms
{
    partial class OpWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox_SelectedFiles = new System.Windows.Forms.ListBox();
            this.textBox_Key = new System.Windows.Forms.TextBox();
            this.btn_BrowseFiles = new System.Windows.Forms.Button();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.btn_Remove = new System.Windows.Forms.Button();
            this.btn_Start = new System.Windows.Forms.Button();
            this.label_Key = new System.Windows.Forms.Label();
            this.textbox_OutputDir = new System.Windows.Forms.TextBox();
            this.btn_BrowseOut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox_SelectedFiles
            // 
            this.listBox_SelectedFiles.FormattingEnabled = true;
            this.listBox_SelectedFiles.ItemHeight = 16;
            this.listBox_SelectedFiles.Location = new System.Drawing.Point(13, 23);
            this.listBox_SelectedFiles.Name = "listBox_SelectedFiles";
            this.listBox_SelectedFiles.Size = new System.Drawing.Size(307, 308);
            this.listBox_SelectedFiles.TabIndex = 0;
            // 
            // textBox_Key
            // 
            this.textBox_Key.Location = new System.Drawing.Point(12, 416);
            this.textBox_Key.Name = "textBox_Key";
            this.textBox_Key.PasswordChar = '*';
            this.textBox_Key.Size = new System.Drawing.Size(307, 22);
            this.textBox_Key.TabIndex = 1;
            this.textBox_Key.TextChanged += new System.EventHandler(this.TextBox_Key_OnTextChanged);
            // 
            // btn_BrowseFiles
            // 
            this.btn_BrowseFiles.Location = new System.Drawing.Point(327, 23);
            this.btn_BrowseFiles.Name = "btn_BrowseFiles";
            this.btn_BrowseFiles.Size = new System.Drawing.Size(96, 23);
            this.btn_BrowseFiles.TabIndex = 2;
            this.btn_BrowseFiles.Text = "Browse Files";
            this.btn_BrowseFiles.UseVisualStyleBackColor = true;
            this.btn_BrowseFiles.Click += new System.EventHandler(this.Btn_BrowseFiles_OnClick);
            // 
            // btn_Clear
            // 
            this.btn_Clear.Location = new System.Drawing.Point(326, 66);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(96, 23);
            this.btn_Clear.TabIndex = 2;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.Btn_Clear_OnClick);
            // 
            // btn_Remove
            // 
            this.btn_Remove.Location = new System.Drawing.Point(327, 113);
            this.btn_Remove.Name = "btn_Remove";
            this.btn_Remove.Size = new System.Drawing.Size(96, 23);
            this.btn_Remove.TabIndex = 2;
            this.btn_Remove.Text = "Remove";
            this.btn_Remove.UseVisualStyleBackColor = true;
            this.btn_Remove.Click += new System.EventHandler(this.Btn_Remove_OnClick);
            // 
            // btn_Start
            // 
            this.btn_Start.Location = new System.Drawing.Point(327, 416);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(96, 23);
            this.btn_Start.TabIndex = 3;
            this.btn_Start.Text = "Start";
            this.btn_Start.UseVisualStyleBackColor = true;
            // 
            // label_Key
            // 
            this.label_Key.AutoSize = true;
            this.label_Key.Location = new System.Drawing.Point(12, 397);
            this.label_Key.Name = "label_Key";
            this.label_Key.Size = new System.Drawing.Size(33, 16);
            this.label_Key.TabIndex = 4;
            this.label_Key.Text = "Key:";
            // 
            // textbox_OutputDir
            // 
            this.textbox_OutputDir.Location = new System.Drawing.Point(13, 372);
            this.textbox_OutputDir.Name = "textbox_OutputDir";
            this.textbox_OutputDir.Size = new System.Drawing.Size(307, 22);
            this.textbox_OutputDir.TabIndex = 1;
            // 
            // btn_BrowseOut
            // 
            this.btn_BrowseOut.Location = new System.Drawing.Point(326, 372);
            this.btn_BrowseOut.Name = "btn_BrowseOut";
            this.btn_BrowseOut.Size = new System.Drawing.Size(96, 23);
            this.btn_BrowseOut.TabIndex = 3;
            this.btn_BrowseOut.Text = "Browse";
            this.btn_BrowseOut.UseVisualStyleBackColor = true;

            this.btn_BrowseOut.Click += Btn_BrowseOut_OnClick;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 353);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Output directory";
            // 
            // OpWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_Key);
            this.Controls.Add(this.btn_BrowseOut);
            this.Controls.Add(this.btn_Start);
            this.Controls.Add(this.btn_Remove);
            this.Controls.Add(this.btn_Clear);
            this.Controls.Add(this.btn_BrowseFiles);
            this.Controls.Add(this.textbox_OutputDir);
            this.Controls.Add(this.textBox_Key);
            this.Controls.Add(this.listBox_SelectedFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "OpWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_SelectedFiles;
        protected System.Windows.Forms.TextBox textBox_Key;
        protected System.Windows.Forms.Button btn_BrowseFiles;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Button btn_Remove;
        private System.Windows.Forms.Button btn_Start;
        private System.Windows.Forms.Label label_Key;
        protected System.Windows.Forms.TextBox textbox_OutputDir;
        private System.Windows.Forms.Button btn_BrowseOut;
        private System.Windows.Forms.Label label1;
    }
}