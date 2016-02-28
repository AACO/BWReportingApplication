namespace BWServerLogger.Model {
    public static class Extensions {
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

    public enum IndexType {
        PRIMARY = 0,
        UNIQUE = 1,
        NONUNIQUE = 2,
        FOREIGN = 3
    }
}
