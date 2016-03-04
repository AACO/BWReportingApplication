using MySql.Data.MySqlClient;

using System;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Session database access object to deal with <see cref="Session"/> objects. Extends <see cref="BaseDAO"/>.
    /// </summary>
    /// <seealso cref="BaseDAO"/>
    public class SessionDAO : BaseDAO {
        private MySqlCommand _addSession;
        private MySqlCommand _updateSession;

        /// <summary>
        /// Constructor, sets up prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/>, used to create prepared statements</param>
        /// <seealso cref="BaseDAO(MySqlConnection)"/>
        public SessionDAO(MySqlConnection connection) : base(connection) {
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_addSession != null) {
                    _addSession.Dispose();
                }
                if (_updateSession != null) {
                    _updateSession.Dispose();
                }
            }
        }

        /// <summary>
        /// Inserts a <see cref="Session"/> object into the database
        /// </summary>
        /// <param name="session"><see cref="Session"/> to create in the database</param>
        /// <returns>The <see cref="Session"/> with updated values from the database</returns>
        public Session CreateSession(Session session) {
            session.Date = DateTime.Now;

            _addSession.Parameters[DatabaseUtil.DATE_KEY].Value = session.Date;
            _addSession.Parameters[DatabaseUtil.MAX_PLAYERS_KEY].Value = session.MaxPlayers;
            _addSession.Parameters[DatabaseUtil.VERSION_KEY].Value = session.Version;
            _addSession.Parameters[DatabaseUtil.HOST_NAME_KEY].Value = session.HostName;
            _addSession.Parameters[DatabaseUtil.MIN_PING_KEY].Value = session.MinPing;
            _addSession.Parameters[DatabaseUtil.MAX_PING_KEY].Value = session.MaxPing;
            _addSession.ExecuteNonQuery();

            session.Id = GetLastInsertedId();
            _logger.DebugFormat("Created session in the database with id: {0}", session.Id);

            return session;
        }

        /// <summary>
        /// Updates the <see cref="Session"/> in the database
        /// </summary>
        /// <param name="session">The <see cref="Session"/> to update</param>
        public void UpdateSession(Session session) {
            _updateSession.Parameters[DatabaseUtil.MAX_PLAYERS_KEY].Value = session.MaxPlayers;
            _updateSession.Parameters[DatabaseUtil.MAX_PING_KEY].Value = session.MaxPing;
            _updateSession.Parameters[DatabaseUtil.MIN_PING_KEY].Value = session.MinPing;
            _updateSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
            _updateSession.ExecuteNonQuery();
            _logger.DebugFormat("Updated session in the database with id: {0}", session.Id);
        }

        /// <summary>
        /// Method to setup prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/> used to prepare statements</param>
        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder addSessionInsert = new StringBuilder();
            addSessionInsert.Append("insert into session (date, max_players, version, host_name, min_ping, max_ping) values");
            addSessionInsert.Append("(");
            addSessionInsert.Append(DatabaseUtil.DATE_KEY);
            addSessionInsert.Append(", ");
            addSessionInsert.Append(DatabaseUtil.MAX_PLAYERS_KEY);
            addSessionInsert.Append(", ");
            addSessionInsert.Append(DatabaseUtil.VERSION_KEY);
            addSessionInsert.Append(", ");
            addSessionInsert.Append(DatabaseUtil.HOST_NAME_KEY);
            addSessionInsert.Append(", ");
            addSessionInsert.Append(DatabaseUtil.MIN_PING_KEY);
            addSessionInsert.Append(", ");
            addSessionInsert.Append(DatabaseUtil.MAX_PING_KEY);
            addSessionInsert.Append(")");

            _addSession = new MySqlCommand(addSessionInsert.ToString(), connection);
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.DATE_KEY, MySqlDbType.DateTime));
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.VERSION_KEY, MySqlDbType.String));
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.HOST_NAME_KEY, MySqlDbType.String));
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PLAYERS_KEY, MySqlDbType.Int32));
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PING_KEY, MySqlDbType.Int32));
            _addSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MIN_PING_KEY, MySqlDbType.Int32));
            _addSession.Prepare();

            StringBuilder sessionUpdate = new StringBuilder();
            sessionUpdate.Append("update session ");
            sessionUpdate.Append("set max_players = ");
            sessionUpdate.Append(DatabaseUtil.MAX_PLAYERS_KEY);
            sessionUpdate.Append(", ");
            sessionUpdate.Append("max_ping = ");
            sessionUpdate.Append(DatabaseUtil.MAX_PING_KEY);
            sessionUpdate.Append(", ");
            sessionUpdate.Append("min_ping = ");
            sessionUpdate.Append(DatabaseUtil.MIN_PING_KEY);
            sessionUpdate.Append(" ");
            sessionUpdate.Append("where id = ");
            sessionUpdate.Append(DatabaseUtil.SESSION_ID_KEY);

            _updateSession = new MySqlCommand(sessionUpdate.ToString(), connection);
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PLAYERS_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PING_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MIN_PING_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _updateSession.Prepare();
        }
    }
}
