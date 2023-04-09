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
            contextMenu = new ContextMenuStrip(components);
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
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(800, 388);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Controls.Add(lblFolder);
            panel1.Controls.Add(txtFolder);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(794, 34);
            panel1.TabIndex = 0;
            // 
            // lblFolder
            // 
            lblFolder.AutoSize = true;
            lblFolder.Location = new Point(9, 10);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(43, 15);
            lblFolder.TabIndex = 1;
            lblFolder.Text = "Folder:";
            // 
            // txtFolder
            // 
            txtFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFolder.Location = new Point(56, 7);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(735, 23);
            txtFolder.TabIndex = 0;
            txtFolder.KeyDown += txtFolder_KeyDown;
            // 
            // lvFilesAndFolders
            // 
            lvFilesAndFolders.ContextMenuStrip = contextMenu;
            lvFilesAndFolders.Dock = DockStyle.Fill;
            lvFilesAndFolders.Location = new Point(8, 47);
            lvFilesAndFolders.Margin = new Padding(8, 7, 8, 7);
            lvFilesAndFolders.Name = "lvFilesAndFolders";
            lvFilesAndFolders.Size = new Size(784, 334);
            lvFilesAndFolders.SmallImageList = imageList;
            lvFilesAndFolders.TabIndex = 1;
            lvFilesAndFolders.UseCompatibleStateImageBehavior = false;
            lvFilesAndFolders.View = View.Details;
            lvFilesAndFolders.VirtualMode = true;
            lvFilesAndFolders.ColumnClick += lvFilesAndFolders_ColumnClick;
            lvFilesAndFolders.RetrieveVirtualItem += lvFilesAndFolders_RetrieveVirtualItem;
            lvFilesAndFolders.DoubleClick += lvFilesAndFolders_DoubleClick;
            lvFilesAndFolders.MouseDown += lvFilesAndFolders_MouseDown;
            // 
            // contextMenu
            // 
            contextMenu.Name = "contextMenuStrip1";
            contextMenu.Size = new Size(61, 4);
            // 
            // imageList
            // 
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new Size(32, 32);
            imageList.TransparentColor = Color.Transparent;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 388);
            Controls.Add(tableLayoutPanel1);
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
        private ContextMenuStrip contextMenu;
    }
}