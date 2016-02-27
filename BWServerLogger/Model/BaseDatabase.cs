namespace BWServerLogger.Model {
    public class BaseDatabase {
        public int Id {
            get;
            set;
        }

        public bool Updated {
            get;
            set;
        }

        public BaseDatabase() {
            Updated = false;
        }
    }
}
