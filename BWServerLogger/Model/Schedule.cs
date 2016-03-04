using BWServerLogger.Util;

using System;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object that represents a Schedule in the database. Extends <see cref="BaseDatabase"/>
    /// </summary>
    public class Schedule : BaseDatabase {
        /// <summary>
        /// The Day of the week the schedule is for
        /// </summary>
        public DayOfWeek DayOfTheWeek { get; set; }

        /// <summary>
        /// The time of day the schedule is for
        /// </summary>
        public TimeSpan TimeOfDay { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <seealso cref="BaseDatabase()"/>
        public Schedule() : base() {
        }

        /// <summary>
        /// Constructor with all the elements that make up a schedule
        /// </summary>
        /// <param name="id">ID of the schedule item</param>
        /// <param name="dayOfTheWeek">Day of the week of the schedule item</param>
        /// <param name="timeOfDay">Time of day of the schedule item</param>
        /// <seealso cref="Schedule()"/>
        public Schedule(int id, int dayOfTheWeek, string timeOfDay) : this() {
            Id = id;
            DayOfTheWeek = (DayOfWeek)dayOfTheWeek;
            TimeOfDay = TimeSpan.Parse(timeOfDay);
        }

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, base.GetHashCode());
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, DayOfTheWeek);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, TimeOfDay);

            return hashcode;
        }
    }
}
