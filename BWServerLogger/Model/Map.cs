using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 map in the database. Extends <see cref="BaseDatabase"/>
    /// </summary>
    /// <seealso cref="BaseDatabase"/>
    public class Map : BaseDatabase {
        /// <summary>
        /// Map name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Map active status
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Default constructor, sets active to true.
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public Map() : base() {
            Active = true;
        }

        /// <summary>
        /// Constructs a map with a given name, calls <see cref="Map()"/>
        /// </summary>
        /// <param name="name">map name</param>
        public Map(string name) : this() {
            Name = name;
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Name);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Active);

            return hashcode;
        }
    }
}
