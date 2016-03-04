using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 player session to mission session in the database. Extends <see cref="BaseRelational"/>
    /// </summary>
    /// <seealso cref="BaseRelational"/>
    public class PlayerSessionToMissionSession : BaseRelational {
        /// <summary>
        /// The player session id
        /// </summary>
        public int PlayerSessionId { get; set; }

        /// <summary>
        /// The mission session id
        /// </summary>
        public int MissionSessionId { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseRelational()"/>
        public PlayerSessionToMissionSession() : base() {
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, PlayerSessionId);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MissionSessionId);

            return hashcode;
        }
    }
}
