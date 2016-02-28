using BWServerLogger.Util;

using System.Collections.Generic;

namespace BWServerLogger.Model {
    public class Index {
        public string Name {
            get;
            private set;
        }

        public IList<string> Columns {
            get;
            private set;
        }

        public IndexType Type {
            get;
            private set;
        }

        public string ReferenceTable {
            get;
            private set;
        }

        public IList<string> ReferenceColumns {
            get;
            private set;
        }

        public Index(string column) {
            Columns = new List<string>();
            Columns.Add(column);
            Type = IndexType.PRIMARY;
        }

        public Index(string type, string name, string columns) {
            // split input columns, and add them to our list
            Columns = _csvToList(columns);
            Type = IndexType.PRIMARY.FromString(type);
            Name = name == "" ? null : name;
        }

        public Index(IndexType type, string name, string columns) {
            // split input columns, and add them to our list
            Columns = _csvToList(columns);
            Type = type;
            Name = name;
        }

        public Index(string name, string columns, string refTable, string refColumns) : this(IndexType.FOREIGN, name, columns) {
            ReferenceTable = refTable;
            ReferenceColumns = _csvToList(refColumns);
        }

        public override int GetHashCode() {
            int hashCode = 17;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Name);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Columns);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Type);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, ReferenceTable);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, ReferenceColumns);

            return hashCode;
        }

        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is Index) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        private IList<string> _csvToList(string csv) {
            IList<string> returnList = new List<string>();

            string[] colArray = csv.Split(',');
            foreach (string column in colArray) {
                returnList.Add(column.Replace("`", ""));
            }

            return returnList;
        }
    }
}
