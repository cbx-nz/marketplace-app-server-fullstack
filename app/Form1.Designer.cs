using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace app
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
        /// Required method for Designer support.
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBoxProducts = new TreeView();
            buttonMinusOne = new Button();
            buttonPlusOne = new Button();
            imageBoxLoadImage = new PictureBox();
            buttonLoadImage = new Button();
            buttonLogPurchase = new Button();
            textReadProductTitle = new TextBox();
            buttonLoadProducts = new Button();
            textBoxQuantity = new TextBox();
            textBoxEnterInformation = new TextBox();
            textBoxEnterEncryptionCode = new TextBox();
            buttonUseEncryptionCode = new Button();

            ((System.ComponentModel.ISupportInitialize)imageBoxLoadImage).BeginInit();
            SuspendLayout();

            // 
            // listBoxProducts
            // 
            listBoxProducts.Location = new Point(12, 45);
            listBoxProducts.Name = "listBoxProducts";
            listBoxProducts.Size = new Size(163, 226);
            listBoxProducts.TabIndex = 0;

            // 
            // buttonMinusOne
            // 
            buttonMinusOne.Location = new Point(412, 10);
            buttonMinusOne.Name = "buttonMinusOne";
            buttonMinusOne.Size = new Size(80, 29);
            buttonMinusOne.TabIndex = 2;
            buttonMinusOne.Text = "👎";
            buttonMinusOne.UseVisualStyleBackColor = true;

            // 
            // buttonPlusOne
            // 
            buttonPlusOne.Location = new Point(329, 10);
            buttonPlusOne.Name = "buttonPlusOne";
            buttonPlusOne.Size = new Size(77, 29);
            buttonPlusOne.TabIndex = 3;
            buttonPlusOne.Text = "👍";
            buttonPlusOne.UseVisualStyleBackColor = true;

            // 
            // imageBoxLoadImage
            // 
            imageBoxLoadImage.Location = new Point(181, 45);
            imageBoxLoadImage.Name = "imageBoxLoadImage";
            imageBoxLoadImage.Size = new Size(311, 226);
            imageBoxLoadImage.SizeMode = PictureBoxSizeMode.Zoom;
            imageBoxLoadImage.TabIndex = 4;
            imageBoxLoadImage.TabStop = false;

            // 
            // buttonLoadImage
            // 
            buttonLoadImage.Location = new Point(181, 10);
            buttonLoadImage.Name = "buttonLoadImage";
            buttonLoadImage.Size = new Size(142, 29);
            buttonLoadImage.TabIndex = 5;
            buttonLoadImage.Text = "Load Image";
            buttonLoadImage.UseVisualStyleBackColor = true;

            // 
            // buttonLogPurchase
            // 
            buttonLogPurchase.Location = new Point(181, 310);
            buttonLogPurchase.Name = "buttonLogPurchase";
            buttonLogPurchase.Size = new Size(311, 29);
            buttonLogPurchase.TabIndex = 6;
            buttonLogPurchase.Text = "Log Purchase";
            buttonLogPurchase.UseVisualStyleBackColor = true;

            // 
            // textReadProductTitle
            // 
            textReadProductTitle.Location = new Point(12, 277);
            textReadProductTitle.Name = "textReadProductTitle";
            textReadProductTitle.ReadOnly = true;
            textReadProductTitle.Size = new Size(163, 27);
            textReadProductTitle.TabIndex = 7;
            textReadProductTitle.Text = "Choose a Product";

            // 
            // buttonLoadProducts
            // 
            buttonLoadProducts.Location = new Point(12, 10);
            buttonLoadProducts.Name = "buttonLoadProducts";
            buttonLoadProducts.Size = new Size(163, 29);
            buttonLoadProducts.TabIndex = 8;
            buttonLoadProducts.Text = "Load Products";
            buttonLoadProducts.UseVisualStyleBackColor = true;

            // 
            // textBoxQuantity
            // 
            textBoxQuantity.Location = new Point(12, 310);
            textBoxQuantity.Name = "textBoxQuantity";
            textBoxQuantity.Size = new Size(163, 27);
            textBoxQuantity.TabIndex = 9;
            textBoxQuantity.Text = "1";

            // 
            // textBoxEnterInformation
            // 
            textBoxEnterInformation.Location = new Point(181, 277);
            textBoxEnterInformation.Multiline = true;
            textBoxEnterInformation.Name = "textBoxEnterInformation";
            textBoxEnterInformation.ScrollBars = ScrollBars.Vertical;
            textBoxEnterInformation.Size = new Size(311, 27);
            textBoxEnterInformation.TabIndex = 10;
            textBoxEnterInformation.Text = "";

            // 
            // textBoxEnterEncryptionCode
            // 
            textBoxEnterEncryptionCode.Location = new Point(12, 343);
            textBoxEnterEncryptionCode.Name = "textBoxEnterEncryptionCode";
            textBoxEnterEncryptionCode.Size = new Size(380, 27);
            textBoxEnterEncryptionCode.TabIndex = 11;
            textBoxEnterEncryptionCode.UseSystemPasswordChar = true;

            // 
            // buttonUseEncryptionCode
            // 
            buttonUseEncryptionCode.Location = new Point(398, 341);
            buttonUseEncryptionCode.Name = "buttonUseEncryptionCode";
            buttonUseEncryptionCode.Size = new Size(94, 29);
            buttonUseEncryptionCode.TabIndex = 12;
            buttonUseEncryptionCode.Text = "Use Code";
            buttonUseEncryptionCode.UseVisualStyleBackColor = true;

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 382);

            Controls.Add(buttonUseEncryptionCode);
            Controls.Add(textBoxEnterEncryptionCode);
            Controls.Add(textBoxEnterInformation);
            Controls.Add(textBoxQuantity);
            Controls.Add(buttonLoadProducts);
            Controls.Add(textReadProductTitle);
            Controls.Add(buttonLogPurchase);
            Controls.Add(buttonLoadImage);
            Controls.Add(imageBoxLoadImage);
            Controls.Add(buttonPlusOne);
            Controls.Add(buttonMinusOne);
            Controls.Add(listBoxProducts);

            Name = "Form1";
            Text = "Marketplace";

            ((System.ComponentModel.ISupportInitialize)imageBoxLoadImage).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TreeView listBoxProducts;
        private Button buttonMinusOne;
        private Button buttonPlusOne;
        private PictureBox imageBoxLoadImage;
        private Button buttonLoadImage;
        private Button buttonLogPurchase;
        private TextBox textReadProductTitle;
        private Button buttonLoadProducts;
        private TextBox textBoxQuantity;
        private TextBox textBoxEnterInformation;
        private TextBox textBoxEnterEncryptionCode;
        private Button buttonUseEncryptionCode;
    }
}