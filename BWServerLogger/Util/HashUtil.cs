using System.Collections.Generic;

namespace BWServerLogger.Util {
    class HashUtil {
        public static int SimpleObjectHashBuilderHelper(int currentHashCode, object itemToAdd) {
            return 23 * currentHashCode + ((itemToAdd == null) ? 0 : itemToAdd.GetHashCode());
        }

        public static int SimpleCollectionHashBuilderHelper<T>(int currentHashCode, ICollection<T> itemToAdd) {
            int returnHash = 23 * currentHashCode;
            if (itemToAdd != null && itemToAdd.Count > 0) {
                int totalHash = 0;
                foreach(T obj in itemToAdd) {
                    if (obj != null) {
                        totalHash += obj.GetHashCode();
                    }
                }
                returnHash += totalHash;
            }
            return returnHash;
        }
    }
}
