using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    public class SessionDAO : BaseDAO {
        private MySqlCommand _addSession;
        private MySqlCommand _updateSession;

        public SessionDAO(MySqlConnection connection) : base(connection) {
        }

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

        public Session CreateSession(Session session) {
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

            session.Id = GetLastInsertedId();

            return session;
        }

        public void UpdateSession(Session session) {
            _updateSession.Parameters[DatabaseUtil.MAX_PLAYERS_KEY].Value = session.MaxPlayers;
            _updateSession.Parameters[DatabaseUtil.MAX_PING_KEY].Value = session.MaxPing;
            _updateSession.Parameters[DatabaseUtil.MIN_PING_KEY].Value = session.MinPing;
            _updateSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
            _updateSession.ExecuteNonQuery();
        }

        protected override IDictionary<string, ISet<Column>> GetRequiredSchema() {
            Dictionary<string, ISet<Column>> returnMap = new Dictionary<string, ISet<Column>>();

            // define session columns
            HashSet<Column> columns = new HashSet<Column>();
            columns.Add(new Column("id",
                                   "int(10) unsigned",
                                   "NO",
                                   "PRI",
                                   null,
                                   "auto_increment"));

            columns.Add(new Column("date",
                                   "datetime",
                                   "NO",
                                   "UNI",
                                   null,
                                   ""));

            columns.Add(new Column("host_name",
                                   "varchar(255)",
                                   "NO",
                                   "",
                                   null,
                                   ""));

            columns.Add(new Column("max_players",
                                   "int(10) unsigned",
                                   "NO",
                                   "",
                                   null,
                                   ""));

            columns.Add(new Column("version",
                                   "varchar(50)",
                                   "NO",
                                   "",
                                   null,
                                   ""));

            columns.Add(new Column("min_ping",
                                   "int(10) unsigned",
                                   "NO",
                                   "",
                                   null,
                                   ""));

            columns.Add(new Column("max_ping",
                                   "int(10) unsigned",
                                   "NO",
                                   "",
                                   null,
                                   ""));
            returnMap.Add("session", columns);

            return returnMap;
        }

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
