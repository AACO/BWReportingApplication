using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object to represent a MySQL table column
    /// </summary>
    public class Column {
        /// <summary>
        /// Column name
        /// </summary>
        public string Field { get; private set; }

        /// <summary>
        /// Column type
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// True if the column is nullable, false if it is not
        /// </summary>
        public bool Null { get; private set; }

        /// <summary>
        /// Default column value
        /// </summary>
        public string Default { get; private set; }

        /// <summary>
        /// True if the column automatically increments, false if it does not
        /// </summary>
        public bool AutoIncrement { get; private set; }

        /// <summary>
        /// Column constructor, takes strings from a MySQL query. Parses isNull and autoIncrement then calls <see cref="Column(string, string, bool, string, bool)"/>
        /// </summary>
        /// <param name="field">Column name</param>
        /// <param name="type">Column type</param>
        /// <param name="isNull">Is column nullable?</param>
        /// <param name="defaultValue">Column default value</param>
        /// <param name="autoIncrement">Does the column automatically increment?</param>
        public Column(string field, string type, string isNull, string defaultValue, string autoIncrement) : this(field, type, isNull == "", defaultValue, autoIncrement != "") {
        }

        /// <summary>
        /// Column constructor, take objects built for expected schema validation
        /// </summary>
        /// <param name="field">Column name</param>
        /// <param name="type">Column type</param>
        /// <param name="isNull">Is column nullable?</param>
        /// <param name="defaultValue">Column default value</param>
        /// <param name="autoIncrement">Does the column automatically increment?</param>
        public Column(string field, string type, bool isNull, string defaultValue, bool autoIncrement) {
            Field = field;
            Type = type;
            Null = isNull;
            Default = defaultValue;
            AutoIncrement = autoIncrement;
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashCode = 17;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Field);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Type);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Null);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Default);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, AutoIncrement);

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

            if (obj is Column) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }
    }
}
