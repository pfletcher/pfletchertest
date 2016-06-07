namespace mdtodita
{
    partial class MdToDitaForm
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
            this.richTextConsole = new System.Windows.Forms.RichTextBox();
            this.buttonReadFiles = new System.Windows.Forms.Button();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextConsole
            // 
            this.richTextConsole.Location = new System.Drawing.Point(13, 90);
            this.richTextConsole.Name = "richTextConsole";
            this.richTextConsole.Size = new System.Drawing.Size(259, 160);
            this.richTextConsole.TabIndex = 0;
            this.richTextConsole.Text = "";
            // 
            // buttonReadFiles
            // 
            this.buttonReadFiles.Location = new System.Drawing.Point(13, 13);
            this.buttonReadFiles.Name = "buttonReadFiles";
            this.buttonReadFiles.Size = new System.Drawing.Size(259, 23);
            this.buttonReadFiles.TabIndex = 1;
            this.buttonReadFiles.Text = "Read files.xml";
            this.buttonReadFiles.UseVisualStyleBackColor = true;
            this.buttonReadFiles.Click += new System.EventHandler(this.buttonReadFiles_Click);
            // 
            // buttonConvert
            // 
            this.buttonConvert.Location = new System.Drawing.Point(13, 43);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(259, 23);
            this.buttonConvert.TabIndex = 2;
            this.buttonConvert.Text = "Convert to DITA";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // MdToDitaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.buttonReadFiles);
            this.Controls.Add(this.richTextConsole);
            this.Name = "MdToDitaForm";
            this.Text = "MD to Dita";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextConsole;
        private System.Windows.Forms.Button buttonReadFiles;
        private System.Windows.Forms.Button buttonConvert;
    }
}

