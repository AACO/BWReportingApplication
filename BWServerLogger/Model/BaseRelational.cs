using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Abstract class with the base relational database atributes. Extends <see cref="BaseDatabase"/>
    /// </summary>
    /// <seealso cref="BaseDatabase"/>
    public abstract class BaseRelational : BaseDatabase {
        /// <summary>
        /// Length an item has been played for
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// Flag to mark the item as played
        /// </summary>
        public bool Played { get; set; }

        /// <summary>
        /// Default constructor sets <see cref="Length"/> to 0 and <see cref="Played"/> to false
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public BaseRelational() : base() {
            Length = 0;
            Played = false;
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Length);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Played);

            return hashcode;
        }
    }
}
