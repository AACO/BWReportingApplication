using System;

namespace BWServerLogger
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.TabbedWindow = new System.Windows.Forms.TabControl();
            this.Run = new System.Windows.Forms.TabPage();
            this.CloseInput = new System.Windows.Forms.Button();
            this.ReportingInput = new System.Windows.Forms.Button();
            this.ScheduleInput = new System.Windows.Forms.Button();
            this.Schedule = new System.Windows.Forms.TabPage();
            this.ScheduleGrid = new System.Windows.Forms.DataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dayOfTheWeekDataGridViewComboBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.timeOfDayDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.scheduleBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Settings = new System.Windows.Forms.TabPage();
            this.SettingsTable = new System.Windows.Forms.TableLayoutPanel();
            this.ServerReconnectLimitLabel = new System.Windows.Forms.Label();
            this.ArmA3RunTimeThresholdLabel = new System.Windows.Forms.Label();
            this.ArmA3ServerAddressInput = new System.Windows.Forms.TextBox();
            this.ArmA3ServerPortInput = new System.Windows.Forms.NumericUpDown();
            this.MySQLServerAddressLabel = new System.Windows.Forms.Label();
            this.MySQLServerPortLabel = new System.Windows.Forms.Label();
            this.MySQLServerDatabaseLabel = new System.Windows.Forms.Label();
            this.MySQLServerUsernameLabel = new System.Windows.Forms.Label();
            this.MySQLServerPasswordLabel = new System.Windows.Forms.Label();
            this.ArmA3ServerAddressLabel = new System.Windows.Forms.Label();
            this.ArmA3ServerPort = new System.Windows.Forms.Label();
            this.ArmA3ServerPollRateLabel = new System.Windows.Forms.Label();
            this.ArmA3PlayedThresholdLabel = new System.Windows.Forms.Label();
            this.ArmA3MissionThresholdLabel = new System.Windows.Forms.Label();
            this.MySQLServerAddressInput = new System.Windows.Forms.TextBox();
            this.MySQLServerPortInput = new System.Windows.Forms.NumericUpDown();
            this.MySQLServerDatabaseInput = new System.Windows.Forms.TextBox();
            this.MySQLServerUsernameInput = new System.Windows.Forms.TextBox();
            this.MySQLServerPasswordInput = new System.Windows.Forms.TextBox();
            this.ArmA3ServerPollRateInput = new System.Windows.Forms.NumericUpDown();
            this.ArmA3PlayedThresholdInput = new System.Windows.Forms.NumericUpDown();
            this.ArmA3MissionThresholdInput = new System.Windows.Forms.NumericUpDown();
            this.ArmA3RunTimeThresholdInput = new System.Windows.Forms.NumericUpDown();
            this.serverReconnectLimitInput = new System.Windows.Forms.NumericUpDown();
            this.Save = new System.Windows.Forms.Button();
            this.Cancel = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ReportStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.SchedulerRunning = new System.Windows.Forms.ToolStripStatusLabel();
            this.CloseUITip = new System.Windows.Forms.ToolTip(this.components);
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.TabbedWindow.SuspendLayout();
            this.Run.SuspendLayout();
            this.Schedule.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScheduleGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleBindingSource)).BeginInit();
            this.Settings.SuspendLayout();
            this.SettingsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3ServerPortInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MySQLServerPortInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3ServerPollRateInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3PlayedThresholdInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3MissionThresholdInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3RunTimeThresholdInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverReconnectLimitInput)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabbedWindow
            // 
            this.TabbedWindow.Controls.Add(this.Run);
            this.TabbedWindow.Controls.Add(this.Schedule);
            this.TabbedWindow.Controls.Add(this.Settings);
            this.TabbedWindow.Location = new System.Drawing.Point(12, 12);
            this.TabbedWindow.Name = "TabbedWindow";
            this.TabbedWindow.SelectedIndex = 0;
            this.TabbedWindow.Size = new System.Drawing.Size(610, 454);
            this.TabbedWindow.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.TabbedWindow.TabIndex = 0;
            // 
            // Run
            // 
            this.Run.Controls.Add(this.CloseInput);
            this.Run.Controls.Add(this.ReportingInput);
            this.Run.Controls.Add(this.ScheduleInput);
            this.Run.Location = new System.Drawing.Point(4, 22);
            this.Run.Name = "Run";
            this.Run.Padding = new System.Windows.Forms.Padding(3);
            this.Run.Size = new System.Drawing.Size(602, 428);
            this.Run.TabIndex = 0;
            this.Run.Text = "Run";
            this.Run.ToolTipText = "Run options";
            this.Run.UseVisualStyleBackColor = true;
            // 
            // CloseInput
            // 
            this.CloseInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseInput.Location = new System.Drawing.Point(208, 342);
            this.CloseInput.Name = "CloseInput";
            this.CloseInput.Size = new System.Drawing.Size(200, 80);
            this.CloseInput.TabIndex = 3;
            this.CloseInput.Text = "Close UI";
            this.CloseUITip.SetToolTip(this.CloseInput, "This closes the UI, but leaves the scheduler/report running");
            this.CloseInput.UseVisualStyleBackColor = true;
            this.CloseInput.Click += new System.EventHandler(this.CloseInput_Click);
            // 
            // ReportingInput
            // 
            this.ReportingInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReportingInput.Location = new System.Drawing.Point(208, 175);
            this.ReportingInput.Name = "ReportingInput";
            this.ReportingInput.Size = new System.Drawing.Size(200, 80);
            this.ReportingInput.TabIndex = 2;
            this.ReportingInput.UseVisualStyleBackColor = true;
            this.ReportingInput.Click += new System.EventHandler(this.ReportingInput_Click);
            // 
            // ScheduleInput
            // 
            this.ScheduleInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ScheduleInput.Location = new System.Drawing.Point(208, 6);
            this.ScheduleInput.Name = "ScheduleInput";
            this.ScheduleInput.Size = new System.Drawing.Size(200, 80);
            this.ScheduleInput.TabIndex = 1;
            this.ScheduleInput.UseVisualStyleBackColor = true;
            this.ScheduleInput.Click += new System.EventHandler(this.ScheduleInput_Click);
            // 
            // Schedule
            // 
            this.Schedule.Controls.Add(this.ScheduleGrid);
            this.Schedule.Location = new System.Drawing.Point(4, 22);
            this.Schedule.Name = "Schedule";
            this.Schedule.Padding = new System.Windows.Forms.Padding(3);
            this.Schedule.Size = new System.Drawing.Size(602, 428);
            this.Schedule.TabIndex = 1;
            this.Schedule.Text = "Schedule";
            this.Schedule.ToolTipText = "View and set schedule items";
            this.Schedule.UseVisualStyleBackColor = true;
            // 
            // ScheduleGrid
            // 
            this.ScheduleGrid.AllowUserToResizeColumns = false;
            this.ScheduleGrid.AllowUserToResizeRows = false;
            this.ScheduleGrid.AutoGenerateColumns = false;
            this.ScheduleGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ScheduleGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.ScheduleGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ScheduleGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.dayOfTheWeekDataGridViewComboBoxColumn,
            this.timeOfDayDataGridViewTextBoxColumn,
            this.updatedDataGridViewCheckBoxColumn});
            this.ScheduleGrid.DataSource = this.scheduleBindingSource;
            this.ScheduleGrid.Location = new System.Drawing.Point(6, 6);
            this.ScheduleGrid.MultiSelect = false;
            this.ScheduleGrid.Name = "ScheduleGrid";
            this.ScheduleGrid.Size = new System.Drawing.Size(590, 416);
            this.ScheduleGrid.TabIndex = 0;
            this.ScheduleGrid.CancelRowEdit += new System.Windows.Forms.QuestionEventHandler(this.ScheduleGrid_CancelRowEdit);
            this.ScheduleGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.ScheduleGrid_CellEndEdit);
            this.ScheduleGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ScheduleGrid_CellValidating);
            this.ScheduleGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.ScheduleGrid_UserDeletingRow);
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Visible = false;
            this.idDataGridViewTextBoxColumn.Width = 41;
            // 
            // dayOfTheWeekDataGridViewComboBoxColumn
            // 
            this.dayOfTheWeekDataGridViewComboBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dayOfTheWeekDataGridViewComboBoxColumn.DataPropertyName = "DayOfTheWeek";
            this.dayOfTheWeekDataGridViewComboBoxColumn.HeaderText = "Day Of The Week";
            this.dayOfTheWeekDataGridViewComboBoxColumn.Items.AddRange(new object[] {
            System.DayOfWeek.Sunday,
            System.DayOfWeek.Monday,
            System.DayOfWeek.Tuesday,
            System.DayOfWeek.Wednesday,
            System.DayOfWeek.Thursday,
            System.DayOfWeek.Friday,
            System.DayOfWeek.Saturday});
            this.dayOfTheWeekDataGridViewComboBoxColumn.Name = "dayOfTheWeekDataGridViewComboBoxColumn";
            this.dayOfTheWeekDataGridViewComboBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dayOfTheWeekDataGridViewComboBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dayOfTheWeekDataGridViewComboBoxColumn.ToolTipText = "Day for the application to run on";
            // 
            // timeOfDayDataGridViewTextBoxColumn
            // 
            this.timeOfDayDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.timeOfDayDataGridViewTextBoxColumn.DataPropertyName = "TimeOfDay";
            this.timeOfDayDataGridViewTextBoxColumn.HeaderText = "Time Of Day";
            this.timeOfDayDataGridViewTextBoxColumn.Name = "timeOfDayDataGridViewTextBoxColumn";
            this.timeOfDayDataGridViewTextBoxColumn.ToolTipText = "Time the application runs in HH:mm:ss format";
            // 
            // updatedDataGridViewCheckBoxColumn
            // 
            this.updatedDataGridViewCheckBoxColumn.DataPropertyName = "Updated";
            this.updatedDataGridViewCheckBoxColumn.HeaderText = "Updated";
            this.updatedDataGridViewCheckBoxColumn.Name = "updatedDataGridViewCheckBoxColumn";
            this.updatedDataGridViewCheckBoxColumn.Visible = false;
            this.updatedDataGridViewCheckBoxColumn.Width = 54;
            // 
            // scheduleBindingSource
            // 
            this.scheduleBindingSource.DataSource = typeof(BWServerLogger.Model.Schedule);
            // 
            // Settings
            // 
            this.Settings.AutoScroll = true;
            this.Settings.Controls.Add(this.SettingsTable);
            this.Settings.Controls.Add(this.Save);
            this.Settings.Controls.Add(this.Cancel);
            this.Settings.Location = new System.Drawing.Point(4, 22);
            this.Settings.Name = "Settings";
            this.Settings.Size = new System.Drawing.Size(602, 428);
            this.Settings.TabIndex = 2;
            this.Settings.Text = "Settings";
            this.Settings.ToolTipText = "Configure the application";
            this.Settings.UseVisualStyleBackColor = true;
            // 
            // SettingsTable
            // 
            this.SettingsTable.AutoScroll = true;
            this.SettingsTable.ColumnCount = 2;
            this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SettingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.SettingsTable.Controls.Add(this.ServerReconnectLimitLabel, 0, 11);
            this.SettingsTable.Controls.Add(this.ArmA3RunTimeThresholdLabel, 0, 10);
            this.SettingsTable.Controls.Add(this.ArmA3ServerAddressInput, 1, 5);
            this.SettingsTable.Controls.Add(this.ArmA3ServerPortInput, 1, 6);
            this.SettingsTable.Controls.Add(this.MySQLServerAddressLabel, 0, 0);
            this.SettingsTable.Controls.Add(this.MySQLServerPortLabel, 0, 1);
            this.SettingsTable.Controls.Add(this.MySQLServerDatabaseLabel, 0, 2);
            this.SettingsTable.Controls.Add(this.MySQLServerUsernameLabel, 0, 3);
            this.SettingsTable.Controls.Add(this.MySQLServerPasswordLabel, 0, 4);
            this.SettingsTable.Controls.Add(this.ArmA3ServerAddressLabel, 0, 5);
            this.SettingsTable.Controls.Add(this.ArmA3ServerPort, 0, 6);
            this.SettingsTable.Controls.Add(this.ArmA3ServerPollRateLabel, 0, 7);
            this.SettingsTable.Controls.Add(this.ArmA3PlayedThresholdLabel, 0, 8);
            this.SettingsTable.Controls.Add(this.ArmA3MissionThresholdLabel, 0, 9);
            this.SettingsTable.Controls.Add(this.MySQLServerAddressInput, 1, 0);
            this.SettingsTable.Controls.Add(this.MySQLServerPortInput, 1, 1);
            this.SettingsTable.Controls.Add(this.MySQLServerDatabaseInput, 1, 2);
            this.SettingsTable.Controls.Add(this.MySQLServerUsernameInput, 1, 3);
            this.SettingsTable.Controls.Add(this.MySQLServerPasswordInput, 1, 4);
            this.SettingsTable.Controls.Add(this.ArmA3ServerPollRateInput, 1, 7);
            this.SettingsTable.Controls.Add(this.ArmA3PlayedThresholdInput, 1, 8);
            this.SettingsTable.Controls.Add(this.ArmA3MissionThresholdInput, 1, 9);
            this.SettingsTable.Controls.Add(this.ArmA3RunTimeThresholdInput, 1, 10);
            this.SettingsTable.Controls.Add(this.serverReconnectLimitInput, 1, 11);
            this.SettingsTable.Location = new System.Drawing.Point(3, 3);
            this.SettingsTable.Name = "SettingsTable";
            this.SettingsTable.RowCount = 12;
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.SettingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.SettingsTable.Size = new System.Drawing.Size(596, 360);
            this.SettingsTable.TabIndex = 3;
            // 
            // ServerReconnectLimitLabel
            // 
            this.ServerReconnectLimitLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ServerReconnectLimitLabel.AutoSize = true;
            this.ServerReconnectLimitLabel.BackColor = System.Drawing.Color.Transparent;
            this.ServerReconnectLimitLabel.Location = new System.Drawing.Point(177, 338);
            this.ServerReconnectLimitLabel.Name = "ServerReconnectLimitLabel";
            this.ServerReconnectLimitLabel.Size = new System.Drawing.Size(118, 13);
            this.ServerReconnectLimitLabel.TabIndex = 28;
            this.ServerReconnectLimitLabel.Text = "Server Reconnect Limit";
            this.ServerReconnectLimitLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3RunTimeThresholdLabel
            // 
            this.ArmA3RunTimeThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3RunTimeThresholdLabel.AutoSize = true;
            this.ArmA3RunTimeThresholdLabel.BackColor = System.Drawing.Color.Transparent;
            this.ArmA3RunTimeThresholdLabel.Location = new System.Drawing.Point(155, 308);
            this.ArmA3RunTimeThresholdLabel.Name = "ArmA3RunTimeThresholdLabel";
            this.ArmA3RunTimeThresholdLabel.Size = new System.Drawing.Size(140, 13);
            this.ArmA3RunTimeThresholdLabel.TabIndex = 27;
            this.ArmA3RunTimeThresholdLabel.Text = "ArmA 3 Run Time Threshold";
            this.ArmA3RunTimeThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3ServerAddressInput
            // 
            this.ArmA3ServerAddressInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3ServerAddressInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3ServerAddressInput.Location = new System.Drawing.Point(301, 155);
            this.ArmA3ServerAddressInput.Name = "ArmA3ServerAddressInput";
            this.ArmA3ServerAddressInput.Size = new System.Drawing.Size(175, 20);
            this.ArmA3ServerAddressInput.TabIndex = 6;
            // 
            // ArmA3ServerPortInput
            // 
            this.ArmA3ServerPortInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3ServerPortInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3ServerPortInput.Location = new System.Drawing.Point(301, 185);
            this.ArmA3ServerPortInput.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.ArmA3ServerPortInput.Name = "ArmA3ServerPortInput";
            this.ArmA3ServerPortInput.Size = new System.Drawing.Size(75, 20);
            this.ArmA3ServerPortInput.TabIndex = 7;
            // 
            // MySQLServerAddressLabel
            // 
            this.MySQLServerAddressLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MySQLServerAddressLabel.AutoSize = true;
            this.MySQLServerAddressLabel.Location = new System.Drawing.Point(178, 8);
            this.MySQLServerAddressLabel.Name = "MySQLServerAddressLabel";
            this.MySQLServerAddressLabel.Size = new System.Drawing.Size(117, 13);
            this.MySQLServerAddressLabel.TabIndex = 16;
            this.MySQLServerAddressLabel.Text = "MySQL Server Address";
            this.MySQLServerAddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MySQLServerPortLabel
            // 
            this.MySQLServerPortLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MySQLServerPortLabel.AutoSize = true;
            this.MySQLServerPortLabel.Location = new System.Drawing.Point(197, 38);
            this.MySQLServerPortLabel.Name = "MySQLServerPortLabel";
            this.MySQLServerPortLabel.Size = new System.Drawing.Size(98, 13);
            this.MySQLServerPortLabel.TabIndex = 17;
            this.MySQLServerPortLabel.Text = "MySQL Server Port";
            this.MySQLServerPortLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MySQLServerDatabaseLabel
            // 
            this.MySQLServerDatabaseLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MySQLServerDatabaseLabel.AutoSize = true;
            this.MySQLServerDatabaseLabel.Location = new System.Drawing.Point(170, 68);
            this.MySQLServerDatabaseLabel.Name = "MySQLServerDatabaseLabel";
            this.MySQLServerDatabaseLabel.Size = new System.Drawing.Size(125, 13);
            this.MySQLServerDatabaseLabel.TabIndex = 18;
            this.MySQLServerDatabaseLabel.Text = "MySQL Server Database";
            this.MySQLServerDatabaseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MySQLServerUsernameLabel
            // 
            this.MySQLServerUsernameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MySQLServerUsernameLabel.AutoSize = true;
            this.MySQLServerUsernameLabel.Location = new System.Drawing.Point(168, 98);
            this.MySQLServerUsernameLabel.Name = "MySQLServerUsernameLabel";
            this.MySQLServerUsernameLabel.Size = new System.Drawing.Size(127, 13);
            this.MySQLServerUsernameLabel.TabIndex = 19;
            this.MySQLServerUsernameLabel.Text = "MySQL Server Username";
            this.MySQLServerUsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MySQLServerPasswordLabel
            // 
            this.MySQLServerPasswordLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MySQLServerPasswordLabel.AutoSize = true;
            this.MySQLServerPasswordLabel.Location = new System.Drawing.Point(170, 128);
            this.MySQLServerPasswordLabel.Name = "MySQLServerPasswordLabel";
            this.MySQLServerPasswordLabel.Size = new System.Drawing.Size(125, 13);
            this.MySQLServerPasswordLabel.TabIndex = 20;
            this.MySQLServerPasswordLabel.Text = "MySQL Server Password";
            this.MySQLServerPasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3ServerAddressLabel
            // 
            this.ArmA3ServerAddressLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3ServerAddressLabel.AutoSize = true;
            this.ArmA3ServerAddressLabel.Location = new System.Drawing.Point(179, 158);
            this.ArmA3ServerAddressLabel.Name = "ArmA3ServerAddressLabel";
            this.ArmA3ServerAddressLabel.Size = new System.Drawing.Size(116, 13);
            this.ArmA3ServerAddressLabel.TabIndex = 21;
            this.ArmA3ServerAddressLabel.Text = "ArmA 3 Server Address";
            this.ArmA3ServerAddressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3ServerPort
            // 
            this.ArmA3ServerPort.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3ServerPort.AutoSize = true;
            this.ArmA3ServerPort.Location = new System.Drawing.Point(198, 188);
            this.ArmA3ServerPort.Name = "ArmA3ServerPort";
            this.ArmA3ServerPort.Size = new System.Drawing.Size(97, 13);
            this.ArmA3ServerPort.TabIndex = 22;
            this.ArmA3ServerPort.Text = "ArmA 3 Server Port";
            this.ArmA3ServerPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3ServerPollRateLabel
            // 
            this.ArmA3ServerPollRateLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3ServerPollRateLabel.AutoSize = true;
            this.ArmA3ServerPollRateLabel.Location = new System.Drawing.Point(174, 218);
            this.ArmA3ServerPollRateLabel.Name = "ArmA3ServerPollRateLabel";
            this.ArmA3ServerPollRateLabel.Size = new System.Drawing.Size(121, 13);
            this.ArmA3ServerPollRateLabel.TabIndex = 23;
            this.ArmA3ServerPollRateLabel.Text = "ArmA 3 Server Poll Rate";
            this.ArmA3ServerPollRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3PlayedThresholdLabel
            // 
            this.ArmA3PlayedThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3PlayedThresholdLabel.AutoSize = true;
            this.ArmA3PlayedThresholdLabel.Location = new System.Drawing.Point(169, 248);
            this.ArmA3PlayedThresholdLabel.Name = "ArmA3PlayedThresholdLabel";
            this.ArmA3PlayedThresholdLabel.Size = new System.Drawing.Size(126, 13);
            this.ArmA3PlayedThresholdLabel.TabIndex = 25;
            this.ArmA3PlayedThresholdLabel.Text = "ArmA 3 Played Threshold";
            this.ArmA3PlayedThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ArmA3MissionThresholdLabel
            // 
            this.ArmA3MissionThresholdLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ArmA3MissionThresholdLabel.AutoSize = true;
            this.ArmA3MissionThresholdLabel.Location = new System.Drawing.Point(166, 278);
            this.ArmA3MissionThresholdLabel.Name = "ArmA3MissionThresholdLabel";
            this.ArmA3MissionThresholdLabel.Size = new System.Drawing.Size(129, 13);
            this.ArmA3MissionThresholdLabel.TabIndex = 26;
            this.ArmA3MissionThresholdLabel.Text = "ArmA 3 Mission Threshold";
            this.ArmA3MissionThresholdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MySQLServerAddressInput
            // 
            this.MySQLServerAddressInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MySQLServerAddressInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MySQLServerAddressInput.Location = new System.Drawing.Point(301, 5);
            this.MySQLServerAddressInput.Name = "MySQLServerAddressInput";
            this.MySQLServerAddressInput.Size = new System.Drawing.Size(175, 20);
            this.MySQLServerAddressInput.TabIndex = 1;
            // 
            // MySQLServerPortInput
            // 
            this.MySQLServerPortInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MySQLServerPortInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MySQLServerPortInput.Location = new System.Drawing.Point(301, 35);
            this.MySQLServerPortInput.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.MySQLServerPortInput.Name = "MySQLServerPortInput";
            this.MySQLServerPortInput.Size = new System.Drawing.Size(75, 20);
            this.MySQLServerPortInput.TabIndex = 2;
            // 
            // MySQLServerDatabaseInput
            // 
            this.MySQLServerDatabaseInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MySQLServerDatabaseInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MySQLServerDatabaseInput.Location = new System.Drawing.Point(301, 65);
            this.MySQLServerDatabaseInput.Name = "MySQLServerDatabaseInput";
            this.MySQLServerDatabaseInput.Size = new System.Drawing.Size(175, 20);
            this.MySQLServerDatabaseInput.TabIndex = 3;
            // 
            // MySQLServerUsernameInput
            // 
            this.MySQLServerUsernameInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MySQLServerUsernameInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MySQLServerUsernameInput.Location = new System.Drawing.Point(301, 95);
            this.MySQLServerUsernameInput.Name = "MySQLServerUsernameInput";
            this.MySQLServerUsernameInput.Size = new System.Drawing.Size(175, 20);
            this.MySQLServerUsernameInput.TabIndex = 4;
            // 
            // MySQLServerPasswordInput
            // 
            this.MySQLServerPasswordInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.MySQLServerPasswordInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MySQLServerPasswordInput.Location = new System.Drawing.Point(301, 125);
            this.MySQLServerPasswordInput.Name = "MySQLServerPasswordInput";
            this.MySQLServerPasswordInput.PasswordChar = '*';
            this.MySQLServerPasswordInput.Size = new System.Drawing.Size(175, 20);
            this.MySQLServerPasswordInput.TabIndex = 5;
            this.MySQLServerPasswordInput.UseSystemPasswordChar = true;
            // 
            // ArmA3ServerPollRateInput
            // 
            this.ArmA3ServerPollRateInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3ServerPollRateInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3ServerPollRateInput.Location = new System.Drawing.Point(301, 215);
            this.ArmA3ServerPollRateInput.Maximum = new decimal(new int[] {
            2147483,
            0,
            0,
            0});
            this.ArmA3ServerPollRateInput.Name = "ArmA3ServerPollRateInput";
            this.ArmA3ServerPollRateInput.Size = new System.Drawing.Size(75, 20);
            this.ArmA3ServerPollRateInput.TabIndex = 8;
            this.CloseUITip.SetToolTip(this.ArmA3ServerPollRateInput, "How often, in seconds, the application checks the server");
            // 
            // ArmA3PlayedThresholdInput
            // 
            this.ArmA3PlayedThresholdInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3PlayedThresholdInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3PlayedThresholdInput.Location = new System.Drawing.Point(301, 245);
            this.ArmA3PlayedThresholdInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.ArmA3PlayedThresholdInput.Name = "ArmA3PlayedThresholdInput";
            this.ArmA3PlayedThresholdInput.Size = new System.Drawing.Size(75, 20);
            this.ArmA3PlayedThresholdInput.TabIndex = 10;
            this.CloseUITip.SetToolTip(this.ArmA3PlayedThresholdInput, "Time in seconds a players has to play until they\'re considered to have played");
            // 
            // ArmA3MissionThresholdInput
            // 
            this.ArmA3MissionThresholdInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3MissionThresholdInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3MissionThresholdInput.Location = new System.Drawing.Point(301, 275);
            this.ArmA3MissionThresholdInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.ArmA3MissionThresholdInput.Name = "ArmA3MissionThresholdInput";
            this.ArmA3MissionThresholdInput.Size = new System.Drawing.Size(75, 20);
            this.ArmA3MissionThresholdInput.TabIndex = 11;
            this.CloseUITip.SetToolTip(this.ArmA3MissionThresholdInput, "Number of missions in a session");
            // 
            // ArmA3RunTimeThresholdInput
            // 
            this.ArmA3RunTimeThresholdInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ArmA3RunTimeThresholdInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ArmA3RunTimeThresholdInput.Location = new System.Drawing.Point(301, 305);
            this.ArmA3RunTimeThresholdInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.ArmA3RunTimeThresholdInput.Name = "ArmA3RunTimeThresholdInput";
            this.ArmA3RunTimeThresholdInput.Size = new System.Drawing.Size(75, 20);
            this.ArmA3RunTimeThresholdInput.TabIndex = 12;
            this.CloseUITip.SetToolTip(this.ArmA3RunTimeThresholdInput, "Max window, in seconds, on session run time");
            // 
            // serverReconnectLimitInput
            // 
            this.serverReconnectLimitInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.serverReconnectLimitInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverReconnectLimitInput.Location = new System.Drawing.Point(301, 335);
            this.serverReconnectLimitInput.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.serverReconnectLimitInput.Name = "serverReconnectLimitInput";
            this.serverReconnectLimitInput.Size = new System.Drawing.Size(75, 20);
            this.serverReconnectLimitInput.TabIndex = 13;
            this.CloseUITip.SetToolTip(this.serverReconnectLimitInput, "How many seconds to try and reconnect to a crashed server");
            // 
            // Save
            // 
            this.Save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Save.Location = new System.Drawing.Point(443, 405);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 15;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // Cancel
            // 
            this.Cancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Cancel.Location = new System.Drawing.Point(524, 405);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 14;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReportStatus,
            this.SchedulerRunning});
            this.statusStrip1.Location = new System.Drawing.Point(0, 469);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(634, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // ReportStatus
            // 
            this.ReportStatus.Name = "ReportStatus";
            this.ReportStatus.Size = new System.Drawing.Size(94, 17);
            this.ReportStatus.Text = "Reporting Status";
            // 
            // SchedulerRunning
            // 
            this.SchedulerRunning.Name = "SchedulerRunning";
            this.SchedulerRunning.Size = new System.Drawing.Size(94, 17);
            this.SchedulerRunning.Text = "Scheduler Status";
            // 
            // StatusTimer
            // 
            this.StatusTimer.Enabled = true;
            this.StatusTimer.Interval = 500;
            this.StatusTimer.Tick += new System.EventHandler(this.StatusTimer_Tick);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(634, 491);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.TabbedWindow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWindow";
            this.Text = "Bourbon Warfare ArmA 3 Server Logger";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainWindow_FormClosed);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.TabbedWindow.ResumeLayout(false);
            this.Run.ResumeLayout(false);
            this.Schedule.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScheduleGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.scheduleBindingSource)).EndInit();
            this.Settings.ResumeLayout(false);
            this.SettingsTable.ResumeLayout(false);
            this.SettingsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3ServerPortInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MySQLServerPortInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3ServerPollRateInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3PlayedThresholdInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3MissionThresholdInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ArmA3RunTimeThresholdInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.serverReconnectLimitInput)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl TabbedWindow;
        private System.Windows.Forms.TabPage Run;
        private System.Windows.Forms.TabPage Schedule;
        private System.Windows.Forms.TabPage Settings;
        private System.Windows.Forms.DataGridView ScheduleGrid;
        private System.Windows.Forms.BindingSource scheduleBindingSource;
        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TableLayoutPanel SettingsTable;
        private System.Windows.Forms.Label ArmA3ServerAddressLabel;
        private System.Windows.Forms.Label ArmA3ServerPort;
        private System.Windows.Forms.TextBox ArmA3ServerAddressInput;
        private System.Windows.Forms.NumericUpDown ArmA3ServerPortInput;
        private System.Windows.Forms.Label MySQLServerAddressLabel;
        private System.Windows.Forms.Label MySQLServerPortLabel;
        private System.Windows.Forms.Label MySQLServerDatabaseLabel;
        private System.Windows.Forms.Label MySQLServerUsernameLabel;
        private System.Windows.Forms.Label MySQLServerPasswordLabel;
        private System.Windows.Forms.Label ArmA3ServerPollRateLabel;
        private System.Windows.Forms.Label ArmA3PlayedThresholdLabel;
        private System.Windows.Forms.Label ArmA3MissionThresholdLabel;
        private System.Windows.Forms.Label ArmA3RunTimeThresholdLabel;
        private System.Windows.Forms.TextBox MySQLServerAddressInput;
        private System.Windows.Forms.NumericUpDown MySQLServerPortInput;
        private System.Windows.Forms.TextBox MySQLServerDatabaseInput;
        private System.Windows.Forms.TextBox MySQLServerUsernameInput;
        private System.Windows.Forms.TextBox MySQLServerPasswordInput;
        private System.Windows.Forms.NumericUpDown ArmA3ServerPollRateInput;
        private System.Windows.Forms.NumericUpDown ArmA3PlayedThresholdInput;
        private System.Windows.Forms.NumericUpDown ArmA3MissionThresholdInput;
        private System.Windows.Forms.NumericUpDown ArmA3RunTimeThresholdInput;
        private System.Windows.Forms.Label ServerReconnectLimitLabel;
        private System.Windows.Forms.NumericUpDown serverReconnectLimitInput;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button ScheduleInput;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn dayOfTheWeekDataGridViewComboBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeOfDayDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn updatedDataGridViewCheckBoxColumn;
        private System.Windows.Forms.Button CloseInput;
        private System.Windows.Forms.ToolTip CloseUITip;
        private System.Windows.Forms.Button ReportingInput;
        private System.Windows.Forms.ToolStripStatusLabel ReportStatus;
        private System.Windows.Forms.Timer StatusTimer;
        private System.Windows.Forms.ToolStripStatusLabel SchedulerRunning;
    }
}

