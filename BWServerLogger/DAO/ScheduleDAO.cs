using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Schedule database access object to deal with <see cref="Schedule"/> objects. Extends <see cref="BaseDAO"/>.
    /// </summary>
    /// <seealso cref="BaseDAO"/>
    public class ScheduleDAO : BaseDAO {
        private MySqlCommand _addScheduleItem;
        private MySqlCommand _getScheduleItems;
        private MySqlCommand _removeScheduleItem;
        private MySqlCommand _updateScheduleItem;

        /// <summary>
        /// Constructor, sets up prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/>, used to create prepared statements</param>
        /// <seealso cref="BaseDAO(MySqlConnection)"/>
        public ScheduleDAO(MySqlConnection connection) : base(connection) {
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_addScheduleItem != null) {
                    _addScheduleItem.Dispose();
                }
                if (_getScheduleItems != null) {
                    _getScheduleItems.Dispose();
                }
                if (_removeScheduleItem != null) {
                    _removeScheduleItem.Dispose();
                }
                if (_updateScheduleItem != null) {
                    _updateScheduleItem.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets a set of <see cref="Schedule"/> objects from the database
        /// </summary>
        /// <returns>set of <see cref="Schedule"/> objects from the database</returns>
        public ISet<Schedule> GetScheduleItems() {
            ISet<Schedule> scheduleItems = new HashSet<Schedule>();

            MySqlDataReader getScheduleItemsResult = _getScheduleItems.ExecuteReader();

            if (getScheduleItemsResult.HasRows) {
                while (getScheduleItemsResult.Read()) {
                    scheduleItems.Add(new Schedule(getScheduleItemsResult.GetInt32(0),
                                                   getScheduleItemsResult.GetInt32(1),
                                                   getScheduleItemsResult.GetString(2)));
                }
            }

            getScheduleItemsResult.Close();

            _logger.Debug("Schedule items retrieved from the database");

            return scheduleItems;
        }

        /// <summary>
        /// Saves/adds a <see cref="Schedule"/> item to the database
        /// </summary>
        /// <param name="scheduleItem">The <see cref="Schedule"/> object to save/add</param>
        public void SaveScheduleItem(Schedule scheduleItem) {
            if (scheduleItem.Id < 1) {
                _addScheduleItem.Parameters[DatabaseUtil.DAY_OF_THE_WEEK_KEY].Value = scheduleItem.DayOfTheWeek + 1;
                _addScheduleItem.Parameters[DatabaseUtil.TIME_OF_DAY_KEY].Value = scheduleItem.TimeOfDay.ToString();
                _addScheduleItem.ExecuteNonQuery();
                _logger.DebugFormat("Schedule item inserted into the database with day of the week: {0}, and time of day: {1}", scheduleItem.DayOfTheWeek, scheduleItem.TimeOfDay);
            } else {
                _updateScheduleItem.Parameters[DatabaseUtil.DAY_OF_THE_WEEK_KEY].Value = scheduleItem.DayOfTheWeek + 1;
                _updateScheduleItem.Parameters[DatabaseUtil.TIME_OF_DAY_KEY].Value = scheduleItem.TimeOfDay.ToString();
                _updateScheduleItem.Parameters[DatabaseUtil.SCHEDULE_ID_KEY].Value = scheduleItem.Id;
                _updateScheduleItem.ExecuteNonQuery();
                _logger.DebugFormat("Schedule item updated in the database with id: {0}", scheduleItem.Id);
            }
        }

        /// <summary>
        /// Removes the given <see cref="Schedule"/> item
        /// </summary>
        /// <param name="scheduleItem">The <see cref="Schedule"/> object to remove</param>
        public void RemoveScheduleItem(Schedule scheduleItem) {
            _removeScheduleItem.Parameters[DatabaseUtil.SCHEDULE_ID_KEY].Value = scheduleItem.Id;
            _removeScheduleItem.ExecuteNonQuery();
            _logger.DebugFormat("Schedule item removed from the database with id: {0}", scheduleItem.Id);
        }

        /// <summary>
        /// Method to setup prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/> used to prepare statements</param>
        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder getScheduleItemsSelect = new StringBuilder();
            getScheduleItemsSelect.Append("select id, day_of_the_week-1, time_of_day ");
            getScheduleItemsSelect.Append("from schedule ");

            _getScheduleItems = new MySqlCommand(getScheduleItemsSelect.ToString(), connection);
            _getScheduleItems.Prepare();

            StringBuilder addScheduleItemInsert = new StringBuilder();
            addScheduleItemInsert.Append("insert into schedule (day_of_the_week, time_of_day)");
            addScheduleItemInsert.Append("values (");
            addScheduleItemInsert.Append(DatabaseUtil.DAY_OF_THE_WEEK_KEY);
            addScheduleItemInsert.Append(", ");
            addScheduleItemInsert.Append(DatabaseUtil.TIME_OF_DAY_KEY);
            addScheduleItemInsert.Append(")");

            _addScheduleItem = new MySqlCommand(addScheduleItemInsert.ToString(), connection);
            _addScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.DAY_OF_THE_WEEK_KEY, MySqlDbType.Int32));
            _addScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.TIME_OF_DAY_KEY, MySqlDbType.String));
            _addScheduleItem.Prepare();

            StringBuilder scheduleItemUpdate = new StringBuilder();
            scheduleItemUpdate.Append("update schedule ");
            scheduleItemUpdate.Append("set day_of_the_week = ");
            scheduleItemUpdate.Append(DatabaseUtil.DAY_OF_THE_WEEK_KEY);
            scheduleItemUpdate.Append(", ");
            scheduleItemUpdate.Append("time_of_day = ");
            scheduleItemUpdate.Append(DatabaseUtil.TIME_OF_DAY_KEY);
            scheduleItemUpdate.Append(" ");
            scheduleItemUpdate.Append("where id = ");
            scheduleItemUpdate.Append(DatabaseUtil.SCHEDULE_ID_KEY);

            _updateScheduleItem = new MySqlCommand(scheduleItemUpdate.ToString(), connection);
            _updateScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.DAY_OF_THE_WEEK_KEY, MySqlDbType.Int32));
            _updateScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.TIME_OF_DAY_KEY, MySqlDbType.String));
            _updateScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.SCHEDULE_ID_KEY, MySqlDbType.Int32));
            _updateScheduleItem.Prepare();

            StringBuilder scheduleItemDelete = new StringBuilder();
            scheduleItemDelete.Append("delete from schedule ");
            scheduleItemDelete.Append("where id = ");
            scheduleItemDelete.Append(DatabaseUtil.SCHEDULE_ID_KEY);

            _removeScheduleItem = new MySqlCommand(scheduleItemDelete.ToString(), connection);
            _removeScheduleItem.Parameters.Add(new MySqlParameter(DatabaseUtil.SCHEDULE_ID_KEY, MySqlDbType.Int32));
            _removeScheduleItem.Prepare();
        }
    }
}
