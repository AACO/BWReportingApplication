using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents an A3 player in the database. Extends <see cref="BaseDatabase"/>
    /// </summary>
    /// <seealso cref="BaseDatabase"/>
    public class Player : BaseDatabase {
        private string playerName;

        /// <summary>
        /// The player name if the name contains a tag it will set <see cref="HasClanTag"/> to true.
        /// Will strip the player name of their tag
        /// </summary>
        public string Name {
            get {
                return playerName;
            }
            set {
                if (TagUtil.HasClanTag(value)) {
                    HasClanTag = true;
                }
                playerName = TagUtil.StripTags(value);
            }
        }

        /// <summary>
        /// Boolean to track if the object has been updated (true) or not (false)
        /// </summary>
        public bool Updated { get; set; }

        /// <summary>
        /// True if the player has a clan tag, false if the player does not.
        /// </summary>
        public bool HasClanTag { get; private set; }

        /// <summary>
        /// Default constructor, sets <see cref="HasClanTag"/> to false
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public Player() : base() {
            HasClanTag = false;
            Updated = false;
        }

        /// <summary>
        /// Constructs a player with a given name, calls <see cref="Player()"/>
        /// </summary>
        /// <param name="name">mission name</param>
        public Player(string name) : this() {
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
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, HasClanTag);

            return hashcode;
        }
    }
}
