using log4net;

using MySql.Data;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO
{
    public class MissionDAO
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MissionDAO));

        private MySqlConnection _connection;
        private MySqlCommand _getMap;
        private MySqlCommand _addMap;
        private MySqlCommand _getMissionSession;
        private MySqlCommand _addMissionSession;
        private MySqlCommand _updateMissionSession;
        private MySqlCommand _addMission;
        private MySqlCommand _updateMission;

        public MissionDAO(MySqlConnection connection)
        {
            _connection = connection;
            SetupPreparedStatements();
        }

        public MissionSession GetOrCreateMissionSession(string mapName, string mission, Session session)
        {
            MissionSession missionSession = new MissionSession();
            missionSession.Mission = new Mission(mission);
            missionSession.Mission.Map = new Map(mapName);
            missionSession.Session = session;

            _getMissionSession.Parameters[DatabaseUtil.NAME_KEY].Value = missionSession.Mission.Name;
            _getMissionSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;

            MySqlDataReader getMissionResult = _getMissionSession.ExecuteReader();

            if (getMissionResult.HasRows)
            {
                bool relinkMap = false;

                getMissionResult.Read();
                missionSession.Mission.Id = getMissionResult.GetInt32(0);

                if (missionSession.Mission.Map.Name.Equals(getMissionResult.GetString(5)))
                {
                    missionSession.Mission.Map.Id = getMissionResult.GetInt32(4);
                }
                else
                {
                    relinkMap = true;
                }

                if (getMissionResult.IsDBNull(1))
                {
                    getMissionResult.Close();

                    _addMissionSession.Parameters[DatabaseUtil.MISSION_ID_KEY].Value = missionSession.Mission.Id;
                    _addMissionSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
                    _addMissionSession.ExecuteNonQuery();

                    missionSession.Id = DatabaseUtil.GetLastInsertedId(ref _connection);
                }
                else
                {
                    missionSession.Id = getMissionResult.GetInt32(1);
                    missionSession.Length = getMissionResult.GetInt32(2);
                    missionSession.Played = getMissionResult.GetBoolean(3);
                    getMissionResult.Close();
                }

                if (relinkMap)
                {
                    missionSession.Mission.Map.Id = GetMapId(missionSession.Mission.Map.Name);
                    _updateMission.Parameters[DatabaseUtil.MISSION_ID_KEY].Value = missionSession.Mission.Id;
                    _updateMission.Parameters[DatabaseUtil.MAP_ID_KEY].Value = missionSession.Mission.Map.Id;

                    _updateMission.ExecuteNonQuery();
                }
            }
            else
            {
                getMissionResult.Close();

                missionSession.Mission.Map.Id = GetMapId(missionSession.Mission.Map.Name);

                _addMission.Parameters[DatabaseUtil.NAME_KEY].Value = missionSession.Mission.Name;
                _addMission.Parameters[DatabaseUtil.MAP_ID_KEY].Value = missionSession.Mission.Map.Id;
                _addMission.ExecuteNonQuery();

                missionSession.Mission.Id = DatabaseUtil.GetLastInsertedId(ref _connection);

                _addMissionSession.Parameters[DatabaseUtil.MISSION_ID_KEY].Value = missionSession.Mission.Id;
                _addMissionSession.Parameters[DatabaseUtil.SESSION_ID_KEY].Value = session.Id;
                _addMissionSession.ExecuteNonQuery();

                missionSession.Id = DatabaseUtil.GetLastInsertedId(ref _connection);
            }

            return missionSession;
        }

        public void UpdateMissionSession(MissionSession missionSession)
        {
            if (missionSession.Updated)
            {
                _updateMissionSession.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = missionSession.Id;
                _updateMissionSession.Parameters[DatabaseUtil.LENGTH_KEY].Value = missionSession.Length;
                _updateMissionSession.Parameters[DatabaseUtil.PLAYED_KEY].Value = missionSession.Played;
                _updateMissionSession.ExecuteNonQuery();
            }
        }

        private int GetMapId(string mapName)
        {
            int mapId = -1;

            _getMap.Parameters[DatabaseUtil.NAME_KEY].Value = mapName;
            MySqlDataReader getMapResult = _getMap.ExecuteReader();

            if (getMapResult.HasRows)
            {
                getMapResult.Read();
                mapId = getMapResult.GetInt32(0);
                getMapResult.Close();
            }
            else
            {
                getMapResult.Close();

                _addMap.Parameters[DatabaseUtil.FRIENDLY_NAME_KEY].Value = mapName;
                _addMap.Parameters[DatabaseUtil.NAME_KEY].Value = mapName;
                _addMap.ExecuteNonQuery();

                mapId = DatabaseUtil.GetLastInsertedId(ref _connection);
            }

            return mapId;
        }

        private void SetupPreparedStatements()
        {
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

            _getMissionSession = new MySqlCommand(getMissionSessionSelect.ToString(), _connection);
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

            _addMissionSession = new MySqlCommand(addMissionSessionInsert.ToString(), _connection);
            _addMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_ID_KEY, MySqlDbType.Int32));
            _addMissionSession.Parameters.Add(new MySqlParameter(DatabaseUtil.SESSION_ID_KEY, MySqlDbType.Int32));
            _addMissionSession.Prepare();

            StringBuilder getMapSelect = new StringBuilder();
            getMapSelect.Append("select id, name ");
            getMapSelect.Append("from map ");
            getMapSelect.Append("where name = ");
            getMapSelect.Append(DatabaseUtil.NAME_KEY);

            _getMap = new MySqlCommand(getMapSelect.ToString(), _connection);
            _getMap.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _getMap.Prepare();

            StringBuilder addMapInsert = new StringBuilder();
            addMapInsert.Append("insert into map (friendly_name, name) ");
            addMapInsert.Append("values (");
            addMapInsert.Append(DatabaseUtil.FRIENDLY_NAME_KEY);
            addMapInsert.Append(", ");
            addMapInsert.Append(DatabaseUtil.NAME_KEY);
            addMapInsert.Append(")");

            _addMap = new MySqlCommand(addMapInsert.ToString(), _connection);
            _addMap.Parameters.Add(new MySqlParameter(DatabaseUtil.FRIENDLY_NAME_KEY, MySqlDbType.String));
            _addMap.Parameters.Add(new MySqlParameter(DatabaseUtil.NAME_KEY, MySqlDbType.String));
            _addMap.Prepare();

            StringBuilder addMissionInsert = new StringBuilder();
            addMissionInsert.Append("insert into mission (name, map_id, description, notes, min_players, max_players)");
            addMissionInsert.Append("values (");
            addMissionInsert.Append(DatabaseUtil.NAME_KEY);
            addMissionInsert.Append(", ");
            addMissionInsert.Append(DatabaseUtil.MAP_ID_KEY);
            addMissionInsert.Append(", '', '', ");
            addMissionInsert.Append(DatabaseUtil.DEFAULT_PLAYER_COUNT);
            addMissionInsert.Append(", ");
            addMissionInsert.Append(DatabaseUtil.DEFAULT_PLAYER_COUNT);
            addMissionInsert.Append(")");

            _addMission = new MySqlCommand(addMissionInsert.ToString(), _connection);
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

            _updateMissionSession = new MySqlCommand(missionSessionUpdate.ToString(), _connection);
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

            _updateMission = new MySqlCommand(missionUpdate.ToString(), _connection);
            _updateMission.Parameters.Add(new MySqlParameter(DatabaseUtil.MAP_ID_KEY, MySqlDbType.Int32));
            _updateMission.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_ID_KEY, MySqlDbType.Int32));
            _updateMission.Prepare();
        }
    }
}
