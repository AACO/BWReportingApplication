using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class MissionSession : BaseRelational {

        public Mission Mission { get; set; }
        public Session Session { get; set; }

        public MissionSession() : base() {
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Mission);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Session);

            return hashcode;
        }
    }
}
