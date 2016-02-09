using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using BWServerLogger.DAO;
using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.Service
{
    class ReportingService
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(ReportingService));

        private volatile bool _kill;

        private int _missionCount = 0;
        private bool _inGame = false;

        private PlayerDAO _playerDAO;
        private MissionDAO _missionDAO;
        private SessionDAO _sessionDAO;
        private MySqlConnection _connection;
        private ServerInfoService _serverInfoService;

        public ReportingService() : this(new ServerInfoService())
        {
        }

        public ReportingService(ServerInfoService serverInfoService)
        {
            _connection = DatabaseUtil.OpenDataSource();
            _playerDAO = new PlayerDAO(_connection);
            _missionDAO = new MissionDAO(_connection);
            _sessionDAO = new SessionDAO(_connection);
            _serverInfoService = serverInfoService;
        }

        ~ReportingService()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        public void StartReporting()
        {
            _kill = false;
            Session session = null;

            Stopwatch runTime = new Stopwatch();
            runTime.Start();
            try
            {
                while (!_kill && session == null && CheckTimeThreshold(runTime.ElapsedMilliseconds))
                {
                    session = SetUpSession(Properties.Settings.Default.armaServerAddress, Properties.Settings.Default.armaServerPort);
                    System.Threading.Thread.Sleep(Properties.Settings.Default.pollRate);
                }

                while (!_kill && CheckMissionThreshold() && CheckTimeThreshold(runTime.ElapsedMilliseconds))
                {
                    session = UpdateInfo(Properties.Settings.Default.armaServerAddress, Properties.Settings.Default.armaServerPort, session);
                    System.Threading.Thread.Sleep(Properties.Settings.Default.pollRate);
                }
            }
            catch (NoServerInfoException nsie)
            {
                _logger.Error("Error reporting", nsie);
            }

        }

        public void Kill()
        {
            _kill = true;
        }

        private Session SetUpSession(string host, int port)
        {
            // initial info grab
            ServerInfo serverInfo = _serverInfoService.GetServerInfo(host, port);

            if (IsServerRunning(serverInfo.ServerState))
            {
                // create a session
                Session session = new Session();
                session.HostName = serverInfo.HostName;
                session.Version = serverInfo.GameVersion;
                session.MaxPing = serverInfo.Ping;
                session.MinPing = serverInfo.Ping;
                session.MaxPlayers = serverInfo.NumPlayers;
                session = _sessionDAO.CreateSession(session);

                return session;
            }

            return null;
        }

        private Session UpdateInfo(string host, int port, Session session)
        {
            ServerInfo serverInfo = _serverInfoService.GetServerInfo(host, port);

            if (IsServerRunning(serverInfo.ServerState))
            {
                UpdateSessionData(session, serverInfo);
                UpdatePlayerData(session, serverInfo);
                UpdateMissionData(session, serverInfo);
            }
            return session;
        }

        private void UpdateSessionData(Session session, ServerInfo serverInfo)
        {
            bool updateSession = false;

            if (session.MaxPing < serverInfo.Ping)
            {
                session.MaxPing = serverInfo.Ping;
                updateSession = true;
            }
            else if (session.MinPing > serverInfo.Ping)
            {
                session.MinPing = serverInfo.Ping;
                updateSession = true;
            }

            if (session.MaxPlayers < serverInfo.NumPlayers)
            {
                session.MaxPlayers = serverInfo.MaxPlayers;
                updateSession = true;
            }

            if (updateSession) {
                _sessionDAO.UpdateSession(session);
            }
        }

        private void UpdatePlayerData(Session session, ServerInfo serverInfo)
        {
            ISet<PlayerSession> playerSessions = _playerDAO.GetOrCreatePlayerSessions(serverInfo.Players, session);

            foreach (PlayerSession playerSession in playerSessions)
            {
                playerSession.Updated = true;
                playerSession.Length += (Properties.Settings.Default.pollRate / 1000);
                if (playerSession.Played == false && CheckPlayedThreshold(playerSession.Length))
                {
                    playerSession.Played = true;
                }
            }

            _playerDAO.UpdatePlayerSessions(playerSessions);
        }

        private void UpdateMissionData(Session session, ServerInfo serverInfo)
        {
            MissionSession missionSession = _missionDAO.GetOrCreateMissionSession(serverInfo.MapName, serverInfo.Mission, session);

            missionSession.Updated = true;
            missionSession.Length += (Properties.Settings.Default.pollRate / 1000);
            if (missionSession.Played == false && CheckPlayedThreshold(missionSession.Length))
            {
                missionSession.Played = true;
                _missionCount++;
            }

            _missionDAO.UpdateMissionSession(missionSession);
        }

        private bool IsServerRunning(int serverState)
        {
            _inGame = serverState == ServerInfoConstants.IN_GAME;
            return _inGame;
        }

        private bool CheckPlayedThreshold(int length)
        {
            return length > Properties.Settings.Default.playedThreshold;
        }

        private bool CheckTimeThreshold(long elapsedTime)
        {
            return elapsedTime < Properties.Settings.Default.runTimeThreshold;
        }

        private bool CheckMissionThreshold()
        {
            return _missionCount < Properties.Settings.Default.missionThreshold ||
                (_missionCount == Properties.Settings.Default.missionThreshold && _inGame);
        }
    }
}
