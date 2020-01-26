using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;


namespace Bulldog
{
    public class SQLServerDataDestination : DataDestination
    {
        private string tableName;
        private string queryText;
        private SqlConnection conn;
        private SqlCommand command;
        private ColumnDescriptions descriptions;

        Dictionary<string, string> mapColumnToParameter = new Dictionary<string, string>();

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

                String cmdText;

                if (TableName != null)
                {
                    //TODO: handle reduced column list
                    cmdText = $"SELECT * FROM {this.TableName}";
                }
                else
                {
                    //TOOD: deduece parameters
                    /*
                        SqlCommandBuilder.DeriveParameters(cmd);
                        foreach (SqlParameter p in cmd.Parameters)
                        {
                           Console.WriteLine(p.ParameterName);
                        }
                    */
                    cmdText = QueryText;
                }

                SqlCommand cmd = new SqlCommand(cmdText.ToString(), conn);

                SqlDataReader reader = cmd.ExecuteReader();

                DataTable schemaTable = reader.GetSchemaTable();

                this.descriptions = new ColumnDescriptions(schemaTable);

                reader.Close();

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
            for (RowBatch rb = InputComponent.ReadData(); !rb.IsEndOfRows; rb = InputComponent.ReadData())
            {
                InsertBatch(rb);
            }
            return null;
        }

        private void InsertBatch(RowBatch rb)
        {
            AssureStatement(rb);

            for (int n = 0; n < rb.Count; n++)
            {
                command.Parameters.Clear();

                foreach (ColumnDescription c in descriptions.Columns)
                {
                    string paramName = mapColumnToParameter[c.ColumnName];                    
                    command.Parameters.AddWithValue(paramName, rb.GetRowColumnValue(n, c.ColumnOrdinal));
                }

                command.ExecuteNonQuery();
            }
        }

        private void AssureStatement(RowBatch rb)
        {
            if (this.command != null)
                return;


            StringBuilder sb = new StringBuilder();
            sb.Append($"INSERT INTO {this.TableName} (");

            StringBuilder p = new StringBuilder();
            p.Append("VALUES (");
 
            int n = 0;
            foreach (ColumnDescription c in descriptions.Columns)
            {
                if (n != 0)
                {
                    sb.Append(", ");
                    p.Append(", ");
                }
                n++;

                string parameterName = $"@P{n}";
                sb.Append(c.ColumnName);
                p.Append(parameterName);

                mapColumnToParameter.Add(c.ColumnName, parameterName);
            }

            sb.Append(")");
            p.Append(")");

            string cmd = sb.ToString() + p.ToString();
            this.command = new SqlCommand(cmd, this.conn);
            Console.WriteLine($"Dest command is {cmd}");
        }
    }
}
