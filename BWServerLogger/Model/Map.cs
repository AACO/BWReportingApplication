
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
    }
}
