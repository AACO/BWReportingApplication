using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 player session in the database. Extends <see cref="BaseRelational"/>
    /// </summary>
    /// <seealso cref="BaseRelational"/>
    public class PlayerSession : BaseRelational {

        /// <summary>
        /// The <see cref="Player"/> who played
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// The <see cref="Session"/> the <see cref="Player"/> is played on
        /// </summary>
        public Session Session { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseRelational()"/>
        public PlayerSession() : base() {
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Player);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Session);

            return hashcode;
        }
    }
}
