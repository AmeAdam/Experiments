namespace WindowsFormsApplication1
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
            this.tbPrProj = new System.Windows.Forms.TextBox();
            this.tbXml = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.btnAnalizuj = new System.Windows.Forms.Button();
            this.cbEffectName = new System.Windows.Forms.ComboBox();
            this.btnDisableEffect = new System.Windows.Forms.Button();
            this.btnEnableEffect = new System.Windows.Forms.Button();
            this.bRemoveEffect = new System.Windows.Forms.Button();
            this.bSetTop = new System.Windows.Forms.Button();
            this.cbSequences = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // tbPrProj
            // 
            this.tbPrProj.Location = new System.Drawing.Point(26, 38);
            this.tbPrProj.Name = "tbPrProj";
            this.tbPrProj.Size = new System.Drawing.Size(292, 20);
            this.tbPrProj.TabIndex = 0;
            // 
            // tbXml
            // 
            this.tbXml.Location = new System.Drawing.Point(353, 38);
            this.tbXml.Name = "tbXml";
            this.tbXml.Size = new System.Drawing.Size(310, 20);
            this.tbXml.TabIndex = 1;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(687, 36);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "SelectFile";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.BtnSelectFileClick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "pliki";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(26, 76);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(292, 186);
            this.listBox1.TabIndex = 7;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(353, 76);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(310, 186);
            this.listBox2.TabIndex = 8;
            // 
            // btnAnalizuj
            // 
            this.btnAnalizuj.Location = new System.Drawing.Point(687, 88);
            this.btnAnalizuj.Name = "btnAnalizuj";
            this.btnAnalizuj.Size = new System.Drawing.Size(75, 23);
            this.btnAnalizuj.TabIndex = 9;
            this.btnAnalizuj.Text = "Run";
            this.btnAnalizuj.UseVisualStyleBackColor = true;
            this.btnAnalizuj.Click += new System.EventHandler(this.BtnAnalizujClick);
            // 
            // cbEffectName
            // 
            this.cbEffectName.FormattingEnabled = true;
            this.cbEffectName.Items.AddRange(new object[] {
            "Reduce noise",
            "Denoiser II"});
            this.cbEffectName.Location = new System.Drawing.Point(26, 288);
            this.cbEffectName.Name = "cbEffectName";
            this.cbEffectName.Size = new System.Drawing.Size(292, 21);
            this.cbEffectName.TabIndex = 10;
            // 
            // btnDisableEffect
            // 
            this.btnDisableEffect.Location = new System.Drawing.Point(339, 286);
            this.btnDisableEffect.Name = "btnDisableEffect";
            this.btnDisableEffect.Size = new System.Drawing.Size(75, 23);
            this.btnDisableEffect.TabIndex = 11;
            this.btnDisableEffect.Text = "Dissable";
            this.btnDisableEffect.UseVisualStyleBackColor = true;
            this.btnDisableEffect.Click += new System.EventHandler(this.BtnDisableEffectClick);
            // 
            // btnEnableEffect
            // 
            this.btnEnableEffect.Location = new System.Drawing.Point(526, 286);
            this.btnEnableEffect.Name = "btnEnableEffect";
            this.btnEnableEffect.Size = new System.Drawing.Size(75, 23);
            this.btnEnableEffect.TabIndex = 12;
            this.btnEnableEffect.Text = "Enable";
            this.btnEnableEffect.UseVisualStyleBackColor = true;
            this.btnEnableEffect.Click += new System.EventHandler(this.BtnEnableEffectClick);
            // 
            // bRemoveEffect
            // 
            this.bRemoveEffect.Location = new System.Drawing.Point(435, 286);
            this.bRemoveEffect.Name = "bRemoveEffect";
            this.bRemoveEffect.Size = new System.Drawing.Size(75, 23);
            this.bRemoveEffect.TabIndex = 13;
            this.bRemoveEffect.Text = "Remove";
            this.bRemoveEffect.UseVisualStyleBackColor = true;
            this.bRemoveEffect.Click += new System.EventHandler(this.bRemoveEffect_Click);
            // 
            // bSetTop
            // 
            this.bSetTop.Location = new System.Drawing.Point(617, 286);
            this.bSetTop.Name = "bSetTop";
            this.bSetTop.Size = new System.Drawing.Size(75, 23);
            this.bSetTop.TabIndex = 14;
            this.bSetTop.Text = "Set Top";
            this.bSetTop.UseVisualStyleBackColor = true;
            this.bSetTop.Click += new System.EventHandler(this.bSetTop_Click);
            // 
            // cbSequences
            // 
            this.cbSequences.FormattingEnabled = true;
            this.cbSequences.Items.AddRange(new object[] {
            "Reduce noise",
            "Denoiser II"});
            this.cbSequences.Location = new System.Drawing.Point(26, 315);
            this.cbSequences.Name = "cbSequences";
            this.cbSequences.Size = new System.Drawing.Size(292, 21);
            this.cbSequences.TabIndex = 15;
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(790, 341);
            this.Controls.Add(this.cbSequences);
            this.Controls.Add(this.bSetTop);
            this.Controls.Add(this.bRemoveEffect);
            this.Controls.Add(this.btnEnableEffect);
            this.Controls.Add(this.btnDisableEffect);
            this.Controls.Add(this.cbEffectName);
            this.Controls.Add(this.btnAnalizuj);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.tbXml);
            this.Controls.Add(this.tbPrProj);
            this.Name = "Form1";
            this.Text = "Form1";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPrProj;
        private System.Windows.Forms.TextBox tbXml;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button btnAnalizuj;
        private System.Windows.Forms.ComboBox cbEffectName;
        private System.Windows.Forms.Button btnDisableEffect;
        private System.Windows.Forms.Button btnEnableEffect;
        private System.Windows.Forms.Button bRemoveEffect;
        private System.Windows.Forms.Button bSetTop;
        private System.Windows.Forms.ComboBox cbSequences;
    }
}

