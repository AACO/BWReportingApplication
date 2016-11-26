using log4net;

using MySql.Data.MySqlClient;

using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;

using BWServerLogger.DAO;
using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Properties;
using BWServerLogger.Util;

namespace BWServerLogger.Service {
    /// <summary>
    /// Service to report on a session
    /// </summary>
    class ReportingService {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingService));

        /// <summary>
        /// Default constructor
        /// </summary>
        public ReportingService() {
        }

        /// <summary>
        /// Begins reporting on a session, should handle DAOs here
        /// </summary>
        public void StartReporting() {
            // define DAO objects to interact with databases
            MySqlConnection connection = null;
            PlayerDAO playerDAO = null;
            MissionDAO missionDAO = null;
            SessionDAO sessionDAO = null;

            try {
                // create DAO objects to interact with databases
                BuildDAOsIfNeeded(ref connection, ref playerDAO, ref missionDAO, ref sessionDAO);

                ServerInfoService serverInfoService = new ServerInfoService();

                // variables needed to track session playthough
                Session session = null;
                MissionSession currentMissionSession = null;
                ISet<PlayerMissionSession> currentPlayersToMissionSession = new HashSet<PlayerMissionSession>();
                int missionCount = 0;
                bool inGame = false;

                Stopwatch runTime = new Stopwatch();
                runTime.Start();
                try {
                    while (session == null && CheckTimeThreshold(runTime.ElapsedMilliseconds)) {
                        try {
                            _logger.Debug("Trying to set up session");
                            session = SetUpSession(serverInfoService, sessionDAO, Settings.Default.armaServerAddress,
                                                   Settings.Default.armaServerPort, ref inGame);
                        } catch (MySqlException e) {
                            _logger.Error("Problem setting up session: ", e);
                            BuildDAOsIfNeeded(ref connection, ref playerDAO, ref missionDAO, ref sessionDAO);
                        }
                        Thread.Sleep(Settings.Default.pollRate);
                    }

                    while (CheckMissionThreshold(missionCount, inGame) && CheckTimeThreshold(runTime.ElapsedMilliseconds)) {
                        try {
                            _logger.Debug("Trying to update session details");
                            ServerInfo serverInfo = serverInfoService.GetServerInfo(Settings.Default.armaServerAddress, Settings.Default.armaServerPort);
                            inGame = CheckServerRunningState(serverInfo.ServerState);

                            if (inGame) {
                                if (session.MaxPing < serverInfo.Ping) {
                                    session.MaxPing = serverInfo.Ping;
                                } else if (session.MinPing > serverInfo.Ping) {
                                    session.MinPing = serverInfo.Ping;
                                }

                                if (session.MaxPlayers < serverInfo.Players.Count) {
                                    session.MaxPlayers = serverInfo.Players.Count;
                                }

                                MissionSession missionSession = missionDAO.GetOrCreateMissionSession(serverInfo.MapName, serverInfo.Mission, session);
                                if (currentMissionSession != null && currentMissionSession.Id != missionSession.Id) { // handle case where missions changed between polls
                                    sessionDAO.UpdateSession(session);
                                    missionDAO.UpdateMissionSession(currentMissionSession);
                                    playerDAO.UpdatePlayerMissionSessions(currentPlayersToMissionSession);

                                    currentPlayersToMissionSession.Clear();
                                }

                                missionSession.Length += (Settings.Default.pollRate / 1000);
                                if (missionSession.Played == false && CheckPlayedThreshold(missionSession.Length)) {
                                    missionSession.Played = true;
                                    missionCount++;
                                }

                                currentMissionSession = missionSession;

                                ISet<PlayerMissionSession> playersToMissionSession = playerDAO.GetOrCreatePlayerMissionSessions(serverInfo.Players, missionSession);
                                
                                foreach (PlayerMissionSession playerToMissionSession in playersToMissionSession) {
                                    playerToMissionSession.Length += (Settings.Default.pollRate / 1000);
                                    if (playerToMissionSession.Played == false && CheckPlayedThreshold(playerToMissionSession.Length)) {
                                        playerToMissionSession.Played = true;
                                    }
                                }

                                currentPlayersToMissionSession.UnionWith(playersToMissionSession);
                            } else if (currentMissionSession != null) {
                                sessionDAO.UpdateSession(session);
                                missionDAO.UpdateMissionSession(currentMissionSession);
                                playerDAO.UpdatePlayerMissionSessions(currentPlayersToMissionSession);

                                currentMissionSession = null;
                                currentPlayersToMissionSession.Clear();
                            }
                        } catch (MySqlException e) {
                            _logger.Error("Problem updating session details: ", e);
                            BuildDAOsIfNeeded(ref connection, ref playerDAO, ref missionDAO, ref sessionDAO);
                        }
                        Thread.Sleep(Settings.Default.pollRate);
                    }
                } catch (NoServerInfoException nsie) {
                    _logger.Error("Error reporting", nsie);
                }
            }
            finally {
                // Ensure disposable objects are disposed
                if (playerDAO != null) {
                    playerDAO.Dispose();
                }
                if (missionDAO != null) {
                    missionDAO.Dispose();
                }
                if (sessionDAO != null) {
                    sessionDAO.Dispose();
                }
                if (connection != null) {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Helper method to create a new session
        /// </summary>
        /// <param name="serverInfoService">service to get info from an A3 server</param>
        /// <param name="sessionDAO">DAO for sessions</param>
        /// <param name="host">A3 server host</param>
        /// <param name="port">A3 server port</param>
        /// <param name="inGame">Boolean to check if the server is currently in game</param>
        /// <returns>The new session</returns>
        private Session SetUpSession(ServerInfoService serverInfoService, SessionDAO sessionDAO, string host, int port, ref bool inGame) {
            Session returnSession = null;

            // initial info grab
            ServerInfo serverInfo = serverInfoService.GetServerInfo(host, port);
            inGame = CheckServerRunningState(serverInfo.ServerState);

            if (inGame) {
                // create a session
                Session session = new Session();
                session.HostName = serverInfo.HostName;
                session.Version = serverInfo.GameVersion;
                session.MaxPing = serverInfo.Ping;
                session.MinPing = serverInfo.Ping;
                session.MaxPlayers = serverInfo.NumPlayers;
                session = sessionDAO.CreateSession(session);

                returnSession = session;
            }

            return returnSession;
        }

        /// <summary>
        /// Helper method to update the session info
        /// </summary>
        /// <param name="serverInfoService">service to get info from an A3 server</param>
        /// <param name="sessionDAO">DAO for <see cref="Session"/>s</param>
        /// <param name="playerDAO">DAO for <see cref="Player"/>s</param>
        /// <param name="missionDAO">DAO for <see cref="Mission"/>s</param>
        /// <param name="host">A3 server host</param>
        /// <param name="port">A3 server port</param>
        /// <param name="session">The current game <see cref="Session"/></param>
        /// <param name="missionCount">The number of missions played</param>
        /// <param name="inGame">Boolean to check if the server is currently in game</param>
        /// <returns>The current mission session</returns>
        private Session UpdateInfo(ServerInfoService serverInfoService, SessionDAO sessionDAO, PlayerDAO playerDAO, MissionDAO missionDAO,
                                   string host, int port, Session session, ref int missionCount, ref bool inGame) {



            return session;
        }

        /// <summary>
        /// Build DAOs if we need to build DAOs
        /// </summary>
        /// <param name="connection">the current <see cref="MySqlConnection"/></param>
        /// <param name="playerDAO">DAO for <see cref="Player"/>s</param>
        /// <param name="missionDAO">DAO for <see cref="Mission"/>s</param>
        /// <param name="sessionDAO">DAO for <see cref="Session"/>s</param>
        private void BuildDAOsIfNeeded(ref MySqlConnection connection,
                                       ref PlayerDAO playerDAO,
                                       ref MissionDAO missionDAO,
                                       ref SessionDAO sessionDAO) {

            if (connection == null || connection.State == ConnectionState.Closed) {
                connection = DatabaseUtil.OpenDataSource();
                playerDAO = new PlayerDAO(connection);
                missionDAO = new MissionDAO(connection);
                sessionDAO = new SessionDAO(connection);
            }
        }

        /// <summary>
        /// Checks the server state
        /// </summary>
        /// <param name="serverState"></param>
        /// <returns></returns>
        private bool CheckServerRunningState(int serverState) {
            return serverState == ServerInfoConstants.IN_GAME;
        }

        /// <summary>
        /// Checks if the item has past the played threshold
        /// </summary>
        /// <param name="length">Length the item has been played for</param>
        /// <returns>True if the item is past the played threashold, false otherwise</returns>
        private bool CheckPlayedThreshold(int length) {
            return length > Settings.Default.playedThreshold;
        }

        /// <summary>
        /// Check to see if reporting is past the run time threshold
        /// </summary>
        /// <param name="elapsedTime">Length the report has been running for</param>
        /// <returns>True if the report is past the running threashold, false otherwise</returns>
        private bool CheckTimeThreshold(long elapsedTime) {
            return elapsedTime < Settings.Default.runTimeThreshold;
        }

        /// <summary>
        /// Check if the missions are past the mission count
        /// </summary>
        /// <param name="missionCount">The number of missions played</param>
        /// <param name="inGame">Boolean to check if the server is currently in game</param>
        /// <returns>True if the missions are past the mission count, false otherwise</returns>
        private bool CheckMissionThreshold(int missionCount, bool inGame) {
            return missionCount < Settings.Default.missionThreshold ||
                (missionCount == Settings.Default.missionThreshold && inGame);
        }
    }
}
