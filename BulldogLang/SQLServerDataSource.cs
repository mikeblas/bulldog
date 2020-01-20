using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AntlrConsole2
{
    class SQLServerDataSource : DataSource
    {
        private string tableName;
        private string queryText;
        private SqlConnection conn;
        private string actualQuery;
        private SqlDataReader reader;
        private SqlCommand cmd;

        public SQLServerDataSource(string name) : base(name)
        {
        }

        public string TableName { get => tableName; set => tableName = value; }
        public string QueryText { get => queryText; set => queryText = value; }

        public override bool Prepare()
        {
            conn = new SqlConnection(this.ConnectionString);
            try
            {
                conn.Open();

                if (QueryText != null)
                    actualQuery = QueryText;
                else
                {
                    //TODO: build up column list
                    actualQuery = $"SELECT * FROM {this.TableName}";
                }

                this.cmd = new SqlCommand(this.actualQuery, this.conn);

                this.reader = cmd.ExecuteReader();

                DataTable schemaTable = this.reader.GetSchemaTable();

                //For each field in the table...
                foreach (DataRow myField in schemaTable.Rows)
                {
                    //For each property of the field...
                    foreach (DataColumn myProperty in schemaTable.Columns)
                    {
                        //Display the field name and value.
                        Console.WriteLine($"{myProperty.ColumnName} = {myField[myProperty]}");
                    }
                    Console.WriteLine();
                }

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
