namespace PdfParser
{
    partial class Form1
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
            this.txtPDFFile = new System.Windows.Forms.TextBox();
            this.btnFileBrowser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.txtAnnotatedText = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkAnnotation = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAnnotate2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtPDFFile
            // 
            this.txtPDFFile.Location = new System.Drawing.Point(16, 40);
            this.txtPDFFile.Name = "txtPDFFile";
            this.txtPDFFile.Size = new System.Drawing.Size(352, 20);
            this.txtPDFFile.TabIndex = 0;
            // 
            // btnFileBrowser
            // 
            this.btnFileBrowser.Location = new System.Drawing.Point(374, 38);
            this.btnFileBrowser.Name = "btnFileBrowser";
            this.btnFileBrowser.Size = new System.Drawing.Size(141, 23);
            this.btnFileBrowser.TabIndex = 1;
            this.btnFileBrowser.Text = "Browse";
            this.btnFileBrowser.UseVisualStyleBackColor = true;
            this.btnFileBrowser.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select PDF File";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 178);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(499, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Highlight";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtAnnotatedText
            // 
            this.txtAnnotatedText.Location = new System.Drawing.Point(16, 93);
            this.txtAnnotatedText.Name = "txtAnnotatedText";
            this.txtAnnotatedText.Size = new System.Drawing.Size(352, 20);
            this.txtAnnotatedText.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Text to Annotate";
            // 
            // chkAnnotation
            // 
            this.chkAnnotation.AutoSize = true;
            this.chkAnnotation.Location = new System.Drawing.Point(373, 93);
            this.chkAnnotation.Name = "chkAnnotation";
            this.chkAnnotation.Size = new System.Drawing.Size(142, 17);
            this.chkAnnotation.TabIndex = 6;
            this.chkAnnotation.Text = "Annotate All occurances";
            this.chkAnnotation.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Text to Annotate";
            // 
            // txtAnnotate2
            // 
            this.txtAnnotate2.Location = new System.Drawing.Point(16, 146);
            this.txtAnnotate2.Name = "txtAnnotate2";
            this.txtAnnotate2.Size = new System.Drawing.Size(352, 20);
            this.txtAnnotate2.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 213);
            this.Controls.Add(this.chkAnnotation);
            this.Controls.Add(this.txtAnnotate2);
            this.Controls.Add(this.txtAnnotatedText);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFileBrowser);
            this.Controls.Add(this.txtPDFFile);
            this.Name = "Form1";
            this.Text = "Data Comparer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtPDFFile;
        private System.Windows.Forms.Button btnFileBrowser;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtAnnotatedText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkAnnotation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAnnotate2;
    }
}

