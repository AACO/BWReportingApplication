using MySql.Data.MySqlClient;

using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Mission database access object to deal with <see cref="Map"/>, <see cref="Mission"/>, and <see cref="MissionSession"/> objects. Extends <see cref="BaseDAO"/>.
    /// </summary>
    /// <seealso cref="BaseDAO"/>
    public class MissionDAO : BaseDAO {
        private MissionSession _cachedMissionSession;
        private MySqlCommand _getMap;
        private MySqlCommand _addMap;
        private MySqlCommand _getMissionSession;
        private MySqlCommand _addMissionSession;
        private MySqlCommand _updateMissionSession;
        private MySqlCommand _addMission;
        private MySqlCommand _updateMission;

        /// <summary>
        /// Constructor, sets up prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/>, used to create prepared statements</param>
        /// <seealso cref="BaseDAO(MySqlConnection)"/>
        public MissionDAO(MySqlConnection connection) : base(connection) {
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_getMap != null) {
                    _getMap.Dispose();
                }
                if (_addMap != null) {
                    _addMap.Dispose();
                }
                if (_getMissionSession != null) {
                    _getMissionSession.Dispose();
                }
                if (_addMissionSession != null) {
                    _addMissionSession.Dispose();
                }
                if (_updateMissionSession != null) {
                    _updateMissionSession.Dispose();
                }
                if (_addMission != null) {
                    _addMission.Dispose();
                }
                if (_updateMission != null) {
                    _updateMission.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets, or creates a mission session with the provided map name, mission name, and <see cref="Session"/>.
        /// Caches the return value to reduce MySQL interactions and decrease run time
        /// </summary>
        /// <param name="mapName">Name of the map that the server is on</param>
        /// <param name="mission">Name of the mission that the server is on</param>
        /// <param name="session">Current <see cref="Session"/> object</param>
        /// <returns>A full <see cref="MissionSession"/> object with information from the database</returns>
        public MissionSession GetOrCreateMissionSession(string mapName, string mission, Session session) {
            

            MissionSession missionSession = new MissionSession();
            missionSession.Mission = new Mission(mission);
            missionSession.Mission.Map = new Map(mapName);
            missionSession.Session = session;

            if (IsMissionSessionCached(_cachedMissionSession, missionSession)) {
                missionSession = _cachedMissionSession;
                _logger.DebugFormat("Retrieved mission session from cache with ID: {0}", missionSession.Id);
            } else {
                _getMissionSession.Parameters[DatabaseUtil.NAME_KEY].Value = missionSession.Mission.Name;
                _getMissionSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;

                MySqlDataReader getMissionResult = _getMissionSession.ExecuteReader();

                if (getMissionResult.HasRows) {
                    bool relinkMap = false;

                    getMissionResult.Read();
                    missionSession.Mission.Id = getMissionResult.GetInt32(0);

                    if (missionSession.Mission.Map.Name.Equals(getMissionResult.GetString(5))) {
                        missionSession.Mission.Map.Id = getMissionResult.GetInt32(4);
                    } else {
                        relinkMap = true;
                    }

                    if (getMissionResult.IsDBNull(1)) {
                        getMissionResult.Close();
                        CreateMissionSession(missionSession);
                    } else {
                        missionSession.Id = getMissionResult.GetInt32(1);
                        missionSession.Length = getMissionResult.GetInt32(2);
                        missionSession.Played = getMissionResult.GetBoolean(3);
                        _logger.DebugFormat("Retrieved mission session from database with ID: {0}", missionSession.Id);

                        getMissionResult.Close();
                    }

                    if (relinkMap) {
                        missionSession.Mission.Map.Id = GetMapId(missionSession.Mission.Map.Name);
                        _updateMission.Parameters[DatabaseUtil.MISSION_ID_KEY].Value = missionSession.Mission.Id;
                        _updateMission.Parameters[DatabaseUtil.MAP_ID_KEY].Value = missionSession.Mission.Map.Id;

                        _updateMission.ExecuteNonQuery();
                    }
                } else {
                    getMissionResult.Close();

                    missionSession.Mission.Map.Id = GetMapId(missionSession.Mission.Map.Name);

                    _addMission.Parameters[DatabaseUtil.NAME_KEY].Value = missionSession.Mission.Name;
                    _addMission.Parameters[DatabaseUtil.MAP_ID_KEY].Value = missionSession.Mission.Map.Id;
                    _addMission.ExecuteNonQuery();

                    missionSession.Mission.Id = GetLastInsertedId();

                    CreateMissionSession(missionSession);

                    _cachedMissionSession = missionSession;
                }
            }
            
            return missionSession;
        }

        /// <summary>
        /// Update the database with the modified <see cref="MissionSession"/> object
        /// </summary>
        /// <param name="missionSession"><see cref="MissionSession"/> object to update</param>
        public void UpdateMissionSession(MissionSession missionSession) {
            if (missionSession.Updated) {
                _updateMissionSession.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = missionSession.Id;
                _updateMissionSession.Parameters[DatabaseUtil.LENGTH_KEY].Value = missionSession.Length;
                _updateMissionSession.Parameters[DatabaseUtil.PLAYED_KEY].Value = missionSession.Played;
                _updateMissionSession.ExecuteNonQuery();

                _logger.DebugFormat("Mission session updated on the database with ID: {0}", missionSession.Id);
            }
        }

        /// <summary>
        /// Gets (or creates) the map ID for the given map name
        /// </summary>
        /// <param name="mapName">Map name for ID look up</param>
        /// <returns>The map id for the given map name</returns>
        private int GetMapId(string mapName) {
            int mapId = -1;

            _getMap.Parameters[DatabaseUtil.NAME_KEY].Value = mapName;
            MySqlDataReader getMapResult = _getMap.ExecuteReader();

            if (getMapResult.HasRows) {
                getMapResult.Read();
                mapId = getMapResult.GetInt32(0);
                getMapResult.Close();
            } else {
                getMapResult.Close();

                _addMap.Parameters[DatabaseUtil.FRIENDLY_NAME_KEY].Value = mapName;
                _addMap.Parameters[DatabaseUtil.NAME_KEY].Value = mapName;
                _addMap.ExecuteNonQuery();

                mapId = GetLastInsertedId();
            }

            return mapId;
        }

        /// <summary>
        /// Method to setup prepared statements
        /// </summary>
        /// <param name="connection">Open <see cref="MySqlConnection"/> used to prepare statements</param>
        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder getMissionSessionSelect = new StringBuilder();
            getMissionSessionSelect.Append("select m.id, ");
            getMissionSessionSelect.Append("mts.id, mts.length, mts.played, ");
            getMissionSessionSelect.Append("map.id, map.name ");
            getMissionSessionSelect.Append("from mission m ");
            getMissionSessionSelect.Append("left join mission_to_session mts on mts.mission_id = m.id and mts.session_id = ");
            getMissionSessionSelect.Append(DatabaseUtil.SESSION_ID_KEY);
            getMissionSessionSelect.Append(" ");
            getMissionSessionSelect.Append("left join map on m.map_id = map.id ");
            getMissionSessionSelect.Append("where m.name = ");
            getMissionSessionSelect.Append(DatabaseUtil.NAME_KEY);

            _getMissionSession = new MySqlCommand(getMissionSessionSelect.ToString(), connection);
            _getMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _getMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _getMissionSession.Prepare();

            StringBuilder addMissionSessionInsert = new StringBuilder();
            addMissionSessionInsert.Append("insert into mission_to_session (mission_id, session_id) ");
            addMissionSessionInsert.Append("values (");
            addMissionSessionInsert.Append(DatabaseUtil.MISSION_ID_KEY);
            addMissionSessionInsert.Append(", ");
            addMissionSessionInsert.Append(DatabaseUtil.SESSION_ID_KEY);
            addMissionSessionInsert.Append(")");

            _addMissionSession = new MySqlCommand(addMissionSessionInsert.ToString(), connection);
            _addMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_ID_KEY, MySqlDbType.Int32));
            _addMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _addMissionSession.Prepare();

            StringBuilder getMapSelect = new StringBuilder();
            getMapSelect.Append("select id, name ");
            getMapSelect.Append("from map ");
            getMapSelect.Append("where name = ");
            getMapSelect.Append(DatabaseUtil.NAME_KEY);

            _getMap = new MySqlCommand(getMapSelect.ToString(), connection);
            _getMap.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _getMap.Prepare();

            StringBuilder addMapInsert = new StringBuilder();
            addMapInsert.Append("insert into map (friendly_name, name) ");
            addMapInsert.Append("values (");
            addMapInsert.Append(DatabaseUtil.FRIENDLY_NAME_KEY);
            addMapInsert.Append(", ");
            addMapInsert.Append(DatabaseUtil.NAME_KEY);
            addMapInsert.Append(")");

            _addMap = new MySqlCommand(addMapInsert.ToString(), connection);
            _addMap.Parameters.Add(new MySqlParameter(DatabaseUtil.FRIENDLY_NAME_KEY, MySqlDbType.String));
            _addMap.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _addMap.Prepare();

            StringBuilder addMissionInsert = new StringBuilder();
            addMissionInsert.Append("insert into mission (name, map_id, description, target_player_count)");
            addMissionInsert.Append("values (");
            addMissionInsert.Append(DatabaseUtil.NAME_KEY);
            addMissionInsert.Append(", ");
            addMissionInsert.Append(DatabaseUtil.MAP_ID_KEY);
            addMissionInsert.Append(", '', ");
            addMissionInsert.Append(DatabaseUtil.DEFAULT_PLAYER_COUNT);
            addMissionInsert.Append(")");

            _addMission = new MySqlCommand(addMissionInsert.ToString(), connection);
            _addMission.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _addMission.Parameters.Add(new MySqlParameter(DatabaseUtil.MAP_ID_KEY, MySqlDbType.Int32));
            _addMission.Prepare();

            StringBuilder missionSessionUpdate = new StringBuilder();
            missionSessionUpdate.Append("update mission_to_session ");
            missionSessionUpdate.Append("set length = ");
            missionSessionUpdate.Append(DatabaseUtil.LENGTH_KEY);
            missionSessionUpdate.Append(", played = ");
            missionSessionUpdate.Append(DatabaseUtil.PLAYED_KEY);
            missionSessionUpdate.Append(" ");
            missionSessionUpdate.Append("where id = ");
            missionSessionUpdate.Append(DatabaseUtil.MISSION_TO_SESSION_ID_KEY);

            _updateMissionSession = new MySqlCommand(missionSessionUpdate.ToString(), connection);
            _updateMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.LENGTH_KEY, MySqlDbType.Int32));
            _updateMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYED_KEY, MySqlDbType.Bit));
            _updateMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _updateMissionSession.Prepare();

            StringBuilder missionUpdate = new StringBuilder();
            missionUpdate.Append("update mission ");
            missionUpdate.Append("set map_id = ");
            missionUpdate.Append(DatabaseUtil.MAP_ID_KEY);
            missionUpdate.Append(" ");
            missionUpdate.Append("where id = ");
            missionUpdate.Append(DatabaseUtil.MISSION_ID_KEY);

            _updateMission = new MySqlCommand(missionUpdate.ToString(), connection);
            _updateMission.Parameters.Add(new MySqlParameter(DatabaseUtil.MAP_ID_KEY, MySqlDbType.Int32));
            _updateMission.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_ID_KEY, MySqlDbType.Int32));
            _updateMission.Prepare();
        }

        /// <summary>
        /// Check to see if the <see cref="MissionSession"/> is cached
        /// </summary>
        /// <param name="cachedMissionSession">The current cached <see cref="MissionSession"/></param>
        /// <param name="lookupMissionSession">The current <see cref="MissionSession"/> to check</param>
        /// <returns>True if the cached <see cref="MissionSession"/> matches the lookup <see cref="MissionSession"/></returns>
        private bool IsMissionSessionCached(MissionSession cachedMissionSession, MissionSession lookupMissionSession) {
            return cachedMissionSession != null && cachedMissionSession.Mission != null &&
                cachedMissionSession.Session != null && cachedMissionSession.Mission.Map != null &&
                cachedMissionSession.Mission.Name == lookupMissionSession.Mission.Name &&
                cachedMissionSession.Session.Id == lookupMissionSession.Session.Id &&
                cachedMissionSession.Mission.Map.Name == lookupMissionSession.Mission.Map.Name;
        }

        /// <summary>
        /// Helper method to create a <see cref="MissionSession"/> in the database
        /// </summary>
        /// <param name="missionSession"><see cref="MissionSession"/> to create</param>
        /// <returns><see cref="MissionSession"/> with added database id</returns>
        private MissionSession CreateMissionSession(MissionSession missionSession) {
            _addMissionSession.Parameters[DatabaseUtil.MISSION_ID_KEY].Value = missionSession.Mission.Id;
            _addMissionSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = missionSession.Session.Id;
            _addMissionSession.ExecuteNonQuery();

            missionSession.Id = GetLastInsertedId();
            _logger.DebugFormat("Mission session added to the database with ID: {0}", missionSession.Id);

            return missionSession;
        }
    }
}
