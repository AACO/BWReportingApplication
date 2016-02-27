namespace BWServerLogger.Model {
    public class Mission : BaseDatabase {
        public string Name {
            get;
            set;
        }

        public Map Map {
            get;
            set;
        }

        public Mission() : base() {
        }

        public Mission(string name) : this() {
            Name = name;
        }
    }
}
