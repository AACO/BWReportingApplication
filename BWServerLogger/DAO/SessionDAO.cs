using MySql.Data;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO
{
    public class SessionDAO
    {
        private MySqlConnection _connection;
        private MySqlCommand _addSession;
        private MySqlCommand _updateSession;

        public SessionDAO(MySqlConnection connection)
        {
            _connection = connection;
            SetupPreparedStatements();
        }

        public Session CreateSession(Session session)
        {
            session.Date = DateTime.Now;
            StringBuilder insert = new StringBuilder();
            insert.Append("insert into session (date, max_players, version, host_name, min_ping, max_ping) values");
            insert.Append("(\"");
            insert.Append(session.Date.ToString(DatabaseUtil.DATE_TIME_FORMAT));
            insert.Append("\", ");
            insert.Append(session.MaxPlayers);
            insert.Append(", \"");
            insert.Append(session.Version);
            insert.Append("\", \"");
            insert.Append(session.HostName);
            insert.Append("\", ");
            insert.Append(session.MinPing);
            insert.Append(", ");
            insert.Append(session.MaxPing);
            insert.Append(")");

            _addSession.Parameters[DatabaseUtil.DATE_KEY].Value = session.Date;
            _addSession.Parameters[DatabaseUtil.MAX_PLAYERS_KEY].Value = session.MaxPlayers;
            _addSession.Parameters[DatabaseUtil.VERSION_KEY].Value = session.Version;
            _addSession.Parameters[DatabaseUtil.HOST_NAME_KEY].Value = session.HostName;
            _addSession.Parameters[DatabaseUtil.MIN_PING_KEY].Value = session.MinPing;
            _addSession.Parameters[DatabaseUtil.MAX_PING_KEY].Value = session.MaxPing;
            _addSession.ExecuteNonQuery();

            session.Id = DatabaseUtil.GetLastInsertedId(ref _connection);

            return session;
        }

        public void UpdateSession(Session session)
        {
            _updateSession.Parameters[DatabaseUtil.MAX_PLAYERS_KEY].Value = session.MaxPlayers;
            _updateSession.Parameters[DatabaseUtil.MAX_PING_KEY].Value = session.MaxPing;
            _updateSession.Parameters[DatabaseUtil.MIN_PING_KEY].Value = session.MinPing;
            _updateSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
            _updateSession.ExecuteNonQuery();
        }

        private void SetupPreparedStatements()
        {
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

            _addSession = new MySqlCommand(addSessionInsert.ToString(), _connection);
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

            _updateSession = new MySqlCommand(sessionUpdate.ToString(), _connection);
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PLAYERS_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MAX_PING_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MIN_PING_KEY, MySqlDbType.Int32));
            _updateSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _updateSession.Prepare();

        }

    }
}
