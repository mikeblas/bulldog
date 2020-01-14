using System;
using System.Collections.Generic;
using System.Text;

namespace AntlrConsole2
{
    public abstract class DataSource : DataComponent
    {
        public DataSource(string name) : base(name)
        {
            isAllColumns = false;
            listColumns = new List<ColumnInfo>();
        }

        private bool isAllColumns;
        private List<ColumnInfo> listColumns;

        private string executeString;
        private string connectionString;

        public string ConnectionString { get => connectionString; set => connectionString = value; }
        public string ExecuteString { get => executeString; set => executeString = value; }
        public bool IsAllColumns { get => isAllColumns; set => isAllColumns = value; }


        public void AddColumn(string str)
        {
            if (isAllColumns)
                throw new Exception("Can't have column list if all columns specified");

            listColumns.Add(new ColumnInfo(str));
        }

    }
}

