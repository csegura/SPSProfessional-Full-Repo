namespace SPSProfessional.ActionDataBase.Generator
{
    partial class frmGenerator
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGenerator));
            this.mnuStrip = new System.Windows.Forms.MenuStrip();
            this.mnuFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDatabaseConnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stsStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lstTables = new System.Windows.Forms.ListView();
            this.Table = new System.Windows.Forms.ColumnHeader();
            this.ctxMnuTables = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuSelect100 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShowColumns = new System.Windows.Forms.ToolStripMenuItem();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabCommand = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtCommand = new SPSProfessional.ActionDataBase.Generator.HilightRichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuRunSQL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGenerateGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGenerateForm = new System.Windows.Forms.ToolStripMenuItem();
            this.grid = new System.Windows.Forms.DataGridView();
            this.tabColumns = new System.Windows.Forms.TabPage();
            this.lvColumns = new System.Windows.Forms.ListView();
            this.tabGridXML = new System.Windows.Forms.TabPage();
            this.txtGridXML = new SPSProfessional.ActionDataBase.Generator.HilightRichTextBox();
            this.menuStrip5 = new System.Windows.Forms.MenuStrip();
            this.mnuValidateXMLGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyGridXML = new System.Windows.Forms.ToolStripMenuItem();
            this.tabFormXML = new System.Windows.Forms.TabPage();
            this.txtFormXML = new SPSProfessional.ActionDataBase.Generator.HilightRichTextBox();
            this.menuStrip4 = new System.Windows.Forms.MenuStrip();
            this.mnuValidateXMLForm = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCopyFormXML = new System.Windows.Forms.ToolStripMenuItem();
            this.tabXMLGridEditor = new System.Windows.Forms.TabPage();
            this.pgGrid = new System.Windows.Forms.PropertyGrid();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.mnuGenerateGridXML = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGetGridFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tabXMLFormEditor = new System.Windows.Forms.TabPage();
            this.pgEditor = new System.Windows.Forms.PropertyGrid();
            this.menuStrip3 = new System.Windows.Forms.MenuStrip();
            this.mnuGenerateFormXML = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGetEditorFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.lstErrors = new System.Windows.Forms.ListView();
            this.colMessage = new System.Windows.Forms.ColumnHeader();
            this.colLine = new System.Windows.Forms.ColumnHeader();
            this.colColumn = new System.Windows.Forms.ColumnHeader();
            this.XML = new System.Windows.Forms.ColumnHeader();
            this.imgStatus = new System.Windows.Forms.ImageList(this.components);
            this.getXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStrip.SuspendLayout();
            this.stsStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.ctxMnuTables.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabCommand.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.tabColumns.SuspendLayout();
            this.tabGridXML.SuspendLayout();
            this.menuStrip5.SuspendLayout();
            this.tabFormXML.SuspendLayout();
            this.menuStrip4.SuspendLayout();
            this.tabXMLGridEditor.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.tabXMLFormEditor.SuspendLayout();
            this.menuStrip3.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuStrip
            // 
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFiles,
            this.aboutToolStripMenuItem});
            this.mnuStrip.Location = new System.Drawing.Point(0, 0);
            this.mnuStrip.Name = "mnuStrip";
            this.mnuStrip.Size = new System.Drawing.Size(629, 24);
            this.mnuStrip.TabIndex = 0;
            this.mnuStrip.Text = "menuStrip1";
            // 
            // mnuFiles
            // 
            this.mnuFiles.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDatabaseConnect,
            this.toolStripSeparator1,
            this.toolStripSeparator3});
            this.mnuFiles.Name = "mnuFiles";
            this.mnuFiles.Size = new System.Drawing.Size(35, 20);
            this.mnuFiles.Text = "&File";
            // 
            // mnuDatabaseConnect
            // 
            this.mnuDatabaseConnect.Name = "mnuDatabaseConnect";
            this.mnuDatabaseConnect.Size = new System.Drawing.Size(174, 22);
            this.mnuDatabaseConnect.Text = "Database &Connect";
            this.mnuDatabaseConnect.Click += new System.EventHandler(this.mnuDatabaseConnect_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(171, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            // 
            // stsStrip
            // 
            this.stsStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.stsStrip.Location = new System.Drawing.Point(0, 343);
            this.stsStrip.Name = "stsStrip";
            this.stsStrip.Size = new System.Drawing.Size(629, 22);
            this.stsStrip.TabIndex = 1;
            this.stsStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.BorderStyle = System.Windows.Forms.Border3DStyle.Raised;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(158, 17);
            this.lblStatus.Text = "v1.1 - SPSProfessional (c) 2007";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabs);
            this.splitContainer1.Size = new System.Drawing.Size(629, 319);
            this.splitContainer1.SplitterDistance = 209;
            this.splitContainer1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lstTables);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(209, 319);
            this.panel1.TabIndex = 1;
            // 
            // lstTables
            // 
            this.lstTables.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Table});
            this.lstTables.ContextMenuStrip = this.ctxMnuTables;
            this.lstTables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTables.HideSelection = false;
            this.lstTables.Location = new System.Drawing.Point(0, 0);
            this.lstTables.Name = "lstTables";
            this.lstTables.Size = new System.Drawing.Size(209, 319);
            this.lstTables.SmallImageList = this.imgList;
            this.lstTables.TabIndex = 0;
            this.lstTables.UseCompatibleStateImageBehavior = false;
            this.lstTables.View = System.Windows.Forms.View.Details;
            this.lstTables.SelectedIndexChanged += new System.EventHandler(this.lstTables_SelectedIndexChanged);
            // 
            // Table
            // 
            this.Table.Text = "Tables/Views";
            this.Table.Width = 300;
            // 
            // ctxMnuTables
            // 
            this.ctxMnuTables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSelect100,
            this.mnuSelectAll,
            this.toolStripSeparator2,
            this.mnuShowColumns});
            this.ctxMnuTables.Name = "ctxMnuTables";
            this.ctxMnuTables.Size = new System.Drawing.Size(155, 76);
            // 
            // mnuSelect100
            // 
            this.mnuSelect100.Name = "mnuSelect100";
            this.mnuSelect100.Size = new System.Drawing.Size(154, 22);
            this.mnuSelect100.Text = "Select top 100";
            this.mnuSelect100.Click += new System.EventHandler(this.mnuSelect100_Click);
            // 
            // mnuSelectAll
            // 
            this.mnuSelectAll.Name = "mnuSelectAll";
            this.mnuSelectAll.Size = new System.Drawing.Size(154, 22);
            this.mnuSelectAll.Text = "Select All";
            this.mnuSelectAll.Click += new System.EventHandler(this.mnuSelectAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(151, 6);
            // 
            // mnuShowColumns
            // 
            this.mnuShowColumns.Name = "mnuShowColumns";
            this.mnuShowColumns.Size = new System.Drawing.Size(154, 22);
            this.mnuShowColumns.Text = "Show Columns";
            this.mnuShowColumns.Click += new System.EventHandler(this.mnuShowColumns_Click);
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "Web_XML.ico");
            this.imgList.Images.SetKeyName(1, "services.ico");
            this.imgList.Images.SetKeyName(2, "Data_Schema.ico");
            this.imgList.Images.SetKeyName(3, "db.ico");
            this.imgList.Images.SetKeyName(4, "eventlog.ico");
            this.imgList.Images.SetKeyName(5, "table.gif");
            this.imgList.Images.SetKeyName(6, "view.png");
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabCommand);
            this.tabs.Controls.Add(this.tabColumns);
            this.tabs.Controls.Add(this.tabGridXML);
            this.tabs.Controls.Add(this.tabFormXML);
            this.tabs.Controls.Add(this.tabXMLGridEditor);
            this.tabs.Controls.Add(this.tabXMLFormEditor);
            this.tabs.Controls.Add(this.tabErrors);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.ImageList = this.imgList;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(416, 319);
            this.tabs.TabIndex = 0;
            this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
            // 
            // tabCommand
            // 
            this.tabCommand.Controls.Add(this.splitContainer2);
            this.tabCommand.ImageIndex = 3;
            this.tabCommand.Location = new System.Drawing.Point(4, 23);
            this.tabCommand.Name = "tabCommand";
            this.tabCommand.Size = new System.Drawing.Size(408, 292);
            this.tabCommand.TabIndex = 0;
            this.tabCommand.Text = "Command";
            this.tabCommand.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.txtCommand);
            this.splitContainer2.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.grid);
            this.splitContainer2.Size = new System.Drawing.Size(408, 292);
            this.splitContainer2.SplitterDistance = 112;
            this.splitContainer2.TabIndex = 0;
            // 
            // txtCommand
            // 
            this.txtCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCommand.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCommand.Location = new System.Drawing.Point(0, 24);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(408, 88);
            this.txtCommand.TabIndex = 2;
            this.txtCommand.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRunSQL,
            this.mnuGenerateGrid,
            this.mnuGenerateForm});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(408, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuRunSQL
            // 
            this.mnuRunSQL.Image = ((System.Drawing.Image)(resources.GetObject("mnuRunSQL.Image")));
            this.mnuRunSQL.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuRunSQL.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuRunSQL.Name = "mnuRunSQL";
            this.mnuRunSQL.Size = new System.Drawing.Size(54, 20);
            this.mnuRunSQL.Text = "&Run";
            this.mnuRunSQL.Click += new System.EventHandler(this.mnuRunSQL_Click);
            // 
            // mnuGenerateGrid
            // 
            this.mnuGenerateGrid.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuGenerateGrid.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuGenerateGrid.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGenerateGrid.Name = "mnuGenerateGrid";
            this.mnuGenerateGrid.Size = new System.Drawing.Size(101, 20);
            this.mnuGenerateGrid.Text = "&Generate Grid";
            this.mnuGenerateGrid.Click += new System.EventHandler(this.mnuGenerateGrid_Click);
            // 
            // mnuGenerateForm
            // 
            this.mnuGenerateForm.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuGenerateForm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.mnuGenerateForm.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGenerateForm.Name = "mnuGenerateForm";
            this.mnuGenerateForm.Size = new System.Drawing.Size(106, 20);
            this.mnuGenerateForm.Text = "Generate Form";
            this.mnuGenerateForm.Click += new System.EventHandler(this.mnuGenerateForm_Click);
            // 
            // grid
            // 
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 0);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(408, 176);
            this.grid.TabIndex = 0;
            // 
            // tabColumns
            // 
            this.tabColumns.Controls.Add(this.lvColumns);
            this.tabColumns.ImageIndex = 3;
            this.tabColumns.Location = new System.Drawing.Point(4, 23);
            this.tabColumns.Name = "tabColumns";
            this.tabColumns.Padding = new System.Windows.Forms.Padding(3);
            this.tabColumns.Size = new System.Drawing.Size(408, 292);
            this.tabColumns.TabIndex = 1;
            this.tabColumns.Text = "Columns";
            this.tabColumns.UseVisualStyleBackColor = true;
            // 
            // lvColumns
            // 
            this.lvColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvColumns.FullRowSelect = true;
            this.lvColumns.GridLines = true;
            this.lvColumns.Location = new System.Drawing.Point(3, 3);
            this.lvColumns.Name = "lvColumns";
            this.lvColumns.Size = new System.Drawing.Size(402, 286);
            this.lvColumns.TabIndex = 0;
            this.lvColumns.UseCompatibleStateImageBehavior = false;
            this.lvColumns.View = System.Windows.Forms.View.Details;
            // 
            // tabGridXML
            // 
            this.tabGridXML.Controls.Add(this.txtGridXML);
            this.tabGridXML.Controls.Add(this.menuStrip5);
            this.tabGridXML.ImageIndex = 0;
            this.tabGridXML.Location = new System.Drawing.Point(4, 23);
            this.tabGridXML.Name = "tabGridXML";
            this.tabGridXML.Size = new System.Drawing.Size(408, 292);
            this.tabGridXML.TabIndex = 2;
            this.tabGridXML.Text = "Grid XML";
            this.tabGridXML.UseVisualStyleBackColor = true;
            // 
            // txtGridXML
            // 
            this.txtGridXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGridXML.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtGridXML.Location = new System.Drawing.Point(0, 24);
            this.txtGridXML.Name = "txtGridXML";
            this.txtGridXML.Size = new System.Drawing.Size(408, 268);
            this.txtGridXML.TabIndex = 3;
            this.txtGridXML.Text = "";
            // 
            // menuStrip5
            // 
            this.menuStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuValidateXMLGrid,
            this.mnuCopyGridXML});
            this.menuStrip5.Location = new System.Drawing.Point(0, 0);
            this.menuStrip5.Name = "menuStrip5";
            this.menuStrip5.Size = new System.Drawing.Size(408, 24);
            this.menuStrip5.TabIndex = 2;
            this.menuStrip5.Text = "menuStrip3";
            // 
            // mnuValidateXMLGrid
            // 
            this.mnuValidateXMLGrid.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuValidateXMLGrid.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuValidateXMLGrid.Name = "mnuValidateXMLGrid";
            this.mnuValidateXMLGrid.Size = new System.Drawing.Size(95, 20);
            this.mnuValidateXMLGrid.Text = "&Validate XML";
            this.mnuValidateXMLGrid.Click += new System.EventHandler(this.mnuValidateXMLGridm_Click);
            // 
            // mnuCopyGridXML
            // 
            this.mnuCopyGridXML.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.copyToolStripMenuItem_Image;
            this.mnuCopyGridXML.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuCopyGridXML.Name = "mnuCopyGridXML";
            this.mnuCopyGridXML.Size = new System.Drawing.Size(60, 20);
            this.mnuCopyGridXML.Text = "&Copy";
            this.mnuCopyGridXML.Click += new System.EventHandler(this.mnuCopyGridXML_Click);
            // 
            // tabFormXML
            // 
            this.tabFormXML.Controls.Add(this.txtFormXML);
            this.tabFormXML.Controls.Add(this.menuStrip4);
            this.tabFormXML.ImageIndex = 0;
            this.tabFormXML.Location = new System.Drawing.Point(4, 23);
            this.tabFormXML.Name = "tabFormXML";
            this.tabFormXML.Size = new System.Drawing.Size(408, 292);
            this.tabFormXML.TabIndex = 3;
            this.tabFormXML.Text = "Form XML";
            this.tabFormXML.UseVisualStyleBackColor = true;
            // 
            // txtFormXML
            // 
            this.txtFormXML.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFormXML.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFormXML.Location = new System.Drawing.Point(0, 24);
            this.txtFormXML.Name = "txtFormXML";
            this.txtFormXML.Size = new System.Drawing.Size(408, 268);
            this.txtFormXML.TabIndex = 3;
            this.txtFormXML.Text = "";
            // 
            // menuStrip4
            // 
            this.menuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuValidateXMLForm,
            this.mnuCopyFormXML});
            this.menuStrip4.Location = new System.Drawing.Point(0, 0);
            this.menuStrip4.Name = "menuStrip4";
            this.menuStrip4.Size = new System.Drawing.Size(408, 24);
            this.menuStrip4.TabIndex = 2;
            this.menuStrip4.Text = "menuStrip4";
            // 
            // mnuValidateXMLForm
            // 
            this.mnuValidateXMLForm.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuValidateXMLForm.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuValidateXMLForm.Name = "mnuValidateXMLForm";
            this.mnuValidateXMLForm.Size = new System.Drawing.Size(95, 20);
            this.mnuValidateXMLForm.Text = "&Validate XML";
            this.mnuValidateXMLForm.Click += new System.EventHandler(this.mnuValidateXMLForm_Click);
            // 
            // mnuCopyFormXML
            // 
            this.mnuCopyFormXML.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.copyToolStripMenuItem_Image;
            this.mnuCopyFormXML.Name = "mnuCopyFormXML";
            this.mnuCopyFormXML.Size = new System.Drawing.Size(60, 20);
            this.mnuCopyFormXML.Text = "&Copy";
            this.mnuCopyFormXML.Click += new System.EventHandler(this.mnuCopyFormXML_Click);
            // 
            // tabXMLGridEditor
            // 
            this.tabXMLGridEditor.Controls.Add(this.pgGrid);
            this.tabXMLGridEditor.Controls.Add(this.menuStrip2);
            this.tabXMLGridEditor.ImageIndex = 2;
            this.tabXMLGridEditor.Location = new System.Drawing.Point(4, 23);
            this.tabXMLGridEditor.Name = "tabXMLGridEditor";
            this.tabXMLGridEditor.Size = new System.Drawing.Size(408, 292);
            this.tabXMLGridEditor.TabIndex = 7;
            this.tabXMLGridEditor.Text = "XML Grid Editor";
            this.tabXMLGridEditor.UseVisualStyleBackColor = true;
            // 
            // pgGrid
            // 
            this.pgGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgGrid.Location = new System.Drawing.Point(0, 24);
            this.pgGrid.Name = "pgGrid";
            this.pgGrid.Size = new System.Drawing.Size(408, 268);
            this.pgGrid.TabIndex = 1;
            this.pgGrid.ToolbarVisible = false;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGenerateGridXML,
            this.mnuGetGridFromClipboard});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(408, 24);
            this.menuStrip2.TabIndex = 0;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // mnuGenerateGridXML
            // 
            this.mnuGenerateGridXML.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuGenerateGridXML.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGenerateGridXML.Name = "mnuGenerateGridXML";
            this.mnuGenerateGridXML.Size = new System.Drawing.Size(80, 20);
            this.mnuGenerateGridXML.Text = "Generate";
            this.mnuGenerateGridXML.Click += new System.EventHandler(this.mnuGenerateGridXML_Click);
            // 
            // mnuGetGridFromClipboard
            // 
            this.mnuGetGridFromClipboard.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.pasteToolStripMenuItem_Image;
            this.mnuGetGridFromClipboard.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGetGridFromClipboard.Name = "mnuGetGridFromClipboard";
            this.mnuGetGridFromClipboard.Size = new System.Drawing.Size(125, 20);
            this.mnuGetGridFromClipboard.Text = "Get from Clipboard";
            this.mnuGetGridFromClipboard.Click += new System.EventHandler(this.mnuGetGridFromClipboard_Click);
            // 
            // tabXMLFormEditor
            // 
            this.tabXMLFormEditor.Controls.Add(this.pgEditor);
            this.tabXMLFormEditor.Controls.Add(this.menuStrip3);
            this.tabXMLFormEditor.ImageIndex = 2;
            this.tabXMLFormEditor.Location = new System.Drawing.Point(4, 23);
            this.tabXMLFormEditor.Name = "tabXMLFormEditor";
            this.tabXMLFormEditor.Size = new System.Drawing.Size(408, 292);
            this.tabXMLFormEditor.TabIndex = 5;
            this.tabXMLFormEditor.Text = "XML Form Editor";
            this.tabXMLFormEditor.UseVisualStyleBackColor = true;
            // 
            // pgEditor
            // 
            this.pgEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgEditor.Location = new System.Drawing.Point(0, 24);
            this.pgEditor.Name = "pgEditor";
            this.pgEditor.Size = new System.Drawing.Size(408, 268);
            this.pgEditor.TabIndex = 1;
            this.pgEditor.ToolbarVisible = false;
            // 
            // menuStrip3
            // 
            this.menuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuGenerateFormXML,
            this.mnuGetEditorFromClipboard});
            this.menuStrip3.Location = new System.Drawing.Point(0, 0);
            this.menuStrip3.Name = "menuStrip3";
            this.menuStrip3.Size = new System.Drawing.Size(408, 24);
            this.menuStrip3.TabIndex = 0;
            this.menuStrip3.Text = "menuStrip3";
            // 
            // mnuGenerateFormXML
            // 
            this.mnuGenerateFormXML.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.gear_1;
            this.mnuGenerateFormXML.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGenerateFormXML.Name = "mnuGenerateFormXML";
            this.mnuGenerateFormXML.Size = new System.Drawing.Size(80, 20);
            this.mnuGenerateFormXML.Text = "&Generate";
            this.mnuGenerateFormXML.Click += new System.EventHandler(this.mnuGenerateFormXML_Click);
            // 
            // mnuGetEditorFromClipboard
            // 
            this.mnuGetEditorFromClipboard.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.pasteToolStripMenuItem_Image;
            this.mnuGetEditorFromClipboard.ImageTransparentColor = System.Drawing.Color.Fuchsia;
            this.mnuGetEditorFromClipboard.Name = "mnuGetEditorFromClipboard";
            this.mnuGetEditorFromClipboard.Size = new System.Drawing.Size(125, 20);
            this.mnuGetEditorFromClipboard.Text = "Get from Clipboard";
            this.mnuGetEditorFromClipboard.Click += new System.EventHandler(this.mnuGetEditorFromClipboard_Click);
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.lstErrors);
            this.tabErrors.ImageIndex = 4;
            this.tabErrors.Location = new System.Drawing.Point(4, 23);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Size = new System.Drawing.Size(408, 292);
            this.tabErrors.TabIndex = 6;
            this.tabErrors.Text = "Errors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // lstErrors
            // 
            this.lstErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMessage,
            this.colLine,
            this.colColumn,
            this.XML});
            this.lstErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstErrors.FullRowSelect = true;
            this.lstErrors.GridLines = true;
            this.lstErrors.Location = new System.Drawing.Point(0, 0);
            this.lstErrors.Name = "lstErrors";
            this.lstErrors.Size = new System.Drawing.Size(408, 292);
            this.lstErrors.SmallImageList = this.imgStatus;
            this.lstErrors.TabIndex = 0;
            this.lstErrors.UseCompatibleStateImageBehavior = false;
            this.lstErrors.View = System.Windows.Forms.View.Details;
            this.lstErrors.DoubleClick += new System.EventHandler(this.lstErrors_DoubleClick);
            // 
            // colMessage
            // 
            this.colMessage.Text = "Message";
            this.colMessage.Width = 275;
            // 
            // colLine
            // 
            this.colLine.Text = "Line";
            this.colLine.Width = 33;
            // 
            // colColumn
            // 
            this.colColumn.Text = "Column";
            this.colColumn.Width = 28;
            // 
            // XML
            // 
            this.XML.Text = "XML";
            this.XML.Width = 69;
            // 
            // imgStatus
            // 
            this.imgStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgStatus.ImageStream")));
            this.imgStatus.TransparentColor = System.Drawing.Color.Lime;
            this.imgStatus.Images.SetKeyName(0, "info.gif");
            this.imgStatus.Images.SetKeyName(1, "error.gif");
            // 
            // getXMLToolStripMenuItem
            // 
            this.getXMLToolStripMenuItem.Name = "getXMLToolStripMenuItem";
            this.getXMLToolStripMenuItem.Size = new System.Drawing.Size(80, 20);
            this.getXMLToolStripMenuItem.Text = "&Get Grid XML";
            // 
            // frmGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 365);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.stsStrip);
            this.Controls.Add(this.mnuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuStrip;
            this.Name = "frmGenerator";
            this.Text = "SPSProfessional ActionDataBase Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmGenerator_FormClosing);
            this.mnuStrip.ResumeLayout(false);
            this.mnuStrip.PerformLayout();
            this.stsStrip.ResumeLayout(false);
            this.stsStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ctxMnuTables.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabCommand.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.tabColumns.ResumeLayout(false);
            this.tabGridXML.ResumeLayout(false);
            this.tabGridXML.PerformLayout();
            this.menuStrip5.ResumeLayout(false);
            this.menuStrip5.PerformLayout();
            this.tabFormXML.ResumeLayout(false);
            this.tabFormXML.PerformLayout();
            this.menuStrip4.ResumeLayout(false);
            this.menuStrip4.PerformLayout();
            this.tabXMLGridEditor.ResumeLayout(false);
            this.tabXMLGridEditor.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.tabXMLFormEditor.ResumeLayout(false);
            this.tabXMLFormEditor.PerformLayout();
            this.menuStrip3.ResumeLayout(false);
            this.menuStrip3.PerformLayout();
            this.tabErrors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuStrip;
        private System.Windows.Forms.StatusStrip stsStrip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem mnuFiles;
        private System.Windows.Forms.ToolStripMenuItem mnuDatabaseConnect;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ContextMenuStrip ctxMnuTables;
        private System.Windows.Forms.ToolStripMenuItem mnuSelect100;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuShowColumns;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ToolStripMenuItem getXMLToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabCommand;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuRunSQL;
        private System.Windows.Forms.ToolStripMenuItem mnuGenerateGrid;
        private System.Windows.Forms.ToolStripMenuItem mnuGenerateForm;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.TabPage tabColumns;
        private System.Windows.Forms.ListView lvColumns;
        private System.Windows.Forms.TabPage tabGridXML;
        private System.Windows.Forms.MenuStrip menuStrip5;
        private System.Windows.Forms.ToolStripMenuItem mnuValidateXMLGrid;
        private System.Windows.Forms.TabPage tabFormXML;
        private System.Windows.Forms.MenuStrip menuStrip4;
        private System.Windows.Forms.ToolStripMenuItem mnuValidateXMLForm;
        private System.Windows.Forms.TabPage tabXMLFormEditor;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.TabPage tabXMLGridEditor;
        private System.Windows.Forms.PropertyGrid pgGrid;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem mnuGenerateGridXML;
        private System.Windows.Forms.ToolStripMenuItem mnuGetGridFromClipboard;
        private System.Windows.Forms.PropertyGrid pgEditor;
        private System.Windows.Forms.MenuStrip menuStrip3;
        private System.Windows.Forms.ToolStripMenuItem mnuGenerateFormXML;
        private System.Windows.Forms.ToolStripMenuItem mnuGetEditorFromClipboard;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyGridXML;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyFormXML;
        private System.Windows.Forms.ImageList imgStatus;
        private System.Windows.Forms.Panel panel1;
        private HilightRichTextBox txtGridXML;
        private HilightRichTextBox txtFormXML;
        private System.Windows.Forms.ListView lstErrors;
        private System.Windows.Forms.ColumnHeader colMessage;
        private System.Windows.Forms.ColumnHeader colLine;
        private System.Windows.Forms.ColumnHeader colColumn;
        private System.Windows.Forms.ColumnHeader XML;
        private HilightRichTextBox txtCommand;
        private System.Windows.Forms.ListView lstTables;
        private System.Windows.Forms.ColumnHeader Table;
    }
}