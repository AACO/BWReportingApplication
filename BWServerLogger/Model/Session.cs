using BWServerLogger.Util;

using System;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents a game session in the database. Extends <see cref="BaseDatabase"/>
    /// </summary>
    /// <seealso cref="BaseDatabase"/>
    public class Session : BaseDatabase {
        /// <summary>
        /// Server host name
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Server version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Max players that attended the session
        /// </summary>
        public int MaxPlayers { get; set; }

        /// <summary>
        /// Maximum ping observed
        /// </summary>
        public long MaxPing { get; set; }

        /// <summary>
        /// Minimum ping observed
        /// </summary>
        public long MinPing { get; set; }

        /// <summary>
        /// Date the session took place on
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public Session() : base() {
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, HostName);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Version);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MaxPlayers);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MaxPing);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MinPing);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Date);

            return hashcode;
        }
    }
}
