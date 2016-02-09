using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BWServerLogger.Util;

namespace BWServerLogger.Model
{
    public class Mission : BaseDatabase
    {
        public string Name
        {
            get;
            set;
        }

        public Map Map
        {
            get;
            set;
        }

        public Mission() : base()
        {
        }

        public Mission(string name) : this()
        {
            this.Name = name;
        }
    }
}
