using System;
using System.Collections.Generic;
using System.Text;

namespace AntlrConsole2
{
    public class DataComponent
    {
        private string name;
        public string Name { get => name; set => name = value; }

        public DataComponent(string name)
        {
            this.Name = name;
        }
    }
}
