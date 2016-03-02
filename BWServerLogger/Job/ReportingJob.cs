using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;

using BWServerLogger.Exceptions;
using BWServerLogger.DAO;
using BWServerLogger.Model;
using BWServerLogger.Service;
using BWServerLogger.Util;

namespace BWServerLogger.Job {
    class ReportingJob {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingJob));

        public bool IsReporting { get; private set; }

        public ReportingJob() {
            IsReporting = false;
        }

        public void DoJob(object parameter) {
            bool forced = (bool)parameter;
            try {
                while (true) {
                    if (forced) {
                        StartReporting();
                        Thread.Sleep(GetRepetitionInterval());
                    } else {
                        Thread.Sleep(GetRepetitionInterval());
                        StartReporting();
                    }
                }
            } catch (Exception e) {
                IsReporting = false;
                if (e is ThreadAbortException) {
                    _logger.Info("Reporting Job Thread was aborted.");
                    
                } else {
                    _logger.Error("Problem running job.", e);
                }

            }
        }

        private void StartReporting() {
            IsReporting = true;
            new ReportingService().StartReporting();
            IsReporting = false;
        }

        private TimeSpan GetRepetitionInterval() {
            // get "now" date variables
            DateTime now = DateTime.Now;
            double nowMS = now.TimeOfDay.TotalMilliseconds;
            int nowDayOfWeek = (int)now.DayOfWeek;

            // set up loop variables
            int minDayOfWeek = -1;
            double minMS = -1;

            MySqlConnection connection = DatabaseUtil.OpenDataSource();
            foreach (Schedule schedule in new ScheduleDAO(connection).GetScheduleItems()) {
                // get "scheduled" date variables
                int dayOfWeek = (int)schedule.DayOfTheWeek;
                double ms = schedule.TimeOfDay.TotalMilliseconds;

                // ensure every item compared is in the future or right now
                if (dayOfWeek < nowDayOfWeek || (dayOfWeek == nowDayOfWeek && ms < nowMS)) {
                    dayOfWeek += 7;
                }

                // if min DoW is not set, or if the scheduled DoW is less than the min, check MS
                if (minDayOfWeek < 0 || (minDayOfWeek > dayOfWeek && minMS > ms)) {
                    minDayOfWeek = dayOfWeek;
                    minMS = ms;
                }
            }

            connection.Close();

            if (minMS < 0 || minDayOfWeek < 0) {
                throw new NoScheduleException("No schedules found, can not schedule next report run.");
            } else {
                return TimeSpan.FromDays(minDayOfWeek - nowDayOfWeek).Add(TimeSpan.FromMilliseconds(minMS - nowMS));
            }
        }
    }
}
