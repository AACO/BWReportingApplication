using BWServerLogger.Util;

using System;

namespace BWServerLogger.Model {
    public class Session : BaseDatabase {
        public string HostName { get; set; }
        public string Version { get; set; }
        public int MaxPlayers { get; set; }
        public long MaxPing { get; set; }
        public long MinPing { get; set; }
        public DateTime Date { get; set; }

        public Session() : base() {
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, HostName);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Version);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MaxPlayers);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MaxPing);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MinPing);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Date);

            return hashcode;
        }
    }
}
