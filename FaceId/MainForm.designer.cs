namespace FaceId
{
    partial class MainForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.trainButton = new System.Windows.Forms.Button();
            this.deletePersonButton = new System.Windows.Forms.Button();
            this.addPersonButton = new System.Windows.Forms.Button();
            this.deletePersonGroupButton = new System.Windows.Forms.Button();
            this.addPersonGroupButton = new System.Windows.Forms.Button();
            this.addFromDirButton = new System.Windows.Forms.Button();
            this.deleteFacesButton = new System.Windows.Forms.Button();
            this.addFromFileButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.personSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.personGroupSelector = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picLabel = new System.Windows.Forms.Label();
            this.imgInfoBox = new System.Windows.Forms.TextBox();
            this.imgBox = new System.Windows.Forms.PictureBox();
            this.identifyButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.prevButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.configButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.configButton);
            this.panel1.Controls.Add(this.trainButton);
            this.panel1.Controls.Add(this.deletePersonButton);
            this.panel1.Controls.Add(this.addPersonButton);
            this.panel1.Controls.Add(this.deletePersonGroupButton);
            this.panel1.Controls.Add(this.addPersonGroupButton);
            this.panel1.Controls.Add(this.addFromDirButton);
            this.panel1.Controls.Add(this.deleteFacesButton);
            this.panel1.Controls.Add(this.addFromFileButton);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.personSelector);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.personGroupSelector);
            this.panel1.Location = new System.Drawing.Point(13, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(814, 100);
            this.panel1.TabIndex = 0;
            // 
            // trainButton
            // 
            this.trainButton.Location = new System.Drawing.Point(383, 6);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(75, 23);
            this.trainButton.TabIndex = 13;
            this.trainButton.Text = "Train";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.trainButton_Click);
            // 
            // deletePersonButton
            // 
            this.deletePersonButton.Location = new System.Drawing.Point(302, 35);
            this.deletePersonButton.Name = "deletePersonButton";
            this.deletePersonButton.Size = new System.Drawing.Size(75, 23);
            this.deletePersonButton.TabIndex = 12;
            this.deletePersonButton.Text = "Delete";
            this.deletePersonButton.UseVisualStyleBackColor = true;
            this.deletePersonButton.Click += new System.EventHandler(this.deletePersonButton_Click);
            // 
            // addPersonButton
            // 
            this.addPersonButton.Location = new System.Drawing.Point(221, 35);
            this.addPersonButton.Name = "addPersonButton";
            this.addPersonButton.Size = new System.Drawing.Size(75, 23);
            this.addPersonButton.TabIndex = 11;
            this.addPersonButton.Text = "Add New";
            this.addPersonButton.UseVisualStyleBackColor = true;
            this.addPersonButton.Click += new System.EventHandler(this.addPersonButton_Click);
            // 
            // deletePersonGroupButton
            // 
            this.deletePersonGroupButton.Location = new System.Drawing.Point(302, 6);
            this.deletePersonGroupButton.Name = "deletePersonGroupButton";
            this.deletePersonGroupButton.Size = new System.Drawing.Size(75, 23);
            this.deletePersonGroupButton.TabIndex = 10;
            this.deletePersonGroupButton.Text = "Delete";
            this.deletePersonGroupButton.UseVisualStyleBackColor = true;
            this.deletePersonGroupButton.Click += new System.EventHandler(this.deletePersonGroupButton_Click);
            // 
            // addPersonGroupButton
            // 
            this.addPersonGroupButton.Location = new System.Drawing.Point(221, 6);
            this.addPersonGroupButton.Name = "addPersonGroupButton";
            this.addPersonGroupButton.Size = new System.Drawing.Size(75, 23);
            this.addPersonGroupButton.TabIndex = 9;
            this.addPersonGroupButton.Text = "Add New";
            this.addPersonGroupButton.UseVisualStyleBackColor = true;
            this.addPersonGroupButton.Click += new System.EventHandler(this.addPersonGroupButton_Click);
            // 
            // addFromDirButton
            // 
            this.addFromDirButton.Location = new System.Drawing.Point(163, 64);
            this.addFromDirButton.Name = "addFromDirButton";
            this.addFromDirButton.Size = new System.Drawing.Size(115, 23);
            this.addFromDirButton.TabIndex = 8;
            this.addFromDirButton.Text = "Add from directory";
            this.addFromDirButton.UseVisualStyleBackColor = true;
            // 
            // deleteFacesButton
            // 
            this.deleteFacesButton.Location = new System.Drawing.Point(284, 64);
            this.deleteFacesButton.Name = "deleteFacesButton";
            this.deleteFacesButton.Size = new System.Drawing.Size(75, 23);
            this.deleteFacesButton.TabIndex = 7;
            this.deleteFacesButton.Text = "Delete All";
            this.deleteFacesButton.UseVisualStyleBackColor = true;
            this.deleteFacesButton.Click += new System.EventHandler(this.deleteFacesButton_Click);
            // 
            // addFromFileButton
            // 
            this.addFromFileButton.Location = new System.Drawing.Point(82, 64);
            this.addFromFileButton.Name = "addFromFileButton";
            this.addFromFileButton.Size = new System.Drawing.Size(75, 23);
            this.addFromFileButton.TabIndex = 6;
            this.addFromFileButton.Text = "Add from file";
            this.addFromFileButton.UseVisualStyleBackColor = true;
            this.addFromFileButton.Click += new System.EventHandler(this.faceUploadButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Add Face";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Person";
            // 
            // personSelector
            // 
            this.personSelector.FormattingEnabled = true;
            this.personSelector.Location = new System.Drawing.Point(82, 35);
            this.personSelector.Name = "personSelector";
            this.personSelector.Size = new System.Drawing.Size(121, 21);
            this.personSelector.TabIndex = 2;
            this.personSelector.SelectedIndexChanged += new System.EventHandler(this.personSelector_SelectedIndexChanged);
            this.personSelector.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.personSelector_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Person Group";
            // 
            // personGroupSelector
            // 
            this.personGroupSelector.FormattingEnabled = true;
            this.personGroupSelector.Location = new System.Drawing.Point(82, 8);
            this.personGroupSelector.Name = "personGroupSelector";
            this.personGroupSelector.Size = new System.Drawing.Size(121, 21);
            this.personGroupSelector.TabIndex = 0;
            this.personGroupSelector.SelectedIndexChanged += new System.EventHandler(this.personGroupSelector_SelectedIndexChanged);
            this.personGroupSelector.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.personGroupSelector_KeyPress);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.picLabel);
            this.panel2.Controls.Add(this.imgInfoBox);
            this.panel2.Controls.Add(this.imgBox);
            this.panel2.Location = new System.Drawing.Point(13, 120);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(814, 425);
            this.panel2.TabIndex = 1;
            // 
            // picLabel
            // 
            this.picLabel.AutoSize = true;
            this.picLabel.Location = new System.Drawing.Point(192, 407);
            this.picLabel.Name = "picLabel";
            this.picLabel.Size = new System.Drawing.Size(0, 13);
            this.picLabel.TabIndex = 3;
            // 
            // imgInfoBox
            // 
            this.imgInfoBox.Location = new System.Drawing.Point(413, 4);
            this.imgInfoBox.Multiline = true;
            this.imgInfoBox.Name = "imgInfoBox";
            this.imgInfoBox.Size = new System.Drawing.Size(398, 106);
            this.imgInfoBox.TabIndex = 2;
            // 
            // imgBox
            // 
            this.imgBox.Location = new System.Drawing.Point(7, 4);
            this.imgBox.Name = "imgBox";
            this.imgBox.Size = new System.Drawing.Size(400, 400);
            this.imgBox.TabIndex = 1;
            this.imgBox.TabStop = false;
            this.imgBox.Click += new System.EventHandler(this.imgBox_Click);
            this.imgBox.Paint += new System.Windows.Forms.PaintEventHandler(this.imgBox_Paint);
            // 
            // identifyButton
            // 
            this.identifyButton.Location = new System.Drawing.Point(750, 568);
            this.identifyButton.Name = "identifyButton";
            this.identifyButton.Size = new System.Drawing.Size(75, 23);
            this.identifyButton.TabIndex = 0;
            this.identifyButton.Text = "Open...";
            this.identifyButton.UseVisualStyleBackColor = true;
            this.identifyButton.Click += new System.EventHandler(this.identifyButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            this.openFileDialog.Multiselect = true;
            // 
            // prevButton
            // 
            this.prevButton.Location = new System.Drawing.Point(53, 568);
            this.prevButton.Name = "prevButton";
            this.prevButton.Size = new System.Drawing.Size(75, 23);
            this.prevButton.TabIndex = 2;
            this.prevButton.Text = "Previous";
            this.prevButton.UseVisualStyleBackColor = true;
            this.prevButton.Click += new System.EventHandler(this.prevButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(298, 568);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 3;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(720, 8);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(75, 23);
            this.configButton.TabIndex = 14;
            this.configButton.Text = "Configure...";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 603);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.prevButton);
            this.Controls.Add(this.identifyButton);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MainForm";
            this.Text = "Face Id";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button addFromFileButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox personSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox personGroupSelector;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button deleteFacesButton;
        private System.Windows.Forms.Button addPersonGroupButton;
        private System.Windows.Forms.Button addFromDirButton;
        private System.Windows.Forms.Button deletePersonGroupButton;
        private System.Windows.Forms.Button deletePersonButton;
        private System.Windows.Forms.Button addPersonButton;
        private System.Windows.Forms.Button trainButton;
        private System.Windows.Forms.Button identifyButton;
        private System.Windows.Forms.PictureBox imgBox;
        private System.Windows.Forms.TextBox imgInfoBox;
        private System.Windows.Forms.Button prevButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Label picLabel;
        private System.Windows.Forms.Button configButton;
    }
}

