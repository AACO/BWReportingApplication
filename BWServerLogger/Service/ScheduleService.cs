using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using BWServerLogger.DAO;
using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.Service
{
    class ScheduleService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ScheduleService));

        private const int TIMER_OVERLAP = 1000;

        private volatile ISet<Schedule> _cachedScheduleItems;
        private volatile bool _kill;

        private ScheduleDAO _scheduleDAO;
        private MySqlConnection _connection;

        private Thread _reportingThread;
        private ReportingService _reportingService;

        public ScheduleService()
        {
            _connection = DatabaseUtil.OpenDataSource();
            _scheduleDAO = new ScheduleDAO(_connection);
        }

        ~ScheduleService()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        public void Start()
        {
            _kill = false;
            while (!_kill)
            {
                CheckScheduleAndRun();
                Thread.Sleep(Properties.Settings.Default.scheduleCheckRate);
            }
        }

        public void CheckScheduleAndRun()
        {
            if (GetCachedScheduleItems() != null)
            {
                DateTime now = DateTime.Now;
                DateTime offSet = now.AddMilliseconds(-1 * (Properties.Settings.Default.scheduleCheckRate + TIMER_OVERLAP));
                DayOfWeek dayOfWeek = now.DayOfWeek;

                foreach (Schedule schedule in _cachedScheduleItems)
                {
                    if (schedule.DayOfTheWeek == dayOfWeek &&
                        GetMSFromTimeSpan(schedule.TimeOfDay) >= GetMSFromDateTime(offSet) &&
                        GetMSFromTimeSpan(schedule.TimeOfDay) <= GetMSFromDateTime(now))
                    {
                        StartReportingThread();
                    }

                }
            }
        }

        public void Kill()
        {
            StopReportingThread();
            _kill = true;
            ClearCachedScheduleItems();
        }

        public void ClearCachedScheduleItems()
        {
            _cachedScheduleItems = null;
        }

        public ISet<Schedule> GetCachedScheduleItems()
        {
            if (_cachedScheduleItems == null)
            {
                _cachedScheduleItems = _scheduleDAO.GetScheduleItems();
            }
            return _cachedScheduleItems;
        }

        public void RemoveScheduleItem(Schedule scheduleItem)
        {
            _scheduleDAO.RemoveScheduleItem(scheduleItem);
            ClearCachedScheduleItems();
        }

        public void SaveScheduleItem(Schedule scheduleItem)
        {
            _scheduleDAO.SaveScheduleItem(scheduleItem);
            ClearCachedScheduleItems();
        }

        public void StartReportingThread()
        {
            if (IsReportRunning())
            {
                _logger.Warn("Report already running, not creating new thread");
            }
            else
            {
                try
                {
                    _reportingService = new ReportingService();
                    _reportingThread = new Thread(_reportingService.StartReporting);
                    _reportingThread.Start();
                }
                catch (Exception e)
                {
                    _logger.Error("Problem running report: ", e);
                }
            }

        }

        public void StopReportingThread()
        {
            if (!IsReportRunning())
            {
                _logger.Warn("Report already stopped");
            }
            else
            {
                try
                {
                    _reportingService.Kill();
                    _reportingThread.Join();
                }
                catch (Exception e)
                {
                    _logger.Error("Problem stopping report: ", e);
                }
            }
        }

        public bool IsReportRunning()
        {
            return _reportingThread != null && _reportingThread.IsAlive;
        }

        private double GetMSFromTimeSpan(TimeSpan timeSpan)
        {
            return timeSpan.TotalMilliseconds;
        }

        private double GetMSFromDateTime(DateTime dateTime)
        {
            return dateTime.TimeOfDay.TotalMilliseconds;
        }
    }
}
