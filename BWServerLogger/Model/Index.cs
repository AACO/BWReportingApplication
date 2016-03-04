using BWServerLogger.Util;

using System.Collections.Generic;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object to represent a MySQL table index
    /// </summary>
    public class Index {
        /// <summary>
        /// Index name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// List of columns the index is for
        /// </summary>
        public IList<string> Columns { get; private set; }

        /// <summary>
        /// Type of the index <see cref="IndexType"/>
        /// </summary>
        public IndexType Type { get; private set; }

        /// <summary>
        /// For <see cref="IndexType.FOREIGN"/> only, the table the foreign index references
        /// </summary>
        public string ReferenceTable { get; private set; }

        /// <summary>
        /// For <see cref="IndexType.FOREIGN"/> only, the columns the foreign index references on the <see cref="ReferenceTable"/>
        /// </summary>
        public IList<string> ReferenceColumns { get; private set; }

        /// <summary>
        /// Constructor for a <see cref="IndexType.PRIMARY"/> index
        /// </summary>
        /// <param name="column">Name of the column the index references</param>
        public Index(string column) {
            Columns = new List<string>();
            Columns.Add(column);
            Type = IndexType.PRIMARY;
        }

        /// <summary>
        /// Constructor for a <see cref="IndexType.UNIQUE"/> or <see cref="IndexType.NONUNIQUE"/> index
        /// </summary>
        /// <param name="type">Index type</param>
        /// <param name="name">Index name</param>
        /// <param name="columns">List of column names the index is for</param>
        public Index(string type, string name, string columns) {
            // split input columns, and add them to our list
            Columns = CsvToList(columns);
            Type = IndexType.PRIMARY.FromString(type);
            Name = name == "" ? null : name;
        }

        /// <summary>
        /// Constructor for a <see cref="IndexType.UNIQUE"/> or <see cref="IndexType.NONUNIQUE"/> index
        /// </summary>
        /// <param name="type">Index type</param>
        /// <param name="name">Index name</param>
        /// <param name="columns">List of column names the index is for</param>
        public Index(IndexType type, string name, string columns) {
            // split input columns, and add them to our list
            Columns = CsvToList(columns);
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Constructor for a <see cref="IndexType.FOREIGN"/> index
        /// </summary>
        /// <param name="name">Index name</param>
        /// <param name="columns">List of column names the index is for</param>
        /// <param name="refTable">Table the index references</param>
        /// <param name="refColumns">List of column names the index references</param>
        public Index(string name, string columns, string refTable, string refColumns) : this(IndexType.FOREIGN, name, columns) {
            ReferenceTable = refTable;
            ReferenceColumns = CsvToList(refColumns);
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashCode = 17;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Name);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, Columns);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Type);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, ReferenceTable);
            hashCode = HashUtil.SimpleCollectionHashBuilderHelper(hashCode, ReferenceColumns);

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

            if (obj is Index) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        /// <summary>
        /// Helper method to take a comma seperated string and turn it into a list of strings.
        /// Removes "`" ticks from the string
        /// </summary>
        /// <param name="csv">a comma seperated string</param>
        /// <returns>a list of strings</returns>
        private IList<string> CsvToList(string csv) {
            IList<string> returnList = new List<string>();

            string[] colArray = csv.Split(',');
            foreach (string column in colArray) {
                returnList.Add(column.Replace("`", ""));
            }

            return returnList;
        }
    }
}
