using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Bulldog
{
    public class ColumnDescriptions
    {
        // map from ordinal to a description
        Dictionary<int, ColumnDescription> ordinalMap = new Dictionary<int, ColumnDescription>();

        public ColumnDescriptions(DataTable dt)
        {
            //For each field in the table...
            foreach (DataRow myField in dt.Rows)
            {
                ColumnDescription cd = new ColumnDescription(myField);
                ordinalMap.Add(cd.ColumnOrdinal, cd);
            }
        }

        public IEnumerable<ColumnDescription> Columns { get => ordinalMap.Values;  }
        
        public int ColumnCount { get => ordinalMap.Count; }
    }
}

