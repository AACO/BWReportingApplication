using BWServerLogger.Util;

using System.Collections.Generic;
using System.Text;

namespace BWServerLogger.Model {
    public class Table {
        public string Name {
            get;
            private set;
        }

        public ISet<Column> Columns {
            get;
            private set;
        }

        public IList<Index> Indices {
            get;
            private set;
        }

        public Table(string name, ISet<Column> cols, IList<Index> indices) {
            Name = name;
            Columns = cols;
            Indices = indices;
        }

        public string CreateTableSQL() {
            StringBuilder createTableQuery = new StringBuilder();

            // opening SQL
            createTableQuery.Append("create table `");
            createTableQuery.Append(Name);
            createTableQuery.Append("` (");

            // create columns
            bool first = true;
            foreach (Column col in Columns) {
                if (first) {
                    first = false;
                } else {
                    createTableQuery.Append(", ");
                }
                createTableQuery.Append("`");
                createTableQuery.Append(col.Field);
                createTableQuery.Append("` ");
                createTableQuery.Append(col.Type);
                createTableQuery.Append(" ");
                createTableQuery.Append(col.Null ? "" : "NOT NULL");
                createTableQuery.Append(" ");
                if (col.Default != "") {
                    createTableQuery.Append("DEFAULT ");
                    createTableQuery.Append(col.Default);
                    createTableQuery.Append(" ");
                }
                createTableQuery.Append(col.AutoIncrement ? "AUTO_INCREMENT" : "");
            }

            // create constraints
            foreach (Index index in Indices) {
                createTableQuery.Append(", ");

                switch (index.Type) {
                    case IndexType.PRIMARY:
                        createTableQuery.Append("PRIMARY KEY (");
                        _addColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(")");
                        break;
                    case IndexType.UNIQUE:
                        createTableQuery.Append("UNIQUE ");
                        goto case IndexType.NONUNIQUE;
                    case IndexType.NONUNIQUE:
                        createTableQuery.Append("KEY `");
                        createTableQuery.Append(index.Name);
                        createTableQuery.Append("` (");
                        _addColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(")");
                        break;
                    case IndexType.FOREIGN:
                        createTableQuery.Append("CONSTRAINT `");
                        createTableQuery.Append(index.Name);
                        createTableQuery.Append("` FOREIGN KEY (");
                        _addColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(") REFERENCES `");
                        createTableQuery.Append(index.ReferenceTable);
                        createTableQuery.Append("` (");
                        _addColumns(ref createTableQuery, index.ReferenceColumns);
                        createTableQuery.Append(")");
                        break;
                }
            }

            // closing SQL
            createTableQuery.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8");

            return createTableQuery.ToString();
        }

        public override int GetHashCode() {
            int hashCode = 1;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Name);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Columns);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Indices);

            return hashCode;
        }

        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is Table) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        private void _addColumns(ref StringBuilder query, IList<string> columns) {
            bool first = true;
            foreach (string col in columns) {
                if (first) {
                    first = false;
                } else {
                    query.Append(", ");
                }
                query.Append("`");
                query.Append(col);
                query.Append("`");
            }
        }
    }
}
