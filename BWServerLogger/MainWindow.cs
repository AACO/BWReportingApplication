using log4net;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using BWServerLogger.Model;
using BWServerLogger.Service;
using BWServerLogger.Util;

namespace BWServerLogger
{
    public partial class MainWindow : Form
    {
        private const string _TIME_VALIDATION = "^[0-9][0-9]:[0-9][0-9]:[0-9][0-9]$";
        private static readonly ILog _logger = LogManager.GetLogger(typeof(MainWindow));

        private bool _closeThreads = true;
        private Thread _scheduleThread;
        private ScheduleService _scheduleService;
        

        public MainWindow()
        {
            ConstructSchedulerThread();
            StartSchedulerThread();

            InitializeComponent();
        }

        private void CheckThreadStatuses()
        {
            if (IsSchedulerRunning())
            {
                this.SchedulerRunning.BackColor = Color.Green;
                this.ScheduleInput.Text = "Stop Scheduler";
            }
            else
            {
                this.SchedulerRunning.BackColor = Color.Red;
                this.ScheduleInput.Text = "Start Scheduler";
            }

            if (_scheduleService != null && _scheduleService.IsReportRunning())
            {
                this.ReportStatus.BackColor = Color.Green;
                this.ReportingInput.Text = "Stop Reporting";
            }
            else
            {
                this.ReportStatus.BackColor = Color.Red;
                this.ReportingInput.Text = "Start Reporting";
            }
        }

        private void ConstructSchedulerThread()
        {
            try
            {
                _scheduleService = new ScheduleService();
                _scheduleThread = new Thread(_scheduleService.Start);
            }
            catch (Exception e)
            {
                _logger.Error("Scheduler could not be constructed", e);
            }
        }

        private void ConstructSchedulerThreadIfNeeded()
        {
            if (_scheduleService == null || _scheduleThread == null)
            {
                ConstructSchedulerThread();
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // set up settings
            this.MySQLServerAddressInput.Text = Properties.Settings.Default.mySQLServerAddress;
            this.MySQLServerPortInput.Value = Convert.ToDecimal(Properties.Settings.Default.mySQLServerPort);
            this.MySQLServerDatabaseInput.Text = Properties.Settings.Default.mySQLServerDatabase;
            this.MySQLServerUsernameInput.Text = Properties.Settings.Default.mySQLServerUsername;
            this.MySQLServerPasswordInput.Text = DatabaseUtil.GetMySQLPassword();

            this.ArmA3MissionThresholdInput.Value = Properties.Settings.Default.missionThreshold;
            this.ArmA3PlayedThresholdInput.Value = Properties.Settings.Default.playedThreshold;
            this.ArmA3RunTimeThresholdInput.Value = ToSeconds(Properties.Settings.Default.runTimeThreshold);
            this.ArmA3ScheduleCheckRateInput.Value = ToSeconds(Properties.Settings.Default.scheduleCheckRate);
            this.ArmA3ServerAddressInput.Text = Properties.Settings.Default.armaServerAddress;
            this.ArmA3ServerPollRateInput.Value = ToSeconds(Properties.Settings.Default.pollRate);
            this.ArmA3ServerPortInput.Value = Properties.Settings.Default.armaServerPort;
            this.serverReconnectLimitInput.Value = ToSeconds(Properties.Settings.Default.retryTimeLimit);

            // set up scheduler
            this.scheduleBindingSource.Clear();
            if (_scheduleService != null && _scheduleService.GetCachedScheduleItems() != null)
            {
                foreach (Schedule scheduleItem in _scheduleService.GetCachedScheduleItems())
                {
                    this.scheduleBindingSource.Add(scheduleItem);
                }
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_closeThreads)
            {
                StopSchedulerThread();
            }
        }

        private void ScheduleInput_Click(object sender, EventArgs e)
        {
            if (IsSchedulerRunning())
            {
                StopSchedulerThread();
            }
            else
            {
                StartSchedulerThread();
            }
        }

        private int ToSeconds(int milliseconds)
        {
            return milliseconds / 1000;
        }

        private int ToMilliseconds(decimal seconds)
        {
            return (int) seconds * 1000;
        }

        private void Save_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.mySQLServerAddress = this.MySQLServerAddressInput.Text;
            Properties.Settings.Default.mySQLServerPort = Convert.ToString(this.MySQLServerPortInput.Value);
            Properties.Settings.Default.mySQLServerDatabase = this.MySQLServerDatabaseInput.Text;
            Properties.Settings.Default.mySQLServerUsername = this.MySQLServerUsernameInput.Text;
            DatabaseUtil.SetMySQLPassword(this.MySQLServerPasswordInput.Text);
            Properties.Settings.Default.missionThreshold = (int)this.ArmA3MissionThresholdInput.Value;
            Properties.Settings.Default.playedThreshold = (int)this.ArmA3PlayedThresholdInput.Value;
            Properties.Settings.Default.runTimeThreshold = ToMilliseconds(this.ArmA3RunTimeThresholdInput.Value);
            Properties.Settings.Default.scheduleCheckRate = ToMilliseconds(this.ArmA3ScheduleCheckRateInput.Value);
            Properties.Settings.Default.armaServerAddress = this.ArmA3ServerAddressInput.Text;
            Properties.Settings.Default.pollRate = ToMilliseconds(this.ArmA3ServerPollRateInput.Value);
            Properties.Settings.Default.armaServerPort = (int)this.ArmA3ServerPortInput.Value;
            Properties.Settings.Default.retryTimeLimit = ToMilliseconds(this.serverReconnectLimitInput.Value);

            Properties.Settings.Default.Save();
            MainWindow_Load(sender, e);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            MainWindow_Load(sender, e);
        }

