using System.Collections.Generic;

namespace BWServerLogger.Util {
    /// <summary>
    /// Utility to help build hash codes
    /// </summary>
    class HashUtil {
        /// <summary>
        /// Method to build a hash with a given current hash code, and a basic item to add
        /// </summary>
        /// <param name="currentHashCode"></param>
        /// <param name="itemToAdd"></param>
        /// <returns>A built hashcode</returns>
        public static int SimpleObjectHashBuilderHelper(int currentHashCode, object itemToAdd) {
            return 23 * currentHashCode + ((itemToAdd == null) ? 0 : itemToAdd.GetHashCode());
        }

        /// <summary>
        /// Method to build a hash with a given current hash code, and a basic collection to add
        /// </summary>
        /// <param name="currentHashCode"></param>
        /// <param name="itemToAdd"></param>
        /// <returns>A built hashcode</returns>
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
