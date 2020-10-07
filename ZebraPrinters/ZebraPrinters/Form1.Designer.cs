namespace ZebraPrinters
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
            this.cboEncoder = new System.Windows.Forms.ComboBox();
            this.cboPasPrinter = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblAfdeling = new System.Windows.Forms.Label();
            this.lblVervalLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVervaldatum = new System.Windows.Forms.Label();
            this.lblAanmaakdatum = new System.Windows.Forms.Label();
            this.lblPersnr = new System.Windows.Forms.Label();
            this.lblAchternaam = new System.Windows.Forms.Label();
            this.lblVoorletters = new System.Windows.Forms.Label();
            this.pictureBoxIdImage = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIdImage)).BeginInit();
            this.SuspendLayout();
            // 
            // cboEncoder
            // 
            this.cboEncoder.FormattingEnabled = true;
            this.cboEncoder.Location = new System.Drawing.Point(331, 29);
            this.cboEncoder.Name = "cboEncoder";
            this.cboEncoder.Size = new System.Drawing.Size(121, 21);
            this.cboEncoder.TabIndex = 44;
            // 
            // cboPasPrinter
            // 
            this.cboPasPrinter.FormattingEnabled = true;
            this.cboPasPrinter.Location = new System.Drawing.Point(89, 29);
            this.cboPasPrinter.Name = "cboPasPrinter";
            this.cboPasPrinter.Size = new System.Drawing.Size(121, 21);
            this.cboPasPrinter.TabIndex = 43;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblAfdeling);
            this.panel1.Controls.Add(this.lblVervalLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblVervaldatum);
            this.panel1.Controls.Add(this.lblAanmaakdatum);
            this.panel1.Controls.Add(this.lblPersnr);
            this.panel1.Controls.Add(this.lblAchternaam);
            this.panel1.Controls.Add(this.lblVoorletters);
            this.panel1.Controls.Add(this.pictureBoxIdImage);
            this.panel1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(21, 76);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(332, 200);
            this.panel1.TabIndex = 45;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(306, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 24);
            this.label2.TabIndex = 8;
            // 
            // lblAfdeling
            // 
            this.lblAfdeling.AutoSize = true;
            this.lblAfdeling.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAfdeling.Location = new System.Drawing.Point(25, 172);
            this.lblAfdeling.Name = "lblAfdeling";
            this.lblAfdeling.Size = new System.Drawing.Size(50, 18);
            this.lblAfdeling.TabIndex = 6;
            this.lblAfdeling.Text = "label3";
            // 
            // lblVervalLabel
            // 
            this.lblVervalLabel.AutoSize = true;
            this.lblVervalLabel.Location = new System.Drawing.Point(178, 155);
            this.lblVervalLabel.Name = "lblVervalLabel";
            this.lblVervalLabel.Size = new System.Drawing.Size(67, 14);
            this.lblVervalLabel.TabIndex = 4;
            this.lblVervalLabel.Text = "Vervaldatum";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(178, 141);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "Aanmaakdatum";
            // 
            // lblVervaldatum
            // 
            this.lblVervaldatum.AutoSize = true;
            this.lblVervaldatum.Location = new System.Drawing.Point(265, 155);
            this.lblVervaldatum.Name = "lblVervaldatum";
            this.lblVervaldatum.Size = new System.Drawing.Size(35, 14);
            this.lblVervaldatum.TabIndex = 4;
            this.lblVervaldatum.Text = "label1";
            // 
            // lblAanmaakdatum
            // 
            this.lblAanmaakdatum.AutoSize = true;
            this.lblAanmaakdatum.Location = new System.Drawing.Point(265, 141);
            this.lblAanmaakdatum.Name = "lblAanmaakdatum";
            this.lblAanmaakdatum.Size = new System.Drawing.Size(35, 14);
            this.lblAanmaakdatum.TabIndex = 4;
            this.lblAanmaakdatum.Text = "label1";
            // 
            // lblPersnr
            // 
            this.lblPersnr.AutoSize = true;
            this.lblPersnr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.lblPersnr.Location = new System.Drawing.Point(178, 123);
            this.lblPersnr.Name = "lblPersnr";
            this.lblPersnr.Size = new System.Drawing.Size(45, 16);
            this.lblPersnr.TabIndex = 4;
            this.lblPersnr.Text = "label1";
            // 
            // lblAchternaam
            // 
            this.lblAchternaam.AutoSize = true;
            this.lblAchternaam.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.lblAchternaam.Location = new System.Drawing.Point(178, 105);
            this.lblAchternaam.Name = "lblAchternaam";
            this.lblAchternaam.Size = new System.Drawing.Size(45, 16);
            this.lblAchternaam.TabIndex = 4;
            this.lblAchternaam.Text = "label1";
            // 
            // lblVoorletters
            // 
            this.lblVoorletters.AutoSize = true;
            this.lblVoorletters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoorletters.Location = new System.Drawing.Point(178, 87);
            this.lblVoorletters.Name = "lblVoorletters";
            this.lblVoorletters.Size = new System.Drawing.Size(45, 16);
            this.lblVoorletters.TabIndex = 4;
            this.lblVoorletters.Text = "label1";
            // 
            // pictureBoxIdImage
            // 
            this.pictureBoxIdImage.Image = global::ZebraPrinters.Properties.Resources.HappyBday;
            this.pictureBoxIdImage.ImageLocation = "";
            this.pictureBoxIdImage.Location = new System.Drawing.Point(23, 37);
            this.pictureBoxIdImage.Name = "pictureBoxIdImage";
            this.pictureBoxIdImage.Size = new System.Drawing.Size(115, 121);
            this.pictureBoxIdImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxIdImage.TabIndex = 3;
            this.pictureBoxIdImage.TabStop = false;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(491, 29);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(272, 247);
            this.button1.TabIndex = 46;
            this.button1.Text = "Print badge";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 47;
            this.label3.Text = "Printer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 47;
            this.label4.Text = "Encoder";
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 341);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.cboEncoder);
            this.Controls.Add(this.cboPasPrinter);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxIdImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboEncoder;
        private System.Windows.Forms.ComboBox cboPasPrinter;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblAfdeling;
        private System.Windows.Forms.Label lblVervalLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVervaldatum;
        private System.Windows.Forms.Label lblAanmaakdatum;
        private System.Windows.Forms.Label lblPersnr;
        private System.Windows.Forms.Label lblAchternaam;
        private System.Windows.Forms.Label lblVoorletters;
        private System.Windows.Forms.PictureBox pictureBoxIdImage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Drawing.Printing.PrintDocument printDocument1;
    }
}

