using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace BWServerLogger.Model
{
    public static class Extensions
    {
        public static IndexType FromString(this IndexType type, string indexType)
        {
            IndexType returnType = IndexType.NONE;

            if (indexType != null && indexType != "")
            {
                if (indexType == "PRI")
                {
                    returnType = IndexType.PRIMARY;
                }
                else if (indexType == "UNI")
                {
                    returnType = IndexType.UNIQUE;
                }
                else if (indexType == "MUL")
                {
                    returnType = IndexType.NONUNIQUE;
                }
            }

            return returnType;
        }
    }

    public enum IndexType
    {
        NONE = 0,
        PRIMARY = 1,
        UNIQUE = 2,
        NONUNIQUE = 3
    }
}
