// File : SPSGridView.cs
// Date : 30/01/2008
// User : csegura
// Logs

using System;
using System.Data;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.WebControls;
using SPSProParts.SharePoint.Framework.Error;

namespace SPSProParts.SharePoint.Framework.Controls
{
    /// <summary>
    /// Standard grid, sets up styles, paging and header localization
    /// </summary>
    public class SPSGridView : SPGridView, ISPSErrorControl
    {
        #region eColType enum

        public enum eColType
        {
            Boundfield = 0,
            HyperLinkfield = 1,
            Templatecolumn = 2
        }

        #endregion

        private DataRowView _selectedDataRow;
        private string _sortExpression;
        private bool _hiligthSelectedRow;

        /// <summary>
        /// Fires when the page needs to supply a data source 
        /// </summary>
        public event EventHandler DoDataLoad;

        #region Constructor

        /// <summary>
        /// Sets up the grid the first time it is created
        /// </summary>        
        public SPSGridView()
        {
            Initialize();
        }

        private void Initialize()
        {
            CellPadding = 1;
            GridLines = GridLines.Horizontal;
            DataSource = null;
            AllowSorting = true;
            AllowPaging = false;
            HiligthSelectedRow = false;
        }

        #endregion

        #region Properties

        public bool DataGridLoaded
        {
            get { return (ViewState["DataGridLoaded"] != null && (bool) ViewState["DataGridLoaded"]); }
            set
            {
                ViewState["DataGridLoaded"] = value;
            }
        }

        protected bool DataGridInitialized
        {
            get { return (ViewState["DataGridInitialized"] != null && (bool) ViewState["DataGridInitialized"]); }
            set { ViewState["DataGridInitialized"] = value; }
        }

