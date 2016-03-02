using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class PlayerSession : BaseRelational {

        public Player Player {
            get;
            set;
        }

        public Session Session {
            get;
            set;
        }

        public PlayerSession() : base() {
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Player);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Session);

            return hashcode;
        }
    }
}
