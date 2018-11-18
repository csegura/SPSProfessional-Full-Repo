using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using SPSProfessional.ActionDataBase.Generator.EditorConfig;
using SPSProfessional.ActionDataBase.Generator.GridConfig;
using SortOrder=System.Windows.Forms.SortOrder;
using TextBox=System.Windows.Forms.TextBox;

namespace SPSProfessional.ActionDataBase.Generator
{
    public partial class frmGenerator : Form
    {
        private enum XMLMode
        {
            Grid,
            Editor
        } ;

        private XMLMode _xmlMode;
        private DataTable _columnsTable;

        private Configuration _config;
        private SPSActionEditConfig _configEditor;
        private SPSActionGridConfig _configGrid;
        private DataTable _dataTable;
        private readonly Stack<string> _lastConfigFiles;
        private string _selectedTable;

        private int _xmlErrorsCounter;

        public frmGenerator()
        {
            _lastConfigFiles = new Stack<string>();
            InitializeComponent();
            LastOpenInitialize();
            _selectedTable = string.Empty;
            InitializeXMLEditor(txtFormXML);
            InitializeXMLEditor(txtGridXML);
            InitializeSQLEditor(txtCommand);
            pgEditor.SelectedObject = new SPSActionEditConfig();
            pgGrid.SelectedObject = new SPSActionGridConfig();
        }

        private void LastOpenInitialize()
        {
            _config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);

            for (int i = 4; i >= 0; i--)
            {
                if (_config.AppSettings.Settings["Last" + i] != null)
                {
                    string last = _config.AppSettings.Settings["Last" + i].Value;
                    if (!string.IsNullOrEmpty(last))
                    {
                        _lastConfigFiles.Push(last);
                        SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder(last);
                        ToolStripItem mnuItem = new ToolStripMenuItem();
                        mnuItem.Text =
                            string.Format("{0}.- {1} [{2}]", i, connBuilder.DataSource, connBuilder.InitialCatalog);
                        mnuItem.Click += LastOpen;
                        mnuFiles.DropDownItems.Add(mnuItem);
                    }
                }
            }
        }

        private void LastOpen(object sender, EventArgs e)
        {
            ToolStripItem mnuItem = (ToolStripMenuItem) sender;
            int i = 0;
            if (Int32.TryParse(mnuItem.Text.Substring(0, 1), out i))
            {
                Generator.GetGenerator().ConnectionString = _lastConfigFiles.ToArray()[i];
                FillTables();
            }
        }


        private void mnuGenerateGridXML_Click(object sender, EventArgs e)
        {
            try
            {
                StringWriter xml = new StringWriter(new StringBuilder());
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (SPSActionGridConfig));
                xmlSerializer.Serialize(xml, pgGrid.SelectedObject);
                txtGridXML.Text = xml.ToString();
                tabs.SelectedIndex = 2;
                txtGridXML.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            Status("ActionDataBaseGrid XML generated sucessfully.");
        }

        private void mnuGenerateFormXML_Click(object sender, EventArgs e)
        {
            try
            {
                StringWriter xml = new StringWriter(new StringBuilder());
                XmlSerializer xmlSerializer = new XmlSerializer(typeof (SPSActionEditConfig));
                xmlSerializer.Serialize(xml, pgEditor.SelectedObject);
                txtFormXML.Text = xml.ToString();
                tabs.SelectedIndex = 3;
                txtFormXML.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
            Status("ActionDataBasForm XML generated sucessfully.");
        }

        private void mnuValidateXMLGridm_Click(object sender, EventArgs e)
        {
            // schema
            Stream streamSchema =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "SPSProfessional.ActionDataBase.Generator.Schemas.ActionGridConfig.xsd");
            // xml
            TextReader xmlReader = new StringReader(txtGridXML.Text);
            
            lstErrors.Items.Clear();
            _xmlMode = XMLMode.Grid;

            ValidateXML(streamSchema, xmlReader);

            if (_xmlErrorsCounter == 0)
            {
                LoadXMLGridEditor();
                Status("ActionDataBaseGrid Validation OK. Data copied in XML Grid editor.");
            }
        }

