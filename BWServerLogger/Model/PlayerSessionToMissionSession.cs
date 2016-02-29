using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class PlayerSessionToMissionSession : BaseRelational {

        public int PlayerId {
            get;
            set;
        }

        public int SessionId {
            get;
            set;
        }

        public PlayerSessionToMissionSession() : base() {
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, PlayerId);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, SessionId);

            return hashcode;
        }
    }
}
