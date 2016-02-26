using log4net;

using MySql.Data.MySqlClient;

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

        public string Name
        {
            get
            {
                return "Reporting Job";
            }
        }

        public ReportingJob()
        {
        }

        public void StartJob()
        {
            DoJob(false);
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
                if (e is ThreadAbortException)
                {
                    _logger.Info("Reporting Job Thread was aborted.");
                }
                else
                {
                    _logger.Error("Problem running job.", e);
                }
                
            }
        }

        private TimeSpan GetRepetitionInterval()
        {
            // get "now" date variables
            DateTime now = DateTime.Now;
            double nowMS = now.TimeOfDay.TotalMilliseconds;
            int nowDayOfWeek = (int)now.DayOfWeek;

            // set up loop variables
            int minDayOfWeek = -1;
            double minMS = -1;

            MySqlConnection connection = DatabaseUtil.OpenDataSource();
            foreach (Schedule schedule in new ScheduleDAO(connection).GetScheduleItems())
            {
                // get "scheduled" date variables
                int dayOfWeek = (int)schedule.DayOfTheWeek;
                double ms = schedule.TimeOfDay.TotalMilliseconds;

                // ensure every item compared is in the future or right now
                if (dayOfWeek < nowDayOfWeek || (dayOfWeek == nowDayOfWeek && ms < nowMS))
                {
                    dayOfWeek += 7;
                }

                // if min DoW is not set, or if the scheduled DoW is less than the min, check MS
                if (minDayOfWeek < 0 || (minDayOfWeek > dayOfWeek && minMS > ms))
                {
                    minDayOfWeek = dayOfWeek;
                    minMS = ms;
                }
            }

            connection.Close();

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
