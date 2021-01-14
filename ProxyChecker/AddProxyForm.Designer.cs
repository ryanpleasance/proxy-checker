namespace ProxyChecker
{
    partial class AddProxyForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.proxyTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Corbel", 12F);
            this.btnOk.Location = new System.Drawing.Point(363, 381);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(192, 43);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Add";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // proxyTextBox
            // 
            this.proxyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.proxyTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.proxyTextBox.ForeColor = System.Drawing.Color.DimGray;
            this.proxyTextBox.Location = new System.Drawing.Point(12, 12);
            this.proxyTextBox.Name = "proxyTextBox";
            this.proxyTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.proxyTextBox.Size = new System.Drawing.Size(543, 363);
            this.proxyTextBox.TabIndex = 3;
            this.proxyTextBox.Text = "IP:PORT:USERNAME:PASSWORD";
            this.proxyTextBox.WordWrap = false;
            this.proxyTextBox.Enter += new System.EventHandler(this.proxyTextBox_Enter);
            this.proxyTextBox.Leave += new System.EventHandler(this.proxyTextBox_Leave);
            // 
            // AddProxyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 430);
            this.Controls.Add(this.proxyTextBox);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "AddProxyForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Proxy";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RichTextBox proxyTextBox;
    }
}