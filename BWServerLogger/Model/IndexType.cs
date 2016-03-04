namespace BWServerLogger.Model {
    /// <summary>
    /// Extention class to add a method to an enum
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// Extention method to turn a string into a <see cref="IndexType"/> enum
        /// </summary>
        /// <param name="type">Index type enum, automatically passed in</param>
        /// <param name="indexType">Index type string to turn into the enum</param>
        /// <returns></returns>
        public static IndexType FromString(this IndexType type, string indexType) {
            IndexType returnType = IndexType.NONUNIQUE;

            if (indexType != null && indexType != "") {
                if (indexType == "PRIMARY") {
                    returnType = IndexType.PRIMARY;
                } else if (indexType == "UNIQUE") {
                    returnType = IndexType.UNIQUE;
                } else if (indexType == "FOREIGN") {
                    returnType = IndexType.FOREIGN;
                }
            }

            return returnType;
        }
    }

    /// <summary>
    /// Enum for the index type
    /// </summary>
    public enum IndexType {
        /// <summary>
        /// Primary index
        /// </summary>
        PRIMARY = 0,

        /// <summary>
        /// Unique index
        /// </summary>
        UNIQUE = 1,

        /// <summary>
        /// Nonunique index
        /// </summary>
        NONUNIQUE = 2,

        /// <summary>
        /// Foreign key
        /// </summary>
        FOREIGN = 3
    }
}
