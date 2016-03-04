using System;
using System.Collections.Generic;
using System.Text;

using BWServerLogger.Util;

namespace BWServerLogger.Model {
    /// <summary>
    /// Object to hold/parse A3 server data
    /// </summary>
    public class ServerInfo {
        /// <summary>
        /// The server game version
        /// </summary>
        public string GameVersion { get; set; }

        /// <summary>
        /// The server name
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The map name
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        /// The name of the game
        /// </summary>
        public string Game { get; set; }

        /// <summary>
        /// The number of players on the server
        /// </summary>
        public short NumPlayers { get; set; }

        /// <summary>
        /// The maximum number of players allowed on the server
        /// </summary>
        public short MaxPlayers { get; set; }

        /// <summary>
        /// Does the server have a password
        /// </summary>
        public bool Password { get; set; }

        /// <summary>
        /// Required version of the server
        /// </summary>
        public string RequiredVersion { get; set; }

        /// <summary>
        /// Required build version of the server
        /// </summary>
        public string RequiredBuildVersion { get; set; }

        /// <summary>
        /// Does the server have battle eye on enabled
        /// </summary>
        public bool BattleEye { get; set; }

        /// <summary>
        /// List of all the players currently on the server
        /// </summary>
        public IList<Player> Players { get; set; }

        /// <summary>
        /// Name of the mission
        /// </summary>
        public string Mission { get; set; }

        /// <summary>
        /// Ping of the server
        /// </summary>
        public long Ping { get; set; }

        /// <summary>
        /// State of the server (Map screen, slot screen, in game, etc.)
        /// </summary>
        /// <seealso cref="ServerInfoConstants"/>
        public short ServerState { get; set; }

        /// <summary>
        /// Difficulty setting
        /// </summary>
        public short Difficulty { get; set; }

        /// <summary>
        /// Does the server require a matching mod set?
        /// </summary>
        public bool EqualModRequired { get; set; }

        /// <summary>
        /// Is the server currently locked?
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// Does the server require matching mod signatures?
        /// </summary>
        public bool VerifySignatures { get; set; }

        /// <summary>
        /// Server language set to
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Is the server dedicated (true) or a listen server (false)
        /// </summary>
        public bool Dedicated { get; set; }

        /// <summary>
        /// Approximate logitude and latitude of the server
        /// </summary>
        public string LongLat { get; set; }

        /// <summary>
        /// Server platform (Windows, Linux, OSX (lol))
        /// </summary>
        public string Platform { get; set; }

        //constants used to help parse the byte array input
        private const int HEADER_OFFSET = 5;
        private const int TABLE_OFFSET = 10;
        private const byte NULL_TERMINATOR = 0;

        /// <summary>
        /// Creates a server info object, and parses the provided byte array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        public ServerInfo(byte[] response) {
            int pos = HEADER_OFFSET;
            byte protocol = response[pos++]; // I have no idea what this value is for
            HostName = GetString(response, ref pos);
            MapName = GetString(response, ref pos);
            Game = GetString(response, ref pos);
            Mission = GetString(response, ref pos);
            short steamId = GetShort(response, ref pos); // shit not needed
            GetShortFromByte(response[pos++]); // provides incorrect value of number of players, ignore it
            MaxPlayers = GetShortFromByte(response[pos++]);
            short bots = GetShortFromByte(response[pos++]); // more shit not needed
            Dedicated = GetStringFromByte(response[pos++]) == ServerInfoConstants.DEDICATED;
            string os = GetStringFromByte(response[pos++]); // we have no need of this
            Password = Convert.ToBoolean(response[pos++]);
            bool vac = Convert.ToBoolean(response[pos++]); // some how more shit we don't need
            GameVersion = GetString(response, ref pos);
            byte extraDataFlag = response[pos++]; // we have no way to process this, assume it has the extra table data
            GetServerTableInfo(response, ref pos);
        }

        /// <summary>
        /// Parses a byte array, and gets the players from it
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        public void AddPlayers(byte[] response) {
            int pos = HEADER_OFFSET;
            NumPlayers = GetShortFromByte(response[pos++]);
            Players = new List<Player>(NumPlayers);
            if (NumPlayers > 0) {
                for (short i = 0; i < NumPlayers; i++) {
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

        /// <summary>
        /// Helper method to get a null terminated string from a byte array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        /// <param name="pos">"cursor" position</param>
        /// <returns>a null terminated string</returns>
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

        /// <summary>
        /// Helper method to get a short from a provided byte array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        /// <param name="pos">"cursor" position</param>
        /// <returns>short converted from a byte[]</returns>
        private short GetShort(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            return BitConverter.ToInt16(new byte[2] { byte1, byte2 }, 0);
        }

        /// <summary>
        /// Helper method to get an int from a provided byte array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        /// <param name="pos">"cursor" position</param>
        /// <returns>int converted from a byte[]</returns>
        private int GetInt(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            byte byte3 = response[pos++];
            byte byte4 = response[pos++];
            return BitConverter.ToInt32(new byte[4] { byte1, byte2, byte3, byte4 }, 0);
        }

        /// <summary>
        /// Helper method to get a float from a provided byte array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        /// <param name="pos">"cursor" position</param>
        /// <returns>float converted from a byte[]</returns>
        private float GetFloat(byte[] response, ref int pos) {
            byte byte1 = response[pos++];
            byte byte2 = response[pos++];
            byte byte3 = response[pos++];
            byte byte4 = response[pos++];
            return BitConverter.ToSingle(new byte[4] { byte1, byte2, byte3, byte4 }, 0);
        }

        /// <summary>
        /// Helper method to get a short from a provided byte
        /// </summary>
        /// <param name="inputByte">Byte to turn into a short</param>
        /// <returns>short converted from a byte[]</returns>
        private short GetShortFromByte(byte inputByte) {
            return BitConverter.ToInt16(new byte[2] { inputByte, 0 }, 0);
        }

        /// <summary>
        /// Get a string from a provided byte
        /// </summary>
        /// <param name="inputByte">Byte to turn into a string</param>
        /// <returns>string from the provided byt</returns>
        private string GetStringFromByte(byte inputByte) {
            return Convert.ToChar(inputByte).ToString();
        }

        /// <summary>
        /// Gets values from an "extra info" btype array
        /// </summary>
        /// <param name="response">Server info query byte array to parse</param>
        /// <param name="pos">"cursor" position</param>
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

        /// <summary>
        /// Overrides the default hash code
        /// </summary>
        /// <returns>Unique int value for an object</returns>
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

        /// <summary>
        /// Overrides the default equals method.
        /// </summary>
        /// <param name="obj">Object to check for equality</param>
        /// <returns>True if the objects are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            bool equals = false;

            if (obj is ServerInfo) {
                equals = GetHashCode() == obj.GetHashCode();
            }

            return equals;
        }
    }
}
