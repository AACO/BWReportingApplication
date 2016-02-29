using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class Map : BaseDatabase {
        public string Name {
            get;
            set;
        }

        public bool Active {
            get;
            set;
        }

        public Map() : base() {
            Active = true;
        }

        public Map(string name) : this() {
            Name = name;
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Name);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Active);

            return hashcode;
        }
    }
}