        private void ScheduleGrid_CancelRowEdit(object sender, QuestionEventArgs e)
        {
            MainWindow_Load(sender, e);
        }

        private void ScheduleGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            this.scheduleBindingSource.Position = e.RowIndex;
            if (_scheduleService != null)
            {
                _logger.Error("Scheduler Service is null! can't save schedule items");
            }
            else
            {
                _scheduleService.SaveScheduleItem((Schedule)this.scheduleBindingSource.Current);
            }
            MainWindow_Load(sender, e);
        }

        private void StartSchedulerThread()
        {
            ConstructSchedulerThreadIfNeeded();
            try
            {
                _scheduleThread.Start();
                while (!_scheduleThread.IsAlive);
            }
            catch (Exception e)
            {
                _logger.Error("Scheduler could not be started", e);
            }
        }

        private void StopSchedulerThread()
        {
            try
            {
                _scheduleService.Kill();
                _scheduleThread.Join();
            }
            catch (Exception e)
            {
                _logger.Error("Scheduler could not be stopped", e);
            }
        }

        private bool IsSchedulerRunning()
        {
            return _scheduleThread != null && _scheduleThread.IsAlive;
        }

        private void ScheduleGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (_scheduleService == null)
            {
                _logger.Error("Scheduler service is null, can't remove a schedule item");
            }
            else
            {
                _scheduleService.RemoveScheduleItem((Schedule)e.Row.DataBoundItem);
            }
        }

        private void CloseInput_Click(object sender, EventArgs e)
        {
            _closeThreads = false;
            base.Close();
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            CheckThreadStatuses();
        }

        private void ReportingInput_Click(object sender, EventArgs e)
        {
            if (_scheduleService != null && _scheduleService.IsReportRunning())
            {
                ConstructSchedulerThreadIfNeeded();
                _scheduleService.StopReportingThread();
            }
            else
            {
                if (_scheduleService == null)
                {
                    _logger.Error("Scheduler Service is null! can't start reporting");
                }
                else
                {
                    _scheduleService.StartReportingThread();
                }
            }
        }

        private void ScheduleGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            ScheduleGrid.Rows[e.RowIndex].ErrorText = "";

            if (ScheduleGrid.Rows[e.RowIndex].IsNewRow)
            {
                return;
            }

            if (e.ColumnIndex == 2)
            {
                Regex validation = new Regex(_TIME_VALIDATION);
                if (!validation.IsMatch((string)e.FormattedValue))
                {
                    e.Cancel = true;
                    ScheduleGrid.Rows[e.RowIndex].ErrorText = "The time of day value must be in 'HH:mm:ss' format";
                }
            }
        }
    }
}
