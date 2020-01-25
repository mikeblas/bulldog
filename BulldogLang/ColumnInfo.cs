namespace Bulldog
{
    public class ColumnInfo
    {
        private string columnName;

        public ColumnInfo(string columnName)
        {
            this.columnName = columnName;
        }

        public string ColumnName { get => columnName; set => columnName = value; }
    }
}