        private void mnuValidateXMLForm_Click(object sender, EventArgs e)
        {
            // schema
            Stream streamSchema =
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                    "SPSProfessional.ActionDataBase.Generator.Schemas.ActionEditConfig.xsd");
            // xml
            TextReader xmlReader = new StringReader(txtFormXML.Text);

            lstErrors.Items.Clear();
            _xmlMode = XMLMode.Editor;

            ValidateXML(streamSchema, xmlReader);

            if (_xmlErrorsCounter == 0)
            {
                LoadXMLFormEditor();
                Status("ActionDataBaseEditor Validation OK. Data copied in XML Form editor.");
            }
        }

        private void ValidateXML(Stream streamSchema, TextReader xmlReader)
        {
            XmlSchema xmlSchema; // = new XmlSchema();
            _xmlErrorsCounter = 0;

            xmlSchema = XmlSchema.Read(streamSchema, XmlValidationHandler);

            XmlReaderSettings readerSettings = new XmlReaderSettings();

            readerSettings.ValidationType = ValidationType.Schema;
            readerSettings.Schemas.Add(xmlSchema);
            readerSettings.ValidationEventHandler += XmlValidationHandler;

            XmlReader objXmlReader = XmlReader.Create(xmlReader, readerSettings);

            try
            {
                while (objXmlReader.Read())
                {
                    ;
                }
            }
            catch(XmlException ex)
            {
                ListViewItem listViewItem = new ListViewItem(ex.Message, 1);

                listViewItem.SubItems.Add((ex.LineNumber-1).ToString());
                listViewItem.SubItems.Add((ex.LinePosition-1).ToString());
                listViewItem.SubItems.Add(_xmlMode.ToString());
                lstErrors.Items.Add(listViewItem);
                _xmlErrorsCounter++;
            }

            if (_xmlErrorsCounter > 0)
            {
                tabs.SelectedIndex = 6;
                Error("Check XML validation errors");
            }
        }

        private void XmlValidationHandler(object sender, ValidationEventArgs args)
        {
            XmlReader reader = (XmlReader) sender;

            ListViewItem listViewItem = new ListViewItem(args.Message,
                                                         args.Severity == XmlSeverityType.Warning ? 0 : 1);

            listViewItem.SubItems.Add(args.Exception.LineNumber.ToString());
            listViewItem.SubItems.Add(args.Exception.LinePosition.ToString());
            listViewItem.SubItems.Add(_xmlMode.ToString());

            lstErrors.Items.Add(listViewItem);

            _xmlErrorsCounter++;
        }


        private void frmGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string[] lasts = _lastConfigFiles.ToArray();
                int counter = (lasts.Length > 5) ? 5 : lasts.Length;
                _config.AppSettings.Settings.Clear();
                for (int i = 0; i < counter; i++)
                {
                    _config.AppSettings.Settings.Add("Last" + i, lasts[i]);
                    //["Last"+i] = lasts[i];
                }
                _config.Save(ConfigurationSaveMode.Full, true);
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void mnuGetGridFromClipboard_Click(object sender, EventArgs e)
        {
            string XML = Clipboard.GetText();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (SPSActionGridConfig));
                StringReader stringReader = new StringReader(XML);
                _configGrid = (SPSActionGridConfig) serializer.Deserialize(stringReader);
                stringReader.Close();
                pgGrid.SelectedObject = _configGrid;
                txtCommand.Text = _configGrid.Query.SelectCommand;
                Generator.GetGenerator().ConnectionString = _configGrid.DataBase.ConnectionString;
                FillTables();
                pgGrid.ExpandAllGridItems();
                Status("ActionDataBaseGrid configuration read sucessfully.");
            }
            catch (Exception ex)
            {
                Error("Invalid ActionDataBaseGrid XML. " + ex.Message);
            }
        }

        private void mnuGetEditorFromClipboard_Click(object sender, EventArgs e)
        {
            string XML = Clipboard.GetText();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (SPSActionEditConfig));
                StringReader stringReader = new StringReader(XML);
                _configEditor = (SPSActionEditConfig) serializer.Deserialize(stringReader);
                stringReader.Close();
                pgEditor.SelectedObject = _configEditor;
                txtCommand.Text = "SELECT * FROM " + _configEditor.DataBase.Table;
                Generator.GetGenerator().ConnectionString = _configEditor.DataBase.ConnectionString;
                FillTables();
                pgEditor.ExpandAllGridItems();
                Status("ActionDataBaseForm configuration read sucessfully.");
            }
            catch (Exception ex)
            {
                Error("Invalid ActionDataBaseForm XML. " + ex.Message);
            }
        }

        private void mnuCopyGridXML_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtGridXML.Text);            
            Status("XML ActionDataBaseGrid copied to clipboard.");
        }

        private void mnuCopyFormXML_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtFormXML.Text); 
            Status("XML ActionDataBaseForm copied to clipboard.");
        }

        #region MAIN MENU COMMANDS

        private void mnuDatabaseConnect_Click(object sender, EventArgs e)
        {
            frmDatabaseConnection frm = new frmDatabaseConnection();
            frm.ShowDialog();
            if (!string.IsNullOrEmpty(Generator.GetGenerator().ConnectionString))
            {
                _lastConfigFiles.Push(Generator.GetGenerator().ConnectionString);
                FillTables();
            }
        }

        #endregion

        #region USER ACTIONS

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void lstTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTables.SelectedItems.Count > 0)
                _selectedTable = lstTables.SelectedItems[0].Text;
        }

        private void mnuRunSQL_Click(object sender, EventArgs e)
        {
            txtCommand.ProcessAll();
            DoSqlCommand();
        }

        private void mnuGenerateGrid_Click(object sender, EventArgs e)
        {
            DoGridXML();
            tabs.SelectedIndex = 2;
            txtGridXML.SelectionLength = 0;
        }

        private void mnuGenerateForm_Click(object sender, EventArgs e)
        {
            DoFormXML();
            tabs.SelectedIndex = 3;
            txtFormXML.SelectionLength = 0;
        }

        #endregion

        #region BACKGROUND ACTIONS

        private void FillTables()
        {
            try
            {
                lstTables.Sorting = SortOrder.Ascending;
                lstTables.Items.Clear();

                foreach (string table in SPSDbTools.GetTables(Generator.GetGenerator().Connection))
                {
                    lstTables.Items.Add(new ListViewItem(table, 5));
                }
                foreach (string table in SPSDbTools.GetViews(Generator.GetGenerator().Connection))
                {
                    lstTables.Items.Add(new ListViewItem(table, 6));
                }
                
            }
            catch (SqlException ex)
            {
                Error(ex.Message);
            }
        }

        #endregion

        #region STATUS TOOLS

        private void Status(string message)
        {
            lblStatus.Text = message;
            lblStatus.Image = imgStatus.Images[0];
            lblStatus.ForeColor = Color.Blue;
        }

        private void Error(string message)
        {
            lblStatus.Text = message;
            lblStatus.Image = imgStatus.Images[1];
            lblStatus.ForeColor = Color.Red;
        }

        #endregion

        #region TABLES CONTEXTMENU ACTIONS

        private void mnuSelect100_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedTable))
            {
                txtCommand.Text = string.Format("SELECT TOP 100 * FROM {0}",
                                                (_selectedTable));
                tabs.SelectTab(0);
                DoSqlCommand();
            }
        }

        private void mnuSelectAll_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedTable))
            {
                txtCommand.Text = string.Format("SELECT * FROM {0}",
                                                (_selectedTable));
                tabs.SelectTab(0);
                DoSqlCommand();
            }
        }

        private void mnuShowColumns_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedTable))
            {
                DoColumnsInfo();
                tabs.SelectedIndex = 1;
            }
        }

        #endregion

        #region ACTIONS

        private void DoSqlCommand()
        {
            try
            {
                SqlConnection connection = Generator.GetGenerator().Connection;

                using (connection)
                {
                    SqlCommand command = new SqlCommand(txtCommand.Text,
                                                        connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    
                    grid.DataError += grid_DataError;
                    grid.DataSource = ds.Tables[0].DefaultView;
                    grid.AutoGenerateColumns = true;
                    
                    _dataTable = ds.Tables[0];

                    DoColumnsInfo();

                    Status(string.Format("{0} Rows", ds.Tables[0].DefaultView.Count));
                }
            }
            catch (Exception ex)
            {
                Error(ex.Message);
            }
        }

        private void grid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Error(e.Exception.Message);
        }

        private void DoColumnsInfo()
        {
            try
            {
                SqlConnection connection = Generator.GetGenerator().Connection;

                tabs.TabPages[1].Text = connection.Database + " - " + _selectedTable;

                string sql;


                sql = "SELECT TOP 1 * FROM " + _selectedTable;

                using (connection)
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(sql, connection);

                    SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SchemaOnly |
                                                          CommandBehavior.KeyInfo);


                    DataTable dt = rdr.GetSchemaTable();
                    rdr.Close();

                    lvColumns.Columns.Clear();
                    lvColumns.Items.Clear();

                    foreach (DataColumn column in dt.Columns)
                    {
                        lvColumns.Columns.Add(column.ColumnName);
                        lvColumns.Width = -2;
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        List<string> rowData = new List<string>();
                        foreach (DataColumn column in dt.Columns)
                        {
                            if (column.ColumnName == "ProviderType")
                            {
                                rowData.Add(((OleDbType) row[column.ColumnName]).ToString());
                            }
                            else
                            {
                                rowData.Add(row[column.ColumnName].ToString());
                            }
                        }
                        lvColumns.Items.Add(new ListViewItem(rowData.ToArray()));
                    }
                    _columnsTable = dt;
                }
                Status(string.Format("Columns schema from {0} loaded.", _selectedTable));
            }
            catch (Exception ex)
            {
                Error(string.Format("Error getting columns schema from {0} table.", _selectedTable));
                Trace.WriteLine(ex);
            }
        }

        #endregion

        #region GRID GENERATOR

        private void DoGridXML()
        {
            if (_dataTable != null)
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter w = new XmlTextWriter(sw);

                w.WriteStartDocument();
                w.WriteStartElement("SPSActionGridConfig");

                w.WriteStartElement("DataBase");
                w.WriteAttributeString("ConnectionString", Generator.GetGenerator().ConnectionString);
                w.WriteEndElement();

                w.WriteStartElement("Grid");
                w.WriteAttributeString("Name", "");
                w.WriteAttributeString("Sortable", "true");
                w.WriteAttributeString("Pageable", "true");
                w.WriteAttributeString("PageSize", "10");
                w.WriteEndElement();

                w.WriteStartElement("Query");
                w.WriteAttributeString("SelectCommand", txtCommand.Text);
                w.WriteEndElement();

                //if (_command.Parameters.Count > 0)
                //{
                //    w.WriteStartElement("Filter");
                //    foreach (SqlParameter param in _command.Parameters)
                //    {
                //        w.WriteStartElement("Filter");
                //        w.WriteAttributeString("Name", param.ParameterName);
                //        w.WriteAttributeString("Type", param.SqlDbType.ToString());
                //        w.WriteAttributeString("Default", "");
                //        w.WriteEndElement();
                //    }
                //    w.WriteEndElement();
                //}

                w.WriteStartElement("Columns");

                foreach (DataColumn column in _dataTable.Columns)
                {
                    w.WriteStartElement("DataField");
                    w.WriteAttributeString("Name", column.ColumnName);
                    w.WriteAttributeString("Header", column.ColumnName);
                    w.WriteEndElement();
                }

                w.WriteEndElement();

                w.WriteEndElement();

                w.WriteEndDocument();

                txtGridXML.Text = FormatXML(sw.ToString());

                LoadXMLGridEditor();

                Status("ActionDataBaseGrid XML generated sucessfully.");
            }
            else
            {
                Error("Run a query prior to generate a grid.");
            }
        }

        public void LoadXMLGridEditor()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (SPSActionGridConfig));
                StringReader stringReader = new StringReader(txtGridXML.Text);
                _configGrid = (SPSActionGridConfig) serializer.Deserialize(stringReader);
                stringReader.Close();
                pgGrid.SelectedObject = _configGrid;
                pgGrid.ExpandAllGridItems();
            }
            catch (Exception ex)
            {
                Error("Invalid XML. " + ex.Message);
            }

            #region OLD CODE

            //TextReader stringReader = new StringReader(txtGridXML.Text);
            //_dataSetXMLGrid = new DataSet();

            //Stream s =
            //    Assembly.GetExecutingAssembly().GetManifestResourceStream(
            //        "SPSProfessional.ActionDataBase.Generator.Schemas.ActionGridConfig.xsd");

            //_dataSetXMLGrid.ReadXmlSchema(s);

            //XmlTextReader reader = new XmlTextReader(stringReader);

            //_dataSetXMLGrid.ReadXml(reader);
            //_dataSetXMLGrid.EnforceConstraints = true;

            //gridXMLGrid.DataSource = _dataSetXMLGrid.DefaultViewManager;

            //lstXMLGrid.Items.Clear();

            //foreach (DataTable table in _dataSetXMLGrid.Tables)
            //{
            //    lstXMLGrid.Items.Add(table.TableName);
            //}

            #endregion
        }

        public string ApropiatedControl(string sysType, int length)
        {
            if (sysType.Contains("Date"))
            {
                return "Date";
            }
            else if (length > 255)
            {
                return "Memo";
            }
            else if (sysType.Contains("Bool"))
            {
                return "CheckBox";
            }
            else
            {
                return "TextBox";
            }
        }

        #endregion

        #region FORM GENERATOR

        private void DoFormXML()
        {
            if (_columnsTable != null)
            {
                StringWriter sw = new StringWriter();
                XmlTextWriter w = new XmlTextWriter(sw);

                w.WriteStartDocument();
                w.WriteStartElement("SPSActionEditConfig");

                w.WriteStartElement("DataBase");
                w.WriteAttributeString("ConnectionString", Generator.GetGenerator().ConnectionString);

                w.WriteStartElement("Table");
                w.WriteAttributeString("Name", _selectedTable);
                //w.WriteAttributeString("IdentityField", GetPrimaryKey());
                //w.WriteAttributeString("IdentityType", GetTypeForField(GetPrimaryKey()));

                // IdentityColumns 
                foreach (DataRow row in _columnsTable.Rows)
                {
                    if ((bool) row["IsKey"])
                    {
                        SqlDbType sqlDbType =
                            (SqlDbType) Enum.Parse(typeof (SqlDbType), row["DataTypeName"].ToString(), true);
                        string datatype = sqlDbType.ToString();

                        w.WriteStartElement("IdentityColumn");
                        w.WriteAttributeString("Name", row["ColumnName"].ToString());
                        w.WriteAttributeString("Type", datatype);
                        w.WriteEndElement();
                    }
                }

                w.WriteEndElement();
                w.WriteEndElement();

                #region ActionToolBars Sample

                w.WriteStartElement("ActionToolBars");

                w.WriteStartElement("ActionToolBar");
                w.WriteAttributeString("Name", "View");

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Edit");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/edit.gif");
                w.WriteAttributeString("Action", "Edit");
                w.WriteEndElement();

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Delete");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/delete.gif");
                w.WriteAttributeString("Action", "Delete");
                w.WriteEndElement();

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "New");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/newrole.gif");
                w.WriteAttributeString("Action", "New");
                w.WriteEndElement();

                w.WriteEndElement();

                w.WriteStartElement("ActionToolBar");
                w.WriteAttributeString("Name", "Edit");

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Update");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/save.gif");
                w.WriteAttributeString("Action", "Update");
                w.WriteEndElement();

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Back");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/back.gif");
                w.WriteAttributeString("Action", "Back");
                w.WriteEndElement();

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Delete");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/delete.gif");
                w.WriteAttributeString("Action", "Delete");
                w.WriteEndElement();

                w.WriteEndElement();

                w.WriteStartElement("ActionToolBar");
                w.WriteAttributeString("Name", "New");

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Save New");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/save.gif");
                w.WriteAttributeString("Action", "Create");
                w.WriteEndElement();

                w.WriteStartElement("Option");
                w.WriteAttributeString("Name", "Back");
                w.WriteAttributeString("ImageUrl", "/_layouts/images/back.gif");
                w.WriteAttributeString("Action", "Back");
                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndElement();

                #endregion

                w.WriteStartElement("Fields");

                foreach (DataRow row in _columnsTable.Rows)
                {
                    string columnName = row["ColumnName"].ToString();
                    string dataType = row["DataType"].ToString();

                    bool isReadOnly = (bool) row["IsReadOnly"];
                    bool isKey = (bool) row["IsKey"];
                    bool isAutoIncrement = (bool) row["IsAutoIncrement"];
                    bool allowDbNull = ((bool) row["AllowDBNull"]);

                    string newRecord = (isReadOnly || isAutoIncrement ? "Disabled" : "Enabled");
                    string editRecord = newRecord;

                    int maxLength = Int32.Parse(row["ColumnSize"].ToString());
                    int columns = maxLength > 40 ? 40 : maxLength;
                    string control = ApropiatedControl(dataType, maxLength);

                    SqlFKPKInfo sqlFkPkInfo = SPSDbTools.GetFkPkInfoVersion(
                        Generator.GetGenerator().Connection,
                        columnName,
                        _selectedTable);

                    string required = (isAutoIncrement || !allowDbNull || sqlFkPkInfo != null).ToString().ToLower();

                    SqlDbType sqlDbType =
                        (SqlDbType) Enum.Parse(typeof (SqlDbType), row["DataTypeName"].ToString(), true);
                    string datatype = sqlDbType.ToString();

                    // Types not supported
                    if (datatype == "Xml")
                    {
                        w.WriteComment(string.Format("Field {0} type {1} is not supported by ActionDataBase",
                                                     columnName,
                                                     datatype));
                    }
                    else
                    {
                        Status(string.Format("Generating {0}", columnName));

                        w.WriteStartElement("Field");

                        w.WriteAttributeString("Name", columnName);
                        w.WriteAttributeString("Title", columnName);

                        w.WriteAttributeString("Type", datatype);
                        w.WriteAttributeString("Control", control);

                        w.WriteAttributeString("Required", required);
                        w.WriteAttributeString("DefaultValue", "");

                        if (dataType.Contains("Date"))
                        {
                            w.WriteAttributeString("DisplayFormat", "{0:d}");
                        }
                        else
                        {
                            w.WriteAttributeString("DisplayFormat", "");
                        }

                        w.WriteAttributeString("New", newRecord);
                        w.WriteAttributeString("Edit", editRecord);
                        w.WriteAttributeString("View", "Enabled");

                        if (control == "TextBox" && dataType.Contains("String") && maxLength <= 256)
                        {
                            w.WriteStartElement("TextBox");
                            w.WriteAttributeString("Columns", columns.ToString());
                            w.WriteAttributeString("MaxLength", maxLength.ToString());
                            w.WriteEndElement();
                        }

                        if (control == "CheckBox")
                        {
                            w.WriteStartElement("CheckBox");
                            w.WriteAttributeString("TextChecked", "Chekced");
                            w.WriteAttributeString("TextUnChecked", "UnChecked");
                            w.WriteEndElement();
                        }

                        if (control == "Memo")
                        {
                            w.WriteStartElement("Memo");
                            w.WriteAttributeString("Columns", "40");
                            w.WriteAttributeString("Rows", "15");
                            //w.WriteAttributeString("MaxLength", maxLength.ToString());
                            w.WriteEndElement();
                        }

                        if (control == "TextBox"
                            && (!dataType.Contains("String")
                                && !dataType.Contains("Date")))
                        {
                            w.WriteStartElement("TextBox");
                            w.WriteAttributeString("Columns", "20");
                            w.WriteAttributeString("RightToLeft", "true");
                            w.WriteEndElement();
                        }

                        if (sqlFkPkInfo != null)
                        {
                            // TextField="CompanyName" ValueField="CustomerID" Table="TabelName"
                            w.WriteStartElement("Lookup");
                            w.WriteAttributeString("ControlEditor", "DropDownList");
                            w.WriteAttributeString("TextField", sqlFkPkInfo.PKColumnName);
                            w.WriteAttributeString("ValueField", sqlFkPkInfo.PKColumnName);
                            w.WriteAttributeString("ValueFieldType", datatype);
                            w.WriteAttributeString("Table", sqlFkPkInfo.PKTable);
                            w.WriteEndElement();
                        }

                        w.WriteEndElement();
                    }
                }
                w.WriteEndElement();

                w.WriteEndElement();
                w.WriteEndDocument();

                txtFormXML.Text = FormatXML(sw.ToString());
                LoadXMLFormEditor();
                Status("ActionBase XML Form generated.");
            }
            else
            {
                Error("Run column info, prior to generate a form.");
            }
        }

        public void LoadXMLFormEditor()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof (SPSActionEditConfig));
                StringReader stringReader = new StringReader(txtFormXML.Text);
                _configEditor = (SPSActionEditConfig) serializer.Deserialize(stringReader);
                stringReader.Close();
                pgEditor.SelectedObject = _configEditor;
                pgEditor.ExpandAllGridItems();
            }
            catch (Exception ex)
            {
                Error("ActionDataBaseForm XML Error." + ex.Message);
            }

            #region OLD CODE

            //TextReader stringReader = new StringReader(txtFormXML.Text);
            //_dataSetXMLForm = new DataSet();

            //Stream s =
            //    Assembly.GetExecutingAssembly().GetManifestResourceStream(
            //        "SPSProfessional.ActionDataBase.Generator.Schemas.ActionEditConfig.xsd");

            //_dataSetXMLForm.ReadXmlSchema(s);            

            //XmlTextReader reader = new XmlTextReader(stringReader);

            //_dataSetXMLForm.ReadXml(reader);
            //_dataSetXMLForm.EnforceConstraints = true;

            //gridXMLForm.DataSource = _dataSetXMLForm.DefaultViewManager;

            //lstXMLForm.Items.Clear();

            //foreach (DataTable table in _dataSetXMLForm.Tables)
            //{
            //    lstXMLForm.Items.Add(table.TableName);
            //}

            #endregion
        }

        #endregion

        #region XML TOOLS

        public static string FormatXML(string sXML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(sXML);
            StringBuilder sb = new StringBuilder();
            XmlTextWriter xtw = new XmlTextWriter(new StringWriter(sb));
            xtw.Formatting = Formatting.Indented;
            xml.WriteTo(xtw);
            xtw.Close();
            return sb.ToString();
        }

        #endregion

        #region Editors Hilight Initializers

        private void InitializeXMLEditor(HilightRichTextBox control)
        {
            control.Settings.Keywords.Add("(\\w+)\\s*=");
            control.CompileRegexKeywords();

            control.Settings.Symbols.Add("<\\w+|");
            control.Settings.Symbols.Add("</\\w+|");
            control.Settings.Symbols.Add(">|");
            control.Settings.Symbols.Add("/>|");
            control.Settings.Symbols.Add("=");
            control.CompileRegexSymbols();

            control.Settings.EnableIntegers = true;
            control.Settings.EnableStrings = true;

            control.Settings.KeywordColor = Color.Purple;
            control.Settings.SymbolsColor = Color.Blue;
            control.Settings.StringColor = Color.Green;
        }

        private void InitializeSQLEditor(HilightRichTextBox control)
        {
            control.Settings.Keywords.Add("SELECT");
            control.Settings.Keywords.Add("FROM");
            control.Settings.Keywords.Add("WHERE");
            control.Settings.Keywords.Add("GROUP BY");
            control.Settings.Keywords.Add("HAVING");
            control.Settings.Keywords.Add("ORDER BY");
            control.Settings.Keywords.Add("ASC");
            control.Settings.Keywords.Add("DESC");
            control.Settings.Keywords.Add("INNER");
            control.Settings.Keywords.Add("LEFT");
            control.Settings.Keywords.Add("RIGHT");
            control.Settings.Keywords.Add("OUTER");
            control.Settings.Keywords.Add("JOIN");
            control.Settings.Keywords.Add("CROSS");
            control.Settings.Keywords.Add("UNIQUE");
            control.Settings.Keywords.Add("DISTINCT");
            control.Settings.Keywords.Add("AS");            
            control.Settings.Keywords.Add("TOP");            
            control.CompileKeywords();

            control.Settings.Symbols.Add("\\bNOT\\b|");
            control.Settings.Symbols.Add("\\bLIKE\\b|");
            control.Settings.Symbols.Add("=|");
            control.Settings.Symbols.Add("<|");
            control.Settings.Symbols.Add("!|");
            control.Settings.Symbols.Add("\\bALL\\b|");
            control.Settings.Symbols.Add("\\bAND\\b|");
            control.Settings.Symbols.Add("\\bOR\\b|");
            control.Settings.Symbols.Add("\\bIN\\b|");
            control.Settings.Symbols.Add("\\bANY\\b|");
            control.Settings.Symbols.Add("\\(|");
            control.Settings.Symbols.Add("\\)|");
            control.Settings.Symbols.Add("\\+|");
            control.Settings.Symbols.Add("\\-|");
            control.Settings.Symbols.Add("\\bSOME\\b|");
            control.Settings.Symbols.Add("\\bEXIST\\b|");
            control.Settings.Symbols.Add("\\bBETWEEN\\b");
            control.CompileRegexSymbols();

            control.Settings.EnableComments = false;
            control.Settings.EnableStrings = true;
            control.Settings.EnableIntegers = true;

            control.Settings.KeywordColor = Color.Blue;
            control.Settings.SymbolsColor = Color.Purple;
            control.Settings.StringColor = Color.Green;
        }

        #endregion


        private void lstErrors_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem selectedItem = lstErrors.SelectedItems[0];
            int line = Int32.Parse(selectedItem.SubItems[1].Text);
            int column = Int32.Parse(selectedItem.SubItems[2].Text);
            string xml = selectedItem.SubItems[3].Text;

            if (xml == "Grid")
            {
                tabs.SelectTab(2);
                txtGridXML.GoToLineAndColumn(line, column);
            }
            else
            {
                tabs.SelectTab(3);
                txtFormXML.GoToLineAndColumn( line, column);
            }
        }      

        private void ClipboardCopyAll(TextBoxBase txtBox, bool rtf)
        {
            
            if (rtf)
            {                                
                Clipboard.SetText(((RichTextBox)txtBox).Rtf, TextDataFormat.Rtf);
            }
            else
                Clipboard.SetText(txtBox.Text);
        }
    }
}