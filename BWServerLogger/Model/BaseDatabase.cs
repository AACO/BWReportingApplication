using BWServerLogger.Util;

using System.Reflection;
using System.Text;

namespace BWServerLogger.Model {
    public abstract class BaseDatabase {
        public int Id { get; set; }
        public bool Updated { get; set; }

        public BaseDatabase() {
            Updated = false;
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Id);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Updated);

            return hashcode;
        }

        public override bool Equals(object obj) {
            bool equals = false;

            if (GetType().Equals(obj.GetType())) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        public override string ToString() {
            StringBuilder returnString = new StringBuilder(GetType().Name);
            returnString.Append(": [ ");

            bool first = true;
            foreach (PropertyInfo propertyInfo in GetType().GetProperties()) {
                if (first) {
                    first = false;
                } else {
                    returnString.Append(", ");
                }
                returnString.Append(propertyInfo.Name);
                returnString.Append(": ");
                object value = propertyInfo.GetValue(this);
                returnString.Append(value == null ? "{NULL}" : value.ToString());
            }

            returnString.Append(" ]");
            return returnString.ToString();
        }
    }
}
