using System;

namespace NCryptor.GUI.Forms
{
    partial class StatusWindow
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
                _cancellationTokenSource.Dispose();
                Array.Clear(_key, 0, _key.Length);
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
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.label_Status = new System.Windows.Forms.Label();
            this.listbox_Log = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Cancel.Location = new System.Drawing.Point(541, 335);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 29);
            this.btn_Cancel.TabIndex = 0;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_OnClick);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 341);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(504, 23);
            this.progressBar.TabIndex = 1;
            // 
            // label_Status
            // 
            this.label_Status.AutoSize = true;
            this.label_Status.Location = new System.Drawing.Point(13, 319);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(42, 16);
            this.label_Status.TabIndex = 3;
            this.label_Status.Text = "status";
            // 
            // listbox_Log
            // 
            this.listbox_Log.FormattingEnabled = true;
            this.listbox_Log.ItemHeight = 16;
            this.listbox_Log.Location = new System.Drawing.Point(13, 13);
            this.listbox_Log.Name = "listbox_Log";
            this.listbox_Log.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listbox_Log.Size = new System.Drawing.Size(603, 292);
            this.listbox_Log.TabIndex = 4;
            // 
            // StatusWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 376);
            this.Controls.Add(this.listbox_Log);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btn_Cancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "StatusWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StatusWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected System.Windows.Forms.ProgressBar progressBar;
        protected System.Windows.Forms.ListBox listbox_Log;
        protected System.Windows.Forms.Label label_Status;
        protected System.Windows.Forms.Button btn_Cancel;
    }
}