using System.Text.RegularExpressions;

namespace BWServerLogger.Util {
    /// <summary>
    /// Utility to help deal with tags
    /// </summary>
    class TagUtil {
        private const string GENERAL_TAG_REGEX = " \\[.*\\]$";

        /// <summary>
        /// Method to strip tags from a string
        /// </summary>
        /// <param name="name">name to strip tags from</param>
        /// <returns>Name less tags</returns>
        public static string StripTags(string name) {
            Regex tagCheck = new Regex(GENERAL_TAG_REGEX);
            return tagCheck.Replace(name, "");
        }

        /// <summary>
        /// Method to check if a string contains tags
        /// </summary>
        /// <param name="name">name to search tags from</param>
        /// <returns>True if the name has tags, false if it does not.</returns>
        public static bool HasClanTag(string name) {
            Regex tagCheck = new Regex(GENERAL_TAG_REGEX);
            return tagCheck.IsMatch(name);
        }
    }
}
