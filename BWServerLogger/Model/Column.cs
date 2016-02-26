using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BWServerLogger.Model
{
    public class Column
    {
        public string Field
        {
            get;
            private set;
        }

        public string Type
        {
            get;
            private set;
        }

        public bool Null
        {
            get;
            private set;
        }

        public IndexType Key
        {
            get;
            private set;
        }

        public String Default
        {
            get;
            private set;
        }

        public bool AutoIncrement
        {
            get;
            private set;
        }

        public Column(String field, String type, String isNull, String key, String defaultValue, String autoIncrement)
        {
            Field = field;
            Type = type;
            Null = (isNull == "yes") ? true : false;
            Key = IndexType.NONE.FromString(key);
            Default = defaultValue;
            AutoIncrement = (autoIncrement == "auto_increment") ? true : false;
        }

        public override int GetHashCode()
        {
            int hashCode = 1;

            hashCode = _hashBuilder(hashCode, Field);
            hashCode = _hashBuilder(hashCode, Type);
            hashCode = _hashBuilder(hashCode, Null);
            hashCode = _hashBuilder(hashCode, Key);
            hashCode = _hashBuilder(hashCode, Default);
            hashCode = _hashBuilder(hashCode, AutoIncrement);

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            bool equals = false;

            if (obj is Column)
            {
                equals = this.GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        private int _hashBuilder(int currentHashCode, Object itemToAdd)
        {
            return 31 * currentHashCode + ((itemToAdd == null) ? 0 : itemToAdd.GetHashCode());
        }
    }
}
