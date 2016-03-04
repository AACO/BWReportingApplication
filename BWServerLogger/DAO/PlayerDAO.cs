using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Player database access object to deal with <see cref="Player"/> and <see cref="PlayerSession"/> objects. Extends <see cref="BaseDAO"/>.
    /// </summary>
    /// <seealso cref="BaseDAO"/>
    public class PlayerDAO : BaseDAO {
        private Dictionary<string, PlayerSession> _cachedPlayerSessions;
        private MySqlCommand _getPlayerSession;
        private MySqlCommand _addPlayerSession;
        private MySqlCommand _addPlayer;
        private MySqlCommand _updatePlayerSession;
        private MySqlCommand _updatePlayer;

        /// <summary>
        /// Constructor, sets up prepared statements
        /// </summary>
        /// <param name="connection">Open<see cref="MySqlConnection"/>, used to create prepared statements</param>
        /// <seealso cref="BaseDAO(MySqlConnection)"/>
        public PlayerDAO(MySqlConnection connection) : base(connection) {
            _cachedPlayerSessions = new Dictionary<string, PlayerSession>();
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
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

        /// <summary>
        /// Gets or creates a set of <see cref="PlayerSession"/>s from the given list of <see cref="Player"/>s and the current <see cref="Session"/>.
        /// Using caching to reduce MySQL calls and speed up runtime
        /// </summary>
        /// <param name="players">List of <see cref="Player"/>s to get/create</param>
        /// <param name="session">Current <see cref="Session"/></param>
        /// <returns>A set of <see cref="PlayerSession"/> objects, from the database.</returns>
        public ISet<PlayerSession> GetOrCreatePlayerSessions(IList<Player> players, Session session) {
            ISet<PlayerSession> playerSessions = new HashSet<PlayerSession>();

            foreach (Player player in players) {
                PlayerSession playerSession = new PlayerSession();
                playerSession.Player = player;
                playerSession.Session = session;

                if (_cachedPlayerSessions.ContainsKey(player.Name)) {
                    _cachedPlayerSessions.TryGetValue(player.Name, out playerSession);
                    _logger.DebugFormat("Retrieved player session from the cache with ID: {0}", playerSession.Id);
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
                            CreatePlayerSession(playerSession);
                        } else {
                            playerSession.Id = getPlayerResult.GetInt32(2);
                            playerSession.Length = getPlayerResult.GetInt32(3);
                            playerSession.Played = getPlayerResult.GetBoolean(4);
                            _logger.DebugFormat("Retrieved player session from the database with ID: {0}", playerSession.Id);

                            getPlayerResult.Close();
                        }
                    } else {
                        getPlayerResult.Close();
                        _addPlayer.Parameters[DatabaseUtil.NAME_KEY].Value = player.Name;
                        _addPlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = player.HasClanTag;
                        _addPlayer.ExecuteNonQuery();

                        playerSession.Player.Id = GetLastInsertedId();

                        CreatePlayerSession(playerSession);                   
                    }
                    _cachedPlayerSessions.Add(player.Name, playerSession);
                }
                playerSessions.Add(playerSession);
            }

            return playerSessions;
        }

        /// <summary>
        /// Function to update a set of <see cref="PlayerSession"/>s on the database level, just calls <see cref="UpdatePlayerSession(PlayerSession)"/>
        /// </summary>
        /// <param name="playerSessions">Set of <see cref="PlayerSession"/>s to update</param>
        public void UpdatePlayerSessions(ISet<PlayerSession> playerSessions) {
            foreach (PlayerSession playerSession in playerSessions) {
                UpdatePlayerSession(playerSession);
            }
        }

        /// <summary>
        /// Function to update a <see cref="PlayerSession"/> on the database level
        /// </summary>
        /// <param name="playerSession"><see cref="PlayerSession"/> to update</param>
        public void UpdatePlayerSession(PlayerSession playerSession) {
            if (playerSession.Updated) {
                _updatePlayerSession.Parameters[DatabaseUtil.PLAYED_KEY].Value = playerSession.Played;
                _updatePlayerSession.Parameters[DatabaseUtil.LENGTH_KEY].Value = playerSession.Length;
                _updatePlayerSession.Parameters[DatabaseUtil.PLAYER_TO_SESSION_ID_KEY].Value = playerSession.Id;
                _updatePlayerSession.ExecuteNonQuery();
                _logger.DebugFormat("Player session updated with ID: {0}", playerSession.Id);
            }

            if (playerSession.Player.Updated) {
                _updatePlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = playerSession.Player.HasClanTag;
                _updatePlayer.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerSession.Player.Id;
                _updatePlayer.ExecuteNonQuery();
                _logger.DebugFormat("Player updated with ID: {0}", playerSession.Player.Id);
            }
        }

        /// <summary>
        /// Method to setup prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/> used to prepare statements</param>
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

        /// <summary>
        /// Helper method to create a <see cref="PlayerSession"/> in the database
        /// </summary>
        /// <param name="playerSession"><see cref="PlayerSession"/> to create</param>
        /// <returns><see cref="PlayerSession"/> with added database id</returns>
        private PlayerSession CreatePlayerSession(PlayerSession playerSession) {
            _addPlayerSession.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerSession.Player.Id;
            _addPlayerSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = playerSession.Session.Id;
            _addPlayerSession.ExecuteNonQuery();

            playerSession.Id = GetLastInsertedId();
            _logger.DebugFormat("Player session inserted into the database with ID: {0}", playerSession.Id);

            return playerSession;
        }
    }
}
