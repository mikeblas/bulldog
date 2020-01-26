using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Bulldog
{
    class SQLServerDataSource : DataSource
    {
        private string tableName;
        private string queryText;
        private SqlConnection conn;
        private string actualQuery;
        private SqlDataReader reader;
        private SqlCommand cmd;
        private ColumnDescriptions descriptions;

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

                descriptions = new ColumnDescriptions(schemaTable);

                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Couldn't open connection for {this.Name}: \"{ex.Message}\"");
                return false;
            }
        }

        public override RowBatch ReadData()
        {
            RowBatch rb = new RowBatch(descriptions);

            if (reader.IsClosed)
            {
                rb.IsEndOfRows = true;
                return rb;
            }

            bool readResult;
            while ((readResult = reader.Read()) == true && rb.Count < 250)
            {
                rb.AddRow(reader);
            }

            if (readResult == false)
                reader.Close();

            return rb;
        }
    }
}
