using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Threading;

using BWServerLogger.Exceptions;
using BWServerLogger.DAO;
using BWServerLogger.Model;
using BWServerLogger.Service;
using BWServerLogger.Util;

namespace BWServerLogger.Job {
    /// <summary>
    /// Job to schedule and kick off the remoting job
    /// </summary>
    class ReportingJob {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingJob));

        /// <summary>
        /// Boolean to track reporting status
        /// </summary>
        public bool IsReporting { get; private set; }

        /// <summary>
        /// Constructor to create a reporting job.
        /// </summary>
        public ReportingJob() {
            IsReporting = false;
        }

        /// <summary>
        /// Method to start reporting
        /// </summary>
        /// <param name="parameter">bool to see if we should start reporting (true), or start waiting (false)</param>
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

        /// <summary>
        /// Method to start reporting, sets <see cref="IsReporting"/> flag to true (then false when reporting is done)
        /// </summary>
        private void StartReporting() {
            IsReporting = true;
            _logger.Info("Reporting started");
            new ReportingService().StartReporting();
            IsReporting = false;
        }

        /// <summary>
        /// Helper method to determine the wait time between report runs
        /// </summary>
        /// <returns>the wait time between report runs</returns>
        private TimeSpan GetRepetitionInterval() {
            // get "now" date variables
            DateTime now = DateTime.Now;
            double nowMS = now.TimeOfDay.TotalMilliseconds;
            int nowDayOfWeek = (int)now.DayOfWeek;

            // set up loop variables
            int minDayOfWeek = -1;
            double minMS = -1;

            MySqlConnection connection = null;
            ScheduleDAO scheduleDAO = null;
            try {
                connection = DatabaseUtil.OpenDataSource();
                scheduleDAO = new ScheduleDAO(connection);

                foreach (Schedule schedule in scheduleDAO.GetScheduleItems()) {
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
            }
            finally {
                if (connection != null) {
                    connection.Dispose();
                }
                if (scheduleDAO != null) {
                    scheduleDAO.Dispose();
                }
            }

            if (minMS < 0 || minDayOfWeek < 0) {
                throw new NoScheduleException("No schedules found, can not schedule next report run.");
            } else {
                TimeSpan returnTimeSpan = TimeSpan.FromDays(minDayOfWeek - nowDayOfWeek).Add(TimeSpan.FromMilliseconds(minMS - nowMS));
                _logger.DebugFormat("Next reporter run scheduled for: {0}", DateTime.Now.Add(returnTimeSpan).ToString());
                return returnTimeSpan;
            }
        }
    }
}
