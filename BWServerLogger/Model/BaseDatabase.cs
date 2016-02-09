using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BWServerLogger.Util;

namespace BWServerLogger.Model
{
    public class BaseDatabase
    {
        public int Id
        {
            get;
            set;
        }

        public bool Updated
        {
            get;
            set;
        }

        public BaseDatabase()
        {
            Updated = false;
        }
    }
}
