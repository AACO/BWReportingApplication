using BWServerLogger.Util;

using System.Reflection;
using System.Text;

namespace BWServerLogger.Model {
    /// <summary>
    /// Abstract class with the base database atributes
    /// </summary>
    public abstract class BaseDatabase {
        /// <summary>
        /// Database ID value
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDatabase() {
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Id);

            return hashcode;
        }

        /// <summary>
        /// Overrides the default equals method.
        /// Uses some nasty reflection so we only need one equals method for all database objects
        /// </summary>
        /// <param name="obj">Object to check for equality</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            bool equals = false;

            if (GetType().Equals(obj.GetType())) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }

        /// <summary>
        /// Overrides the default ToString method.
        /// Uses some even nastier reflection so we only need one ToString method for all database objects
        /// </summary>
        /// <returns>A string representation of the object</returns>
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
