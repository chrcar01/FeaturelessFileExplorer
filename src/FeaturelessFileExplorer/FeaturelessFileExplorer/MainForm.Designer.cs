namespace FeaturelessFileExplorer
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new TableLayoutPanel();
            panel1 = new Panel();
            lblFolder = new Label();
            txtFolder = new TextBox();
            lvFilesAndFolders = new ListView();
            imageList = new ImageList(components);
            tableLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(lvFilesAndFolders, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(7, 8, 7, 8);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 109F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(1943, 1230);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblFolder);
            panel1.Controls.Add(txtFolder);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(7, 8);
            panel1.Margin = new Padding(7, 8, 7, 8);
            panel1.Name = "panel1";
            panel1.Size = new Size(1929, 93);
            panel1.TabIndex = 0;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new Point(22, 27);
            lblFolder.Margin = new Padding(7, 0, 7, 0);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(109, 41);
            lblFolder.TabIndex = 1;
            lblFolder.Text = "Folder:";
            // 
            // txtFolder
            // 
            txtFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolder.Location = new Point(137, 19);
            txtFolder.Margin = new Padding(7, 8, 7, 8);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(1779, 47);
            txtFolder.TabIndex = 0;
            txtFolder.KeyDown += txtFolder_KeyDown;
            // 
            // lvFilesAndFolders
            // 
            lvFilesAndFolders.Dock = DockStyle.Fill;
            lvFilesAndFolders.Location = new Point(20, 129);
            lvFilesAndFolders.Margin = new Padding(20);
            lvFilesAndFolders.Name = "lvFilesAndFolders";
            lvFilesAndFolders.Size = new Size(1903, 1081);
            lvFilesAndFolders.SmallImageList = imageList;
            lvFilesAndFolders.TabIndex = 1;
            lvFilesAndFolders.UseCompatibleStateImageBehavior = false;
            lvFilesAndFolders.View = View.Details;
            lvFilesAndFolders.VirtualMode = true;
            lvFilesAndFolders.RetrieveVirtualItem += lvFilesAndFolders_RetrieveVirtualItem;
            lvFilesAndFolders.DoubleClick += lvFilesAndFolders_DoubleClick;
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.TransparentColor = Color.Transparent;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1943, 1230);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(7, 8, 7, 8);
            Name = "MainForm";
            Text = "Featureless File Explorer";
            tableLayoutPanel1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private TextBox txtFolder;
        private Label lblFolder;
        private ListView lvFilesAndFolders;
        private ImageList imageList;
    }
}