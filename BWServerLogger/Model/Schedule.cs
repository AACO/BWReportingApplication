using BWServerLogger.Util;

using System;

namespace BWServerLogger.Model {
    public class Schedule : BaseDatabase {
        public DayOfWeek DayOfTheWeek { get; set; }
        public TimeSpan TimeOfDay { get; set; }

        public Schedule() : base() {
        }

        public Schedule(int id, int dayOfTheWeek, string timeOfDay) : this() {
            Id = id;
            DayOfTheWeek = (DayOfWeek)dayOfTheWeek;
            TimeOfDay = TimeSpan.Parse(timeOfDay);
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, DayOfTheWeek);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, TimeOfDay);

            return hashcode;
        }
    }
}
