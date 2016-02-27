using System.Text.RegularExpressions;

namespace BWServerLogger.Util {
    class TagUtil {
        private const string GENERAL_TAG_REGEX = " \\[.*\\]$";

        public static string StripTags(string name) {
            Regex tagCheck = new Regex(GENERAL_TAG_REGEX);
            return tagCheck.Replace(name, "");
        }

        public static bool HasClanTag(string name) {
            Regex tagCheck = new Regex(GENERAL_TAG_REGEX);
            return tagCheck.IsMatch(name);
        }
    }
}
