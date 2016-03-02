using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class Mission : BaseDatabase {
        public string Name { get; set; }
        public Map Map { get; set; }

        public Mission() : base() {
        }

        public Mission(string name) : this() {
            Name = name;
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Name);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Map);

            return hashcode;
        }
    }
}
