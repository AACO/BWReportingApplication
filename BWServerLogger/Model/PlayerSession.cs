namespace BWServerLogger.Model {
    public class PlayerSession : BaseDatabase {

        public Player Player {
            get;
            set;
        }

        public Session Session {
            get;
            set;
        }

        public int Length {
            get;
            set;
        }

        public bool Played {
            get;
            set;
        }

        public PlayerSession() : base() {
        }

        public override int GetHashCode() {
            return 31 * Player.GetHashCode() + (31 * Session.GetHashCode() + 17);
        }
    }
}
