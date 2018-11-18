using System;
using System.Collections.Generic;
using System.Text;

namespace SPSProfessional.SharePoint.WebParts.ActionDataBase
{
    class ActionDataBaseSelect
    {
        private readonly string _table;
        private readonly List<string> _columns;
        private readonly List<string> _parameters;

        public ActionDataBaseSelect(string table)
        {
            this._table = table;
            _columns = new List<string>();
            _parameters = new List<string>();
        }

        public List<string> Columns
        {
            get { return _columns; }
        }

        public List<string> Parameters
        {
            get { return _parameters; }
        }

        ///<summary>
        ///Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            string sql = "SELECT ";

            if (Columns.Count == 0)
            {
                Columns.Add("*");                    
            }
            
            foreach(string s in Columns)
            {
                sql += s + ",";
            }
            
            sql = sql.TrimEnd(',');

            sql += " FROM " + _table; 

            if (Parameters.Count > 0)
            {
                sql += " WHERE ";
                foreach (string s in Parameters)
                {
                    sql += string.Format("{0}=@{0} AND ", s);
                }
                
                sql = sql.Remove(sql.Length - 4, 4);
            }

            return sql;
        }
    }
}
