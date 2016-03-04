using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 mission in the database. Extends <see cref="BaseDatabase"/>
    /// </summary>
    /// <seealso cref="BaseDatabase"/>
    public class Mission : BaseDatabase {
        /// <summary>
        /// Mission name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// <see cref="Map"/> the mission is on
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public Mission() : base() {
        }

        /// <summary>
        /// Constructs a mission with a given name, calls <see cref="Mission()"/>
        /// </summary>
        /// <param name="name">mission name</param>
        public Mission(string name) : this() {
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
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Map);

            return hashcode;
        }
    }
}