        public string SortExAndDirect
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    return string.Empty;
                else
                    return ViewState["SortExpression"].ToString();
            }
            set { ViewState["SortExpression"] = value; }
        }

        public int SelectedDataItem
        {
            set { SelectedDataItemIndex = value; }
        }

        /// <summary>
        /// Gets or sets the index of the selected data item.
        /// When the set value is zero an event different to selection was taked 
        /// then the SelectedDataItemIndex is seted with the first page row.
        /// </summary>
        /// <value>The index of the selected data item.</value>
        private int SelectedDataItemIndex
        {
            get
            {
                if (ViewState["SelectedDataItemIndex"] == null)
                    return 0;
                else
                    return (int) ViewState["SelectedDataItemIndex"];
            }
            set { ViewState["SelectedDataItemIndex"] = value; }
        }

        public DataRowView SelectedDataRow
        {
            get { return _selectedDataRow; }
        }

        public string SortStringDataField
        {
            get { return _sortExpression; }

            set { _sortExpression = value; }
        }

        public bool HiligthSelectedRow
        {
            get { return _hiligthSelectedRow; }
            set { _hiligthSelectedRow = value; }
        }

        #endregion

        #region Control Methods

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // set up standardized paging and sorting
            // these events only fire when ViewState is enabled
            if (EnableViewState && !DataGridInitialized)
            {
                // can't use !IsPostback here because this might be a postback 
                // but still the first time the grid has been loaded
                AllowSorting = true;
            }
            else if (!EnableViewState || Page == null)
            {
                AllowPaging = false;
                AllowSorting = false;
            }

            DataGridInitialized = false;

            // Setup Events
            Sorting += SPSGridView_Sorting;
            PageIndexChanging += SPSGridView_PageIndexChanging;
            RowDataBound += SPSGridView_RowDataBound;
        }
      
        /// <summary>
        /// Checks to see if data is required
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            //Setting Pager template
            //ControlBase.SetGridPaging(this);

            // only load data if either:
            // a) view state is disabled, so we need to fetch the data every time, or
            // b) view state is enabled, but either the data hasn't been loaded yet or a 
            // paging/sorting event has occurred
            if (!DataGridLoaded)
                InternalBinding();

            // Page can become null when we are connecting webparts
            if (Page != null)
            {
                ClientScriptManager clientScript = Page.ClientScript;
                string scriptName = "SPSGridView_Script";

                if (!clientScript.IsClientScriptBlockRegistered(scriptName))
                {
                    clientScript.RegisterClientScriptBlock(GetType(), scriptName, GetScript(), true);
                }
            }
        }


        /// <summary>
        /// Establishes the control hierarchy.
        /// SPSGrid view adds a attribute for each row with the row index
        /// </summary>
        protected override void PrepareControlHierarchy()
        {
            base.PrepareControlHierarchy();

            if ((PageSize*PageIndex) > SelectedDataItemIndex)
                SelectedIndex = 0;
            else
                SelectedIndex = SelectedDataItemIndex - (PageSize*PageIndex);

            for (int i = 0; i < Rows.Count; i++)
            {
                Rows[i].Attributes.Add("rowid", Rows[i].DataItemIndex.ToString());

                // Auto select current row
                if (i == SelectedIndex && HiligthSelectedRow)
                {
                    Rows[i].CssClass = "ms-selectednav";
                    Rows[i].Font.Bold = true;
                }
            }
        }

        #endregion

        #region Events

        private void SPSGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItemIndex == SelectedDataItemIndex)
                {
                    // Get the selected row
                    _selectedDataRow = ((DataRowView) e.Row.DataItem);
                }
            }
        }

        private void SPSGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (SortExAndDirect != e.SortExpression)
                SortExAndDirect = e.SortExpression;
            else
                SortExAndDirect = e.SortExpression + " DESC";

            SelectedDataItemIndex = 0;
            // Force binding
            ForceBinding();
        }

        private void SPSGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            PageIndex = e.NewPageIndex;
            SelectedDataItemIndex = (PageIndex*PageSize);
            // Force binding
            ForceBinding();
        }

        // OnDoDataLoad dispatches the event to delegates that
        // are registered with the DoDataLoad event.
        protected virtual void OnDoDataLoad(EventArgs e)
        {
            if (DoDataLoad != null && Page != null)
            {
                DoDataLoad(this, e);
            }
        }

        #endregion

        #region Engine

        /// <summary>
        /// Forces the binding.
        /// </summary>
        public void ForceBinding()
        {
            DataGridLoaded = false;
            InternalBinding();
        }


        /// <summary>
        /// Forces the binding.
        /// </summary>
        public void Bind()
        {
            InternalBinding();
        }

        /// <summary>
        /// Do de binding
        /// </summary>
        private void InternalBinding()
        {
            try
            {
                // set up the data source
                OnDoDataLoad(EventArgs.Empty);

                if (!DataGridLoaded)
                {
                    DataBind();

                    #region

                    //if (DataSource != null)
                    //{
                    //    if (EnableViewState)
                    //    {
                    //        // turn on/off paging as required (paging is disabled if view state is disabled)
                    //        if (DataSource is DataTable)
                    //            AllowPaging = (((DataTable) DataSource).Rows.Count > PageSize);
                    //        else if (DataSource is DataView)
                    //            AllowPaging = (((DataView) DataSource).Count > PageSize);
                    //        else if (DataSource is ICollection)
                    //            AllowPaging = (((ICollection) DataSource).Count > PageSize);

                    //        //this.FilterDataFields = this.SortStringDataField;

                    //        // set the flag so that we don't load the data on every postback,
                    //        // if the datagrid is set up with ViewState turned on
                    //        DataGridLoaded = true;
                    //    }
                    //    //DataBind();
                    //}

                    #endregion
                }
            }
            catch (Exception ex)
            {
                // SqlException -> problem with SqlDataSource
                // HttpException -> problem with bound columns
                if (OnError != null)
                    OnError(this, new SPSErrorArgs("SPSGridView", "Binding Grid: " + ex.Message, ex));
                DumpException("InternalBinding", ex);
            }

            DataGridLoaded = true;
        }

        /// <summary>
        /// Generate Script
        /// SPSGetRowId is an special function to locate the row id from a SPSGrid
        /// we can call this function in oder to select a determinied row
        /// </summary>
        /// <returns></returns>
        private string GetScript()
        {
            return
                @"                            
                    function SPSGetRowId(id)
                    {
                        var menu = MMU_GetMenuFromClientId(id);
                        while ((menu = menu.parentNode))
                        {                          
                          if (menu.getAttribute('rowid'))
                          {
                            return menu.getAttribute('rowid');
                          }
                        }
                        return 0;
                    }";
        }

        #endregion

        #region ISPSErrorControl Members

        public event SPSControlOnError OnError;

        #endregion

        [Conditional("DEBUG")]
        private void DumpException(string name, Exception ex)
        {
            Debug.WriteLine(string.Format("{0}", name));
            Debug.WriteLine(ex);
        }
    }
}