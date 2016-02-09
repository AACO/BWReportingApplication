using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using BWServerLogger.Util;

namespace BWServerLogger.Model
{
    public class Schedule : BaseDatabase
    {
        public DayOfWeek DayOfTheWeek
        {
            get;
            set;
        }

        public TimeSpan TimeOfDay
        {
            get;
            set;
        }

        public Schedule() : base()
        {
        }

        public Schedule(int id, int dayOfTheWeek, String timeOfDay) : this()
        {
            this.Id = id;
            this.DayOfTheWeek = (DayOfWeek)dayOfTheWeek;
            this.TimeOfDay = TimeSpan.Parse(timeOfDay);
        }

        public override int GetHashCode()
        {
            return 31 * TimeOfDay.GetHashCode() + (31 * DayOfTheWeek.GetHashCode() + 17);
        }

        public override string ToString()
        {
            return "DayOfTheWeek: " + DayOfTheWeek.ToString() + ", TimeOfDay: " + TimeOfDay.ToString();
        }
    }
}
