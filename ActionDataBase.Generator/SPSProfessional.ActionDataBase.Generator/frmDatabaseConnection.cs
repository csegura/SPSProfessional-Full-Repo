using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace SPSProfessional.ActionDataBase.Generator
{
    public partial class frmDatabaseConnection : Form
    {
        private IList<string> servers;

        public frmDatabaseConnection()
        {
            InitializeComponent();
        }

        private void frmDatabaseConnection_Load(object sender, EventArgs e)
        {
            BackgroundWorker bwc = new BackgroundWorker();
            bwc.DoWork += DoWork;
            bwc.RunWorkerCompleted += OnCompleted;
            bwc.RunWorkerAsync();    //Run it
        }


        private void OnCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            cbxServers.DataSource = servers;
            cbxServers.Refresh();
            imgProgress.Image = null;
            lblStatus.Text = "";
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            imgProgress.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.progress;
            servers = SPSDbTools.GetActiveServers();
            
        }

        private void GetServers()
        {
            //GetServersInvoke();
            imgProgress.Image = global::SPSProfessional.ActionDataBase.Generator.Properties.Resources.progress;
            cbxServers.DataSource = SPSDbTools.GetActiveServers();
            cbxServers.Refresh();
            imgProgress.Image = null;         
        }


     

        private void btnTestConnection_Click(object sender, EventArgs e)
        {

            try
            {
                cbxDatabases.DataSource = SPSDbTools.GetDatabases(cbxServers.Text,
                                                                  txtUser.Text,
                                                                  txtPassword.Text,
                                                                  chkSSPI.Checked);
                lblStatus.Text = "Test Sucessfully.";
                //MessageBox.Show("Sucessfully.",
                //                "Database Connection",
                //                MessageBoxButtons.OK,
                //                MessageBoxIcon.Information);
            }
            catch (SPSDbToolsException ex)
            {
                lblStatus.Text = "";
                MessageBox.Show(ex.Message,
                                "ERROR",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Error);
            }
        }

        private void chkSSPI_CheckedChanged(object sender, EventArgs e)
        {
            txtUser.Enabled = !chkSSPI.Checked;
            txtPassword.Enabled = !chkSSPI.Checked;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            connBuilder.DataSource = cbxServers.Text;
            connBuilder.IntegratedSecurity = chkSSPI.Checked;
            connBuilder.UserID = txtUser.Text;
            connBuilder.Password = txtPassword.Text;
            connBuilder.InitialCatalog = cbxDatabases.Text;

            Generator.GetGenerator().ConnectionString = connBuilder.ConnectionString;
            Close();
        }
    }
}