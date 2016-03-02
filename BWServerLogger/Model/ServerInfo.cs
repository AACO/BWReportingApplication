using System;
using System.Collections.Generic;
using System.Text;

using BWServerLogger.Util;

namespace BWServerLogger.Model {
    public class ServerInfo {
        private const int HEADER_OFFSET = 5;
        private const int TABLE_OFFSET = 10;
        private const byte NULL_TERMINATOR = 0;

        public ServerInfo(byte[] response) {
            int pos = HEADER_OFFSET;
            byte protocol = response[pos++]; // I have no idea what this value is for
            HostName = GetString(response, ref pos);
            MapName = GetString(response, ref pos);
            Game = GetString(response, ref pos);
            Mission = GetString(response, ref pos);
            short steamId = GetShort(response, ref pos); // shit not needed
            GetIntFromByte(response[pos++]); // provides incorrect value of number of players, ignore it
            MaxPlayers = GetIntFromByte(response[pos++]);
            int bots = GetIntFromByte(response[pos++]); // more shit not needed
            Dedicated = GetStringFromByte(response[pos++]) == ServerInfoConstants.DEDICATED;
            string os = GetStringFromByte(response[pos++]); // we have no need of this
            Password = Convert.ToBoolean(response[pos++]);
            bool vac = Convert.ToBoolean(response[pos++]); // some how more shit we don't need
            GameVersion = GetString(response, ref pos);
            byte extraDataFlag = response[pos++]; // we have no way to process this, assume it has the extra table data
            GetServerTableInfo(response, ref pos);
        }

        public void AddPlayers(byte[] response) {
            int pos = HEADER_OFFSET;
            NumPlayers = GetIntFromByte(response[pos++]);
            Players = new List<Player>(NumPlayers);
            if (NumPlayers > 0) {
                for (int i = 0; i < NumPlayers; i++) {
                    byte index = response[pos++]; // not really needed, we already have an index
                    string playerName = GetString(response, ref pos);
                    int score = GetInt(response, ref pos); // not going to be used, no need for a dick measuring contest
                    float timeOnServer = GetFloat(response, ref pos); // not going to use time on server, will track differently based on session
                    if (playerName.Trim() != "") { // prevents adding empty player
                        Players.Add(new Player(playerName));
                    }
                }
            }
        }

        private string GetString(byte[] response, ref int pos) {
            StringBuilder newString = new StringBuilder();
            while (pos < response.Length) {
                byte currentByte = response[pos];
                pos++;
                if (currentByte == NULL_TERMINATOR) {
                    break;
                }
                newString.Append(Convert.ToChar(currentByte));
            }
            return newString.ToString();
        }

        private short GetShort(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            return BitConverter.ToInt16(new byte[2] { byte1, byte2 }, 0);
        }

        private int GetIntFromByte(byte inputByte) {
            return BitConverter.ToInt16(new byte[2] { inputByte, 0 }, 0);
        }

        private string GetStringFromByte(byte inputByte) {
            return Convert.ToChar(inputByte).ToString();
        }

        private int GetInt(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            byte byte3 = response[pos++];
            byte byte4 = response[pos++];
            return BitConverter.ToInt32(new byte[4] { byte1, byte2, byte3, byte4 }, 0);
        }

        private float GetFloat(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            byte byte3 = response[pos++];
            byte byte4 = response[pos++];
            return BitConverter.ToSingle(new byte[4] { byte1, byte2, byte3, byte4 }, 0);
        }

        private void GetServerTableInfo(byte[] response, ref int pos) {
            pos = pos + TABLE_OFFSET;
            string info = GetString(response, ref pos);
            string[] infoArray = info.Split(',');
            foreach (string option in infoArray) {
                if (option != null && option.Length > 1) {
                    switch (option.Substring(0, 1)) {
                        case ServerInfoConstants.BATTLE_EYE:
                            BattleEye = option.Substring(1) == ServerInfoConstants.TRUE;
                            break;
                        case ServerInfoConstants.REQUIRED_VERSION:
                            RequiredVersion = option.Substring(1);
                            break;
                        case ServerInfoConstants.REQUIRED_BUILD_VERSION:
                            RequiredBuildVersion = option.Substring(1);
                            break;
                        case ServerInfoConstants.SERVER_STATE:
                            ServerState = Convert.ToInt16(option.Substring(1));
                            break;
                        case ServerInfoConstants.DIFFICULTY:
                            Difficulty = Convert.ToInt16(option.Substring(1));
                            break;
                        case ServerInfoConstants.EQUAL_MOD_REQUIRED:
                            EqualModRequired = option.Substring(1) == ServerInfoConstants.TRUE;
                            break;
                        case ServerInfoConstants.LOCK:
                            Locked = option.Substring(1) == ServerInfoConstants.TRUE;
                            break;
                        case ServerInfoConstants.VERIFY_SIGNATURES:
                            VerifySignatures = option.Substring(1) == ServerInfoConstants.TRUE;
                            break;
                        case ServerInfoConstants.LANGUAGE:
                            Language = option.Substring(1);
                            break;
                        case ServerInfoConstants.DEDICATED:
                            Dedicated = option.Substring(1) == ServerInfoConstants.TRUE;
                            break;
                        case ServerInfoConstants.LONG_LAT:
                            LongLat = option.Substring(1);
                            break;
                        case ServerInfoConstants.PLATFORM:
                            Platform = option.Substring(1);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public string GameVersion {
            get;
            set;
        }

        public string HostName {
            get;
            set;
        }

        public string MapName {
            get;
            set;
        }

        public string Game {
            get;
            set;
        }

        public int NumPlayers {
            get;
            set;
        }

        public int MaxPlayers {
            get;
            set;
        }

        public bool Password {
            get;
            set;
        }

        public string RequiredVersion {
            get;
            set;
        }

        public string RequiredBuildVersion {
            get;
            set;
        }

        public bool BattleEye {
            get;
            set;
        }

        public List<Player> Players {
            get;
            set;
        }

        public string Mission {
            get;
            set;
        }

        public long Ping {
            get;
            set;
        }

        public Int16 ServerState {
            get;
            set;
        }

        public Int16 Difficulty {
            get;
            set;
        }

        public bool EqualModRequired {
            get;
            set;
        }

        public bool Locked {
            get;
            set;
        }

        public bool VerifySignatures {
            get;
            set;
        }

        public string Language {
            get;
            set;
        }

        public bool Dedicated {
            get;
            set;
        }

        public string LongLat {
            get;
            set;
        }

        public string Platform {
            get;
            set;
        }

        public override int GetHashCode() {
            int hashcode = 17;

            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, GameVersion);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, HostName);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MapName);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Game);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, NumPlayers);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, MaxPlayers);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Password);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, RequiredVersion);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, RequiredBuildVersion);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, BattleEye);
            hashcode = HashUtil.SimpleCollectionHashBuilderHelper(hashcode, Players);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Mission);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Ping);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, ServerState);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Difficulty);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, EqualModRequired);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Locked);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, VerifySignatures);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Language);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Dedicated);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, LongLat);
            hashcode = HashUtil.SimpleObjectHashBuilderHelper(hashcode, Platform);

            return hashcode;
        }

        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is ServerInfo) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }
    }
}
