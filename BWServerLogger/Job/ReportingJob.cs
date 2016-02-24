using log4net;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BWServerLogger.Exceptions;
using BWServerLogger.DAO;
using BWServerLogger.Model;
using BWServerLogger.Service;
using BWServerLogger.Util;

namespace BWServerLogger.Job
{
    class ReportingJob
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingJob));
        private ScheduleDAO _scheduleDAO;

        public string Name
        {
            get
            {
                return "Reporting Job";
            }
        }

        public ReportingJob(ScheduleDAO scheduleDAO)
        {
            _scheduleDAO = scheduleDAO;
        }

        public void StartJob()
        {
            DoJob(true);
        }

        public void ForceJobRun()
        {
            DoJob(true);
        }

        private void DoJob(bool forced)
        {
            try
            {
                while (true)
                {
                    if (forced)
                    {
                        new ReportingService().StartReporting();
                        Thread.Sleep(GetRepetitionInterval());
                    }
                    else
                    {
                        Thread.Sleep(GetRepetitionInterval());
                        new ReportingService().StartReporting();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Problem running job (possible interrupt?)", e);
            }
        }

        private TimeSpan GetRepetitionInterval()
        {
            // get "now" date variables
            DateTime now = DateTime.Now;
            double nowMS = now.TimeOfDay.Milliseconds;
            int nowDayOfWeek = (int)now.DayOfWeek;

            // set up loop variables
            int minDayOfWeek = -1;
            double minMS = -1;

            foreach (Schedule schedule in _scheduleDAO.GetScheduleItems())
            {
                // get "scheduled" date variables
                int dayOfWeek = (int)schedule.DayOfTheWeek;
                double ms = schedule.TimeOfDay.Milliseconds;

                // ensure every item compared is in the future or right now
                if (dayOfWeek > nowDayOfWeek || (dayOfWeek == nowDayOfWeek && ms < nowMS))
                {
                    dayOfWeek += 7;
                }

                // if min DoW is not set, or if the scheduled DoW is less than the min, check MS
                if (minDayOfWeek < 0 || minDayOfWeek > dayOfWeek)
                {
                    // if min ms is not set, or if the scheduled ms is less than the min, set the min values
                    if (minMS < 0 || ms > minMS)
                    {
                        minDayOfWeek = dayOfWeek;
                        minMS = ms;
                    }
                }
            }

            if (minMS < 0 || minDayOfWeek < 0)
            {
                throw new NoScheduleException("No schedules found, can not schedule next report run.");
            }
            else
            {
                return TimeSpan.FromDays(minDayOfWeek - nowDayOfWeek).Add(TimeSpan.FromMilliseconds(minMS - nowMS));
            }
        }
    }
}
