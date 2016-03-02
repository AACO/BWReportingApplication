using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public abstract class BaseRelational : BaseDatabase {
        public int Length {
            get;
            set;
        }

        public bool Played {
            get;
            set;
        }

        public BaseRelational() : base() {
            Length = 0;
            Played = false;
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Length);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Played);

            return hashcode;
        }
    }
}
