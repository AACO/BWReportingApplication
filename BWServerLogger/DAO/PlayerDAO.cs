using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Player database access object to deal with <see cref="Player"/> and <see cref="PlayerMissionSession"/> objects. Extends <see cref="BaseDAO"/>.
    /// </summary>
    /// <seealso cref="BaseDAO"/>
    public class PlayerDAO : BaseDAO {
        private Dictionary<string, PlayerMissionSession> _cachedPlayerMissionSessions;
        private MySqlCommand _getPlayerMissionSession;
        private MySqlCommand _addPlayerMissionSession;
        private MySqlCommand _addPlayer;
        private MySqlCommand _updatePlayerMissionSession;
        private MySqlCommand _updatePlayer;

        /// <summary>
        /// Constructor, sets up prepared statements
        /// </summary>
        /// <param name="connection">Open<see cref="MySqlConnection"/>, used to create prepared statements</param>
        /// <seealso cref="BaseDAO(MySqlConnection)"/>
        public PlayerDAO(MySqlConnection connection) : base(connection) {
            _cachedPlayerMissionSessions = new Dictionary<string, PlayerMissionSession>();
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_getPlayerMissionSession != null) {
                    _getPlayerMissionSession.Dispose();
                }
                if (_addPlayerMissionSession != null) {
                    _addPlayerMissionSession.Dispose();
                }
                if (_addPlayer != null) {
                    _addPlayer.Dispose();
                }
                if (_updatePlayerMissionSession != null) {
                    _updatePlayerMissionSession.Dispose();
                }
                if (_updatePlayer != null) {
                    _updatePlayer.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets or creates a set of <see cref="PlayerMissionSession"/>s from the given list of <see cref="Player"/>s and the current <see cref="Session"/>.
        /// Using caching to reduce MySQL calls and speed up runtime
        /// </summary>
        /// <param name="players">List of <see cref="Player"/>s to get/create</param>
        /// <param name="missionSession">Current <see cref="MissionSession"/></param>
        /// <returns>A set of <see cref="PlayerMissionSession"/> objects, from the database.</returns>
        public ISet<PlayerMissionSession> GetOrCreatePlayerMissionSessions(IList<Player> players, MissionSession missionSession) {
            ISet<PlayerMissionSession> playerMissionSessions = new HashSet<PlayerMissionSession>();

            foreach (Player player in players) {
                PlayerMissionSession playerToMissionSession = new PlayerMissionSession();
                playerToMissionSession.Player = player;
                playerToMissionSession.MissionSession = missionSession;

                string key = player.Name + missionSession.Id;
                if (_cachedPlayerMissionSessions.ContainsKey(key)) {
                    _cachedPlayerMissionSessions.TryGetValue(key, out playerToMissionSession);
                    _logger.DebugFormat("Retrieved player mission session from the cache with ID: {0}", playerToMissionSession.Id);
                } else {
                    _getPlayerMissionSession.Parameters[DatabaseUtil.NAME_KEY].Value = player.Name;
                    _getPlayerMissionSession.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = missionSession.Id;

                    MySqlDataReader getPlayerResult = _getPlayerMissionSession.ExecuteReader();

                    if (getPlayerResult.HasRows) {
                        getPlayerResult.Read();
                        playerToMissionSession.Player.Id = getPlayerResult.GetInt32(0);

                        if (getPlayerResult.GetBoolean(1) != playerToMissionSession.Player.HasClanTag) {
                            playerToMissionSession.Player.Updated = true;
                        }

                        if (getPlayerResult.IsDBNull(2)) {
                            getPlayerResult.Close();
                            CreatePlayerMissionSession(playerToMissionSession);
                        } else {
                            playerToMissionSession.Id = getPlayerResult.GetInt32(2);
                            playerToMissionSession.Length = getPlayerResult.GetInt32(3);
                            playerToMissionSession.Played = getPlayerResult.GetBoolean(4);
                            _logger.DebugFormat("Retrieved player mission session from the database with ID: {0}", playerToMissionSession.Id);

                            getPlayerResult.Close();
                        }
                    } else {
                        getPlayerResult.Close();
                        _addPlayer.Parameters[DatabaseUtil.NAME_KEY].Value = player.Name;
                        _addPlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = player.HasClanTag;
                        _addPlayer.ExecuteNonQuery();

                        playerToMissionSession.Player.Id = GetLastInsertedId();

                        CreatePlayerMissionSession(playerToMissionSession);                   
                    }
                    _cachedPlayerMissionSessions.Add(key, playerToMissionSession);
                }
                playerMissionSessions.Add(playerToMissionSession);
            }

            return playerMissionSessions;
        }

        /// <summary>
        /// Function to update a set of <see cref="PlayerMissionSession"/>s on the database level, just calls <see cref="UpdatePlayerMissionSession(PlayerMissionSession)"/>
        /// </summary>
        /// <param name="playerToMissionSessions">Set of <see cref="PlayerMissionSession"/>s to update</param>
        public void UpdatePlayerMissionSessions(ISet<PlayerMissionSession> playerToMissionSessions) {
            foreach (PlayerMissionSession playerToMissionSession in playerToMissionSessions) {
                UpdatePlayerMissionSession(playerToMissionSession);
            }
        }

        /// <summary>
        /// Function to update a <see cref="PlayerMissionSession"/> on the database level
        /// </summary>
        /// <param name="playerToMissionSession"><see cref="PlayerMissionSession"/> to update</param>
        public void UpdatePlayerMissionSession(PlayerMissionSession playerToMissionSession) {
            _updatePlayerMissionSession.Parameters[DatabaseUtil.PLAYED_KEY].Value = playerToMissionSession.Played;
            _updatePlayerMissionSession.Parameters[DatabaseUtil.LENGTH_KEY].Value = playerToMissionSession.Length;
            _updatePlayerMissionSession.Parameters[DatabaseUtil.PLAYER_TO_MISSION_TO_SESSION_ID_KEY].Value = playerToMissionSession.Id;
            _updatePlayerMissionSession.ExecuteNonQuery();
            _logger.DebugFormat("Player mission session updated with ID: {0}", playerToMissionSession.Id);

            if (playerToMissionSession.Player.Updated) {
                _updatePlayer.Parameters[DatabaseUtil.HAS_CLAN_TAG_KEY].Value = playerToMissionSession.Player.HasClanTag;
                _updatePlayer.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerToMissionSession.Player.Id;
                _updatePlayer.ExecuteNonQuery();
                _logger.DebugFormat("Player updated with ID: {0}", playerToMissionSession.Player.Id);
            }
        }

        /// <summary>
        /// Method to setup prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/> used to prepare statements</param>
        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder getPlayerMissionSessionSelect = new StringBuilder();
            getPlayerMissionSessionSelect.Append("select p.id, p.has_clan_tag, ");
            getPlayerMissionSessionSelect.Append("ptmts.id, ptmts.length, ptmts.played ");
            getPlayerMissionSessionSelect.Append("from player p ");
            getPlayerMissionSessionSelect.Append("left join player_to_mission_to_session ptmts on ptmts.player_id = p.id and ptmts.mission_to_session_id = ");
            getPlayerMissionSessionSelect.Append(DatabaseUtil.MISSION_TO_SESSION_ID_KEY);
            getPlayerMissionSessionSelect.Append(" ");
            getPlayerMissionSessionSelect.Append("where p.name = ");
            getPlayerMissionSessionSelect.Append(DatabaseUtil.NAME_KEY);

            _getPlayerMissionSession = new MySqlCommand(getPlayerMissionSessionSelect.ToString(), connection);
            _getPlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _getPlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _getPlayerMissionSession.Prepare();

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

            StringBuilder addPlayerMissionSessionInsert = new StringBuilder();
            addPlayerMissionSessionInsert.Append("insert into player_to_mission_to_session (player_id, mission_to_session_id) ");
            addPlayerMissionSessionInsert.Append("values (");
            addPlayerMissionSessionInsert.Append(DatabaseUtil.PLAYER_ID_KEY);
            addPlayerMissionSessionInsert.Append(", ");
            addPlayerMissionSessionInsert.Append(DatabaseUtil.MISSION_TO_SESSION_ID_KEY);
            addPlayerMissionSessionInsert.Append(")");

            _addPlayerMissionSession = new MySqlCommand(addPlayerMissionSessionInsert.ToString(), connection);
            _addPlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_ID_KEY, MySqlDbType.Int32));
            _addPlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _addPlayerMissionSession.Prepare();

            StringBuilder playerMissionSessionUpdate = new StringBuilder();
            playerMissionSessionUpdate.Append("update player_to_mission_to_session ");
            playerMissionSessionUpdate.Append("set length = ");
            playerMissionSessionUpdate.Append(DatabaseUtil.LENGTH_KEY);
            playerMissionSessionUpdate.Append(", played = ");
            playerMissionSessionUpdate.Append(DatabaseUtil.PLAYED_KEY);
            playerMissionSessionUpdate.Append(" ");
            playerMissionSessionUpdate.Append("where id = ");
            playerMissionSessionUpdate.Append(DatabaseUtil.PLAYER_TO_MISSION_TO_SESSION_ID_KEY);

            _updatePlayerMissionSession = new MySqlCommand(playerMissionSessionUpdate.ToString(), connection);
            _updatePlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.LENGTH_KEY, MySqlDbType.Int32));
            _updatePlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYED_KEY, MySqlDbType.Bit));
            _updatePlayerMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_TO_MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _updatePlayerMissionSession.Prepare();

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
        /// Helper method to create a <see cref="PlayerMissionSession"/> in the database
        /// </summary>
        /// <param name="playerMissionSession"><see cref="PlayerMissionSession"/> to create</param>
        /// <returns><see cref="PlayerMissionSession"/> with added database id</returns>
        private PlayerMissionSession CreatePlayerMissionSession(PlayerMissionSession playerMissionSession) {
            _addPlayerMissionSession.Parameters[DatabaseUtil.PLAYER_ID_KEY].Value = playerMissionSession.Player.Id;
            _addPlayerMissionSession.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = playerMissionSession.MissionSession.Id;
            _addPlayerMissionSession.ExecuteNonQuery();

            playerMissionSession.Id = GetLastInsertedId();
            _logger.DebugFormat("Player mission session inserted into the database with ID: {0}", playerMissionSession.Id);

            return playerMissionSession;
        }
    }
}
