using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using BWServerLogger.DAO;
using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Properties;
using BWServerLogger.Util;

namespace BWServerLogger.Service {
    class ReportingService {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingService));

        public ReportingService() {
        }

        public void StartReporting() {
            // define DAO objects to interact with databases
            MySqlConnection connection = null;
            PlayerDAO playerDAO = null;
            MissionDAO missionDAO = null;
            SessionDAO sessionDAO = null;

            try {
                // create DAO objects to interact with databases
                connection = DatabaseUtil.OpenDataSource();
                playerDAO = new PlayerDAO(connection);
                missionDAO = new MissionDAO(connection);
                sessionDAO = new SessionDAO(connection);
                
                ServerInfoService serverInfoService = new ServerInfoService();

                // variables needed to track session playthough
                Session session = null;
                int missionCount = 0;
                bool inGame = false;

                Stopwatch runTime = new Stopwatch();
                runTime.Start();
                try {
                    while (session == null && CheckTimeThreshold(runTime.ElapsedMilliseconds)) {
                        try {
                            session = SetUpSession(serverInfoService, sessionDAO, Settings.Default.armaServerAddress,
                                                   Settings.Default.armaServerPort, ref inGame);
                        } catch (MySqlException e) {
                            _logger.Error("Problem setting up session: ", e);
                        }
                        Thread.Sleep(Settings.Default.pollRate);
                    }

                    while (CheckMissionThreshold(missionCount, inGame) && CheckTimeThreshold(runTime.ElapsedMilliseconds)) {
                        try {
                            session = UpdateInfo(serverInfoService, sessionDAO, playerDAO, missionDAO,
                                                 Settings.Default.armaServerAddress, Settings.Default.armaServerPort,
                                                 session, ref missionCount, ref inGame);
                        } catch (MySqlException e) {
                            _logger.Error("Problem updating session details: ", e);
                        }
                        Thread.Sleep(Settings.Default.pollRate);
                    }
                } catch (NoServerInfoException nsie) {
                    _logger.Error("Error reporting", nsie);
                }
            }
            finally {
                // Ensure disposable objects are disposed
                if (connection != null) {
                    connection.Dispose();
                }
                if (playerDAO != null) {
                    playerDAO.Dispose();
                }
                if (missionDAO != null) {
                    missionDAO.Dispose();
                }
                if (sessionDAO != null) {
                    sessionDAO.Dispose();
                }
            }
        }

        private Session SetUpSession(ServerInfoService serverInfoService, SessionDAO sessionDAO, string host, int port, ref bool inGame) {
            Session returnSession = null;

            // initial info grab
            ServerInfo serverInfo = serverInfoService.GetServerInfo(host, port);
            inGame = UpdateServerRunningState(serverInfo.ServerState);

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

        private Session UpdateInfo(ServerInfoService serverInfoService, SessionDAO sessionDAO, PlayerDAO playerDAO, MissionDAO missionDAO, string host, int port, Session session, ref int missionCount, ref bool inGame) {
            ServerInfo serverInfo = serverInfoService.GetServerInfo(host, port);
            inGame = UpdateServerRunningState(serverInfo.ServerState);
            if (inGame) {
                UpdateSessionData(sessionDAO, session, serverInfo);
                MissionSession missionSession = UpdateMissionData(missionDAO, session, serverInfo, ref missionCount);
                ISet<PlayerSession> playerSession = UpdatePlayerData(playerDAO, session, serverInfo, missionSession);

            }
            return session;
        }

        private void UpdateSessionData(SessionDAO sessionDAO, Session session, ServerInfo serverInfo) {
            bool updateSession = false;

            if (session.MaxPing < serverInfo.Ping) {
                session.MaxPing = serverInfo.Ping;
                updateSession = true;
            } else if (session.MinPing > serverInfo.Ping) {
                session.MinPing = serverInfo.Ping;
                updateSession = true;
            }

            if (session.MaxPlayers < serverInfo.Players.Count) {
                session.MaxPlayers = serverInfo.Players.Count;
                updateSession = true;
            }

            if (updateSession) {
                sessionDAO.UpdateSession(session);
            }
        }

        private ISet<PlayerSession> UpdatePlayerData(PlayerDAO playerDAO, Session session, ServerInfo serverInfo, MissionSession missionSession) {
            ISet<PlayerSession> playerSessions = playerDAO.GetOrCreatePlayerSessions(serverInfo.Players, session);

            foreach (PlayerSession playerSession in playerSessions) {
                playerSession.Updated = true;
                playerSession.Length += (Settings.Default.pollRate / 1000);
                if (playerSession.Played == false && CheckPlayedThreshold(playerSession.Length)) {
                    playerSession.Played = true;
                }
            }

            playerDAO.UpdatePlayerSessions(playerSessions);
            return playerSessions;
        }

        private MissionSession UpdateMissionData(MissionDAO missionDAO, Session session, ServerInfo serverInfo, ref int missionCount) {
            MissionSession missionSession = missionDAO.GetOrCreateMissionSession(serverInfo.MapName, serverInfo.Mission, session);

            missionSession.Updated = true;
            missionSession.Length += (Settings.Default.pollRate / 1000);
            if (missionSession.Played == false && CheckPlayedThreshold(missionSession.Length)) {
                missionSession.Played = true;
                missionCount++;
            }

            missionDAO.UpdateMissionSession(missionSession);

            return missionSession;
        }

        private void UpdatePTSTMTSData(PlayerSessionToMissionSessionDAO pstmsDAO, MissionSession missionSession, ISet<PlayerSession> playerSessions) {
            ISet<PlayerSessionToMissionSession> pstmses = pstmsDAO.GetOrCreatePSTMS(missionSession, playerSessions);

            foreach (PlayerSessionToMissionSession pstms in pstmses) {
                pstms.Updated = true;
                pstms.Length += (Settings.Default.pollRate / 1000);
                if (pstms.Played == false && CheckPlayedThreshold(pstms.Length)) {
                    pstms.Played = true;
                }
            }

            pstmsDAO.UpdatePSTMS(pstmses);
        }

        private bool UpdateServerRunningState(int serverState) {
            return serverState == ServerInfoConstants.IN_GAME;
        }

        private bool CheckPlayedThreshold(int length) {
            return length > Properties.Settings.Default.playedThreshold;
        }

        private bool CheckTimeThreshold(long elapsedTime) {
            return elapsedTime < Properties.Settings.Default.runTimeThreshold;
        }

        private bool CheckMissionThreshold(int missionCount, bool inGame) {
            return missionCount < Properties.Settings.Default.missionThreshold ||
                (missionCount == Properties.Settings.Default.missionThreshold && inGame);
        }
    }
}
