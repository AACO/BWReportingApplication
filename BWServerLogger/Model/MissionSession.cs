namespace BWServerLogger.Model {
    public class MissionSession : BaseDatabase {

        public Mission Mission {
            get;
            set;
        }

        public Session Session {
            get;
            set;
        }

        public int Length {
            get;
            set;
        }

        public bool Played {
            get;
            set;
        }

        public MissionSession() : base() {
        }
    }
}
