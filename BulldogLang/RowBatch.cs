using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Bulldog
{
    public class RowBatch
    {
        ColumnDescriptions descriptions;
        List<object[]> rows;
        bool isEndOfRows;
        
        public RowBatch(ColumnDescriptions descriptions)
        {
            this.descriptions = descriptions;
            this.isEndOfRows = false;
            this.rows = new List<object[]>();
        }

        public bool IsEndOfRows { get => isEndOfRows; set => isEndOfRows = value;  }

        public void AddRow(SqlDataReader reader)
        {
            Console.Out.WriteLine("=== Row");
            Object [] oa = new Object[descriptions.ColumnCount];
            int n = 0;
            foreach (ColumnDescription c in descriptions.Columns)
            {
                oa[n] = reader[c.ColumnOrdinal];
                Console.Out.WriteLine("   {0}", oa[n].ToString());
                n++;
            }

            rows.Add(oa);
        }

        public object GetRowColumnValue(int row, int col)
        {
            return rows[row][col];
        }

        public int Count { get => rows.Count; }
    }
}

