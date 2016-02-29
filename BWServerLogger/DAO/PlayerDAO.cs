using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    public class PlayerDAO : BaseDAO {
        private Dictionary<string, PlayerSession> _cachedPlayerSessions;
        private MySqlCommand _getPlayerSession;
        private MySqlCommand _addPlayerSession;
        private MySqlCommand _addPlayer;
        private MySqlCommand _updatePlayerSession;
        private MySqlCommand _updatePlayer;

        public PlayerDAO(MySqlConnection connection) : base(connection) {
            _cachedPlayerSessions = new Dictionary<string, PlayerSession>();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_getPlayerSession != null) {
                    _getPlayerSession.Dispose();
                }
                if (_addPlayerSession != null) {
                    _addPlayerSession.Dispose();
                }
                if (_addPlayer != null) {
                    _addPlayer.Dispose();
                }
                if (_updatePlayerSession != null) {
                    _updatePlayerSession.Dispose();
                }
                if (_updatePlayer != null) {
                    _updatePlayer.Dispose();
                }
            }
        }

        public ISet<PlayerSession> GetOrCreatePlayerSessions(IList<Player> players, Session session) {
            ISet<PlayerSession> playerSessions = new HashSet<PlayerSession>();

            foreach (Player player in players) {
                PlayerSession playerSession = new PlayerSession();
                playerSession.Player = player;
                playerSession.Session = session;

                if (_cachedPlayerSessions.ContainsKey(player.Name)) {
                    PlayerSession cachedPlayerSesion = new PlayerSession();
                    _cachedPlayerSessions.TryGetValue(player.Name, out cachedPlayerSesion);
                    playerSessions.Add(cachedPlayerSesion);
                } else {
                    _getPlayerSession.Parameters[DatabaseUtil.NAME_KEY].Value = player.Name;
                    _getPlayerSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;

                    MySqlDataReader getPlayerResult = _getPlayerSession.ExecuteReader();

                    if (getPlayerResult.HasRows) {
                        getPlayerResult.Read();
                        playerSession.Player.Id = getPlayerResult.GetInt32(0);

                        if (getPlayerResult.GetBoolean(1) != playerSession.Player.HasClanTag) {
                            playerSession.Player.Updated = true;
                        }

                        if (getPlayerResult.IsDBNull(2)) {
                            getPlayerResult.Close();

                            _addPlayerSession.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerSession.Player.Id;
                            _addPlayerSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
                            _addPlayerSession.ExecuteNonQuery();

                            playerSession.Id = GetLastInsertedId();
                        } else {
                            playerSession.Id = getPlayerResult.GetInt32(2);
                            playerSession.Length = getPlayerResult.GetInt32(3);
                            playerSession.Played = getPlayerResult.GetBoolean(4);
                            getPlayerResult.Close();
                        }
                    } else {
                        getPlayerResult.Close();
                        _addPlayer.Parameters[DatabaseUtil.NAME_KEY].Value = player.Name;
                        _addPlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = player.HasClanTag;
                        _addPlayer.ExecuteNonQuery();

                        playerSession.Player.Id = GetLastInsertedId();

                        _addPlayerSession.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = player.Id;
                        _addPlayerSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
                        _addPlayerSession.ExecuteNonQuery();

                        playerSession.Id = GetLastInsertedId();
                    }
                    _cachedPlayerSessions.Add(player.Name, playerSession);
                    playerSessions.Add(playerSession);
                }
            }

            return playerSessions;
        }

        public void UpdatePlayerSessions(ISet<PlayerSession> playerSessions) {
            foreach (PlayerSession playerSession in playerSessions) {
                UpdatePlayerSession(playerSession);
            }
        }

        public void UpdatePlayerSession(PlayerSession playerSession) {
            if (playerSession.Updated) {
                _updatePlayerSession.Parameters[DatabaseUtil.PLAYED_KEY].Value = playerSession.Played;
                _updatePlayerSession.Parameters[DatabaseUtil.LENGTH_KEY].Value = playerSession.Length;
                _updatePlayerSession.Parameters[DatabaseUtil.PLAYER_TO_SESSION_ID_KEY].Value = playerSession.Id;
                _updatePlayerSession.ExecuteNonQuery();
            }

            if (playerSession.Player.Updated) {
                _updatePlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = playerSession.Player.HasClanTag;
                _updatePlayer.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerSession.Player.Id;
                _updatePlayer.ExecuteNonQuery();
            }
        }

        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder getPlayerSessionSelect = new StringBuilder();
            getPlayerSessionSelect.Append("select p.id, p.has_clan_tag, ");
            getPlayerSessionSelect.Append("pts.id, pts.length, pts.played ");
            getPlayerSessionSelect.Append("from player p ");
            getPlayerSessionSelect.Append("left join player_to_session pts on pts.player_id = p.id and pts.session_id = ");
            getPlayerSessionSelect.Append(DatabaseUtil.SESSION_ID_KEY);
            getPlayerSessionSelect.Append(" ");
            getPlayerSessionSelect.Append("where p.name = ");
            getPlayerSessionSelect.Append(DatabaseUtil.NAME_KEY);

            _getPlayerSession = new MySqlCommand(getPlayerSessionSelect.ToString(), connection);
            _getPlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _getPlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _getPlayerSession.Prepare();

            StringBuilder addPlayerInsert = new StringBuilder();
            addPlayerInsert.Append("insert into player (name, has_clan_tag)");
            addPlayerInsert.Append("values (");
            addPlayerInsert.Append(DatabaseUtil.NAME_KEY);
            addPlayerInsert.Append(", ");
            addPlayerInsert.Append(DatabaseUtil.HAS_CLAN_TAG_KEY);
            addPlayerInsert.Append(")");

            _addPlayer = new MySqlCommand(addPlayerInsert.ToString(), connection);
            _addPlayer.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _addPlayer.Parameters.Add(new MySqlParameter(DatabaseUtil.HAS_CLAN_TAG_KEY, MySqlDbType.Bit));
            _addPlayer.Prepare();

            StringBuilder addPlayerSessionInsert = new StringBuilder();
            addPlayerSessionInsert.Append("insert into player_to_session (player_id, session_id) ");
            addPlayerSessionInsert.Append("values (");
            addPlayerSessionInsert.Append(DatabaseUtil.PLAYER_ID_KEY);
            addPlayerSessionInsert.Append(", ");
            addPlayerSessionInsert.Append(DatabaseUtil.SESSION_ID_KEY);
            addPlayerSessionInsert.Append(")");

            _addPlayerSession = new MySqlCommand(addPlayerSessionInsert.ToString(), connection);
            _addPlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_ID_KEY, MySqlDbType.Int32));
            _addPlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _addPlayerSession.Prepare();

            StringBuilder playerSessionUpdate = new StringBuilder();
            playerSessionUpdate.Append("update player_to_session ");
            playerSessionUpdate.Append("set length = ");
            playerSessionUpdate.Append(DatabaseUtil.LENGTH_KEY);
            playerSessionUpdate.Append(", played = ");
            playerSessionUpdate.Append(DatabaseUtil.PLAYED_KEY);
            playerSessionUpdate.Append(" ");
            playerSessionUpdate.Append("where id = ");
            playerSessionUpdate.Append(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY);

            _updatePlayerSession = new MySqlCommand(playerSessionUpdate.ToString(), connection);
            _updatePlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.LENGTH_KEY, MySqlDbType.Int32));
            _updatePlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYED_KEY, MySqlDbType.Bit));
            _updatePlayerSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _updatePlayerSession.Prepare();

            StringBuilder playerUpdate = new StringBuilder();
            playerUpdate.Append("update player ");
            playerUpdate.Append("set has_clan_tag = ");
            playerUpdate.Append(DatabaseUtil.HAS_CLAN_TAG_KEY);
            playerUpdate.Append(" ");
            playerUpdate.Append("where id = ");
            playerUpdate.Append(DatabaseUtil.PLAYER_ID_KEY);

            _updatePlayer = new MySqlCommand(playerUpdate.ToString(), connection);
            _updatePlayer.Parameters.Add(new MySqlParameter(DatabaseUtil.HAS_CLAN_TAG_KEY, MySqlDbType.Bit));
            _updatePlayer.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_ID_KEY, MySqlDbType.Int32));
            _updatePlayer.Prepare();
        }
    }
}
