using System;

namespace BWServerLogger.Model {
    public class Schedule : BaseDatabase {
        public DayOfWeek DayOfTheWeek {
            get;
            set;
        }

        public TimeSpan TimeOfDay {
            get;
            set;
        }

        public Schedule() : base() {
        }

        public Schedule(int id, int dayOfTheWeek, string timeOfDay) : this() {
            Id = id;
            DayOfTheWeek = (DayOfWeek)dayOfTheWeek;
            TimeOfDay = TimeSpan.Parse(timeOfDay);
        }

        public override bool Equals(object obj) {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode() {
            return 31 * TimeOfDay.GetHashCode() + (31 * DayOfTheWeek.GetHashCode() + 17);
        }

        public override string ToString() {
            return "DayOfTheWeek: " + DayOfTheWeek.ToString() + ", TimeOfDay: " + TimeOfDay.ToString();
        }
    }
}
