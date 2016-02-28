using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class Column {
        public string Field {
            get;
            private set;
        }

        public string Type {
            get;
            private set;
        }

        public bool Null {
            get;
            private set;
        }

        public string Default {
            get;
            private set;
        }

        public bool AutoIncrement {
            get;
            private set;
        }

        public Column(string field, string type, string isNull, string defaultValue, string autoIncrement) {
            Field = field;
            Type = type;
            Null = (isNull == "yes") ? true : false;
            Default = defaultValue;
            AutoIncrement = (autoIncrement == "auto_increment") ? true : false;
        }

        public Column(string field, string type, bool isNull, string defaultValue, bool autoIncrement) {
            Field = field;
            Type = type;
            Null = isNull;
            Default = defaultValue;
            AutoIncrement = autoIncrement;
        }

        public override int GetHashCode() {
            int hashCode = 17;

            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Field);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Type);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Null);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, Default);
            hashCode = HashUtil.SimpleObjectHashBuilderHelper(hashCode, AutoIncrement);

            return hashCode;
        }

        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is Column) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }
    }
}
