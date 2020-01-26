using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bulldog
{
    public class ColumnDescription
    {
        int columnOrdinal;
        string columnName;
        int numericScale;
        int numericPrecision;
        int columnSize;

        public int ColumnOrdinal { get => columnOrdinal; set => columnOrdinal = value; }
        public string ColumnName { get => columnName; set => columnName = value; }
        public int NumericScale { get => numericScale; set => numericScale = value; }
        public int NumericPrecision { get => numericPrecision; set => numericPrecision = value; }
        public int ColumnSize { get => columnSize; set => columnSize = value; }

        public ColumnDescription(DataRow row)
        {
            columnOrdinal = (int) row["ColumnOrdinal"];
            columnName = (string)row["ColumnName"];
            numericScale = (short)row["NumericScale"];
            numericPrecision = (short)row["NumericPrecision"];
            columnSize = (int)row["ColumnSize"];
        }
    }
}

