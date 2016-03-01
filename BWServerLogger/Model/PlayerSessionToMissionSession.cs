using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class PlayerSessionToMissionSession : BaseRelational {

        public int PlayerSessionId {
            get;
            set;
        }

        public int MissionSessionId {
            get;
            set;
        }

        public PlayerSessionToMissionSession() : base() {
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, PlayerSessionId);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MissionSessionId);

            return hashcode;
        }
    }
}
