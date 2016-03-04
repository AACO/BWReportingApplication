using BWServerLogger.Util;

using System.Collections.Generic;
using System.Text;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object to represent a MySQL table
    /// </summary>
    public class Table {
        /// <summary>
        /// Table name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Set of columns on the table
        /// </summary>
        public ISet<Column> Columns { get; private set; }

        /// <summary>
        /// List of indices on the table
        /// </summary>
        public IList<Index> Indices { get; private set; }

        /// <summary>
        /// Table constructor that takes everything that defines a table
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="cols">Columns on the table</param>
        /// <param name="indices">Indices on the table</param>
        public Table(string name, ISet<Column> cols, IList<Index> indices) {
            Name = name;
            Columns = cols;
            Indices = indices;
        }

        /// <summary>
        /// Creates an SQL string to create a table based on the set properties.
        /// </summary>
        /// <returns></returns>
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
                        AddColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(")");
                        break;
                    case IndexType.UNIQUE:
                        createTableQuery.Append("UNIQUE ");
                        goto case IndexType.NONUNIQUE;
                    case IndexType.NONUNIQUE:
                        createTableQuery.Append("KEY `");
                        createTableQuery.Append(index.Name);
                        createTableQuery.Append("` (");
                        AddColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(")");
                        break;
                    case IndexType.FOREIGN:
                        createTableQuery.Append("CONSTRAINT `");
                        createTableQuery.Append(index.Name);
                        createTableQuery.Append("` FOREIGN KEY (");
                        AddColumns(ref createTableQuery, index.Columns);
                        createTableQuery.Append(") REFERENCES `");
                        createTableQuery.Append(index.ReferenceTable);
                        createTableQuery.Append("` (");
                        AddColumns(ref createTableQuery, index.ReferenceColumns);
                        createTableQuery.Append(")");
                        break;
                }
            }

            // closing SQL
            createTableQuery.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8");

            return createTableQuery.ToString();
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashCode = 1;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Name);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Columns);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Indices);

            return hashCode;
        }

        /// <summary>
        /// Overrides the default equals method.
        /// Uses some nasty reflection so we only need one equals method for all database objects
        /// </summary>
        /// <param name="obj">Object to check for equality</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is Table) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        private void AddColumns(ref StringBuilder query, IList<string> columns) {
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
