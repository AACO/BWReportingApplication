using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class Player : BaseDatabase {
        private string playerName;

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

        public bool HasClanTag {
            get;
            set;
        }

        public Player() : base() {
            HasClanTag = false;
        }

        public Player(string name) : this() {
            Name = name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
