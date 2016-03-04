using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 mission session in the database. Extends <see cref="BaseRelational"/>
    /// </summary>
    /// <seealso cref="BaseRelational"/>
    public class MissionSession : BaseRelational {
        /// <summary>
        /// The <see cref="Mission"/> played
        /// </summary>
        public Mission Mission { get; set; }

        /// <summary>
        /// The <see cref="Session"/> the <see cref="Mission"/> is played on
        /// </summary>
        public Session Session { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseRelational()"/>
        public MissionSession() : base() {
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Mission);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Session);

            return hashcode;
        }
    }
}
