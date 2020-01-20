using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;


namespace AntlrConsole2
{
    public class SQLServerDataDestination : DataDestination
    {
        private string tableName;
        private string queryText;
        private SqlConnection conn;

        public string TableName { get => tableName; set => tableName = value; }
        public string QueryText { get => queryText; set => queryText = value; }

        public SQLServerDataDestination(string name) : base(name)
        {
        }

        public override bool Prepare()
        {
            conn = new SqlConnection(this.ConnectionString);
            try
            {
                conn.Open();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Couldn't open connection for {this.Name}: \"{ex.Message}\"");
                return false;
            }
        }
    }
}
