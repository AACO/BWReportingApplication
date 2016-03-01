using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Text;

using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO {
    public class PlayerSessionToMissionSessionDAO : BaseDAO {
        private int _cachedMissionSessionId;
        private IDictionary<int, PlayerSessionToMissionSession> _cachedPlayerSessionsToPSTMS;
        private MySqlCommand _getPSTMS;
        private MySqlCommand _addPSTMS;
        private MySqlCommand _updatePSTMS;

        public PlayerSessionToMissionSessionDAO(MySqlConnection connection) : base(connection) {
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing) {
                if (_getPSTMS != null) {
                    _getPSTMS.Dispose();
                }
                if (_addPSTMS != null) {
                    _addPSTMS.Dispose();
                }
                if (_updatePSTMS != null) {
                    _updatePSTMS.Dispose();
                }
            }
        }

        public ISet<PlayerSessionToMissionSession> GetOrCreatePSTMS(MissionSession missionSession, ISet<PlayerSession> playerSessions) {
            ISet<PlayerSessionToMissionSession> pstmses = new HashSet<PlayerSessionToMissionSession>();

            // our cached mission session id is invalid, reset the caches
            if (_cachedMissionSessionId != missionSession.Id) {
                _cachedMissionSessionId = missionSession.Id;
                _cachedPlayerSessionsToPSTMS = new Dictionary<int, PlayerSessionToMissionSession>();
            }

            foreach (PlayerSession playerSession in playerSessions) {
                PlayerSessionToMissionSession pstms = new PlayerSessionToMissionSession();
                pstms.PlayerSessionId = playerSession.Id;
                pstms.MissionSessionId = missionSession.Id;

                if (_cachedPlayerSessionsToPSTMS.ContainsKey(playerSession.Id)) {
                    _cachedPlayerSessionsToPSTMS.TryGetValue(playerSession.Id, out pstms);
                } else {
                    //get
                    _getPSTMS.Parameters[DatabaseUtil.PLAYER_TO_SESSION_ID_KEY].Value = playerSession.Id;
                    _getPSTMS.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = missionSession.Id;

                    MySqlDataReader getPTSTMTSResult = _getPSTMS.ExecuteReader();

                    if (getPTSTMTSResult.HasRows) {
                        getPTSTMTSResult.Read();

                        pstms.Id = getPTSTMTSResult.GetInt32(0);
                        pstms.Length = getPTSTMTSResult.GetInt32(1);
                        pstms.Played = getPTSTMTSResult.GetBoolean(2);

                        getPTSTMTSResult.Close();
                    } else {
                        //add
                        getPTSTMTSResult.Close();

                        _addPSTMS.Parameters[DatabaseUtil.PLAYER_TO_SESSION_ID_KEY].Value = playerSession.Id;
                        _addPSTMS.Parameters[DatabaseUtil.MISSION_TO_SESSION_ID_KEY].Value = missionSession.Id;
                        _addPSTMS.ExecuteNonQuery();

                        pstms.Id = GetLastInsertedId();
                    }

                    
                }
                pstmses.Add(pstms);
            }

            return pstmses;
        }

        public void UpdatePSTMS(ISet<PlayerSessionToMissionSession> pstmses) {
            foreach (PlayerSessionToMissionSession pstms in pstmses) {
                UpdatePSTMS(pstms);
            }
        }

        public void UpdatePSTMS(PlayerSessionToMissionSession pstms) {
            if (pstms.Updated) {
                _updatePSTMS.Parameters[DatabaseUtil.PLAYED_KEY].Value = pstms.Played;
                _updatePSTMS.Parameters[DatabaseUtil.LENGTH_KEY].Value = pstms.Length;
                _updatePSTMS.Parameters[DatabaseUtil.PLAYER_TO_SESSION_TO_MISSION_TO_SESSION_ID_KEY].Value = pstms.Id;
                _updatePSTMS.ExecuteNonQuery();
            }
        }

        protected override void SetupPreparedStatements(MySqlConnection connection) {
            StringBuilder getPSTMSSelect = new StringBuilder();
            getPSTMSSelect.Append("select id, length, played ");
            getPSTMSSelect.Append("from player_to_session_to_mission_to_session");
            getPSTMSSelect.Append("where player_to_session_id = ");
            getPSTMSSelect.Append(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY);
            getPSTMSSelect.Append(" and mission_to_session_id = ");
            getPSTMSSelect.Append(DatabaseUtil.MISSION_TO_SESSION_ID_KEY);

            _getPSTMS = new MySqlCommand(getPSTMSSelect.ToString(), connection);
            _getPSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _getPSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _getPSTMS.Prepare();

            StringBuilder addPSTMSInsert = new StringBuilder();
            addPSTMSInsert.Append("insert into player_to_session_to_mission_to_session (player_to_session_id, mission_to_session_id) ");
            addPSTMSInsert.Append("values (");
            addPSTMSInsert.Append(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY);
            addPSTMSInsert.Append(", ");
            addPSTMSInsert.Append(DatabaseUtil.MISSION_TO_SESSION_ID_KEY);
            addPSTMSInsert.Append(")");

            _addPSTMS = new MySqlCommand(addPSTMSInsert.ToString(), connection);
            _addPSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _addPSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _addPSTMS.Prepare();

            StringBuilder pstmsUpdate = new StringBuilder();
            pstmsUpdate.Append("update player_to_session_to_mission_to_session ");
            pstmsUpdate.Append("set length = ");
            pstmsUpdate.Append(DatabaseUtil.LENGTH_KEY);
            pstmsUpdate.Append(", played = ");
            pstmsUpdate.Append(DatabaseUtil.PLAYED_KEY);
            pstmsUpdate.Append(" ");
            pstmsUpdate.Append("where id = ");
            pstmsUpdate.Append(DatabaseUtil.PLAYER_TO_SESSION_TO_MISSION_TO_SESSION_ID_KEY);

            _updatePSTMS = new MySqlCommand(pstmsUpdate.ToString(), connection);
            _updatePSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.LENGTH_KEY, MySqlDbType.Int32));
            _updatePSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYED_KEY, MySqlDbType.Bit));
            _updatePSTMS.Parameters.Add(new MySqlParameter(DatabaseUtil.PLAYER_TO_SESSION_TO_MISSION_TO_SESSION_ID_KEY, MySqlDbType.Int32));
            _updatePSTMS.Prepare();
        }
    }
}
