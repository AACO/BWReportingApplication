using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BWServerLogger.Util;

namespace BWServerLogger.Model
{
    public class Map : BaseDatabase
    {
        public string Name
        {
            get;
            set;
        }

        public bool Active
        {
            get;
            set;
        }

        public Map() : base()
        {
            this.Active = true;
        }

        public Map(string name) : this()
        {
            this.Name = name;
        }
    }
}
