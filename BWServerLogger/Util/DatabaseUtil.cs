using BWServerLogger.Exceptions;
using BWServerLogger.Model;

using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace BWServerLogger.Util {
    /// <summary>
    /// Util to help with database interactions
    /// </summary>
    class DatabaseUtil {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(DatabaseUtil));

        // database table constants
        public const string PLAYER = "player";
        public const string PLAYER_TO_MISSION_TO_SESSION = "player_to_mission_to_session";
        public const string MAP = "map";
        public const string MISSION = "mission";
        public const string MISSION_TO_SESSION = "mission_to_session";
        public const string SESSION = "session";
        public const string SCHEDULE = "schedule";
        public const string FRAMEWORK = "framework";

        // database parameter constants
        public const string NAME_KEY = "@name";
        public const string PLAYER_ID_KEY = "@playerId";
        public const string SESSION_ID_KEY = "@sessionId";
        public const string MAX_PLAYERS_KEY = "@maxPlayers";
        public const string MAX_PING_KEY = "@maxPing";
        public const string MIN_PING_KEY = "@minPing";
        public const string LENGTH_KEY = "@length";
        public const string PLAYED_KEY = "@played";
        public const string HAS_CLAN_TAG_KEY = "@hasClanTag";
        public const string MISSION_ID_KEY = "@missionId";
        public const string MISSION_TO_SESSION_ID_KEY = "@missionToSessionId";
        public const string MAP_ID_KEY = "@mapId";
        public const string FRIENDLY_NAME_KEY = "@friendlyName";
        public const string DAY_OF_THE_WEEK_KEY = "@dayOfTheWeek";
        public const string TIME_OF_DAY_KEY = "@timeOfDay";
        public const string DATE_KEY = "@date";
        public const string VERSION_KEY = "@version";
        public const string HOST_NAME_KEY = "@hostName";
        public const string SCHEDULE_ID_KEY = "@scheduleId";
        public const string PLAYER_TO_MISSION_TO_SESSION_ID_KEY = "@ptmtsId";

        // default target player count for a new mission
        public const int DEFAULT_PLAYER_COUNT = 50;

        // formatting constants
        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string TIME_FORMAT = "HH:mm:ss";

        // regex magic to scrape mysql create table output, need '@' to avoid escaping these monsters
        private const string COLUMN_MATCHER_REGEX = @"`(.+)` ([A-Za-z0-9]+(?:\([A-Za-z0-9,' ]+\))?(?: unsigned)?)( NOT NULL)?(?: DEFAULT ([A-Za-z0-9' ]+))?( AUTO_INCREMENT)?,";
        private const string INDEX_MATCHER_REGEX = @"((?:PRIMARY)|(?:UNIQUE))? ?KEY ?(?:`([A-Za-z0-9_]+)` )?\(((?:`(?:[A-Za-z0-9_]+)`,?)+)\)(?! REFERENCES)";
        private const string FK_MATCHER_REGEX = @"CONSTRAINT `([A-Za-z0-9_]+)` FOREIGN KEY \(((?:`(?:[A-Za-z_]+)`,?)+)\) REFERENCES `([A-Za-z0-9_]+)` \(((?:`(?:[A-Za-z_]+)`,?)+)\)";

        /// <summary>
        /// Opens a MySQL data connection.
        /// Validates the schema integrity.
        /// </summary>
        /// <returns>An open MySQL data connection</returns>
        public static MySqlConnection OpenDataSource() {
            return OpenDataSource(true);
        }

        /// <summary>
        /// Opens a MySQL data connection.
        /// </summary>
        /// <param name="validate">If true, will validate the schema integrity</param>
        /// <returns>An open MySQL data connection</returns>
        public static MySqlConnection OpenDataSource(bool validate) {
            _logger.InfoFormat("Trying to open a new datasource with validation set to {0}.", validate);

            // build connection string (using "AppSettings" instead of "ConnectionStrings" to allow easier password en/decryption)
            StringBuilder connectionString = new StringBuilder();
            connectionString.Append("server=");
            connectionString.Append(Properties.Settings.Default.mySQLServerAddress);
            connectionString.Append(";");
            connectionString.Append("port=");
            connectionString.Append(Properties.Settings.Default.mySQLServerPort);
            connectionString.Append(";");
            connectionString.Append("database=");
            connectionString.Append(Properties.Settings.Default.mySQLServerDatabase);
            connectionString.Append(";");
            connectionString.Append("uid=");
            connectionString.Append(Properties.Settings.Default.mySQLServerUsername);
            connectionString.Append(";");
            connectionString.Append("pwd=");
            connectionString.Append(GetMySQLPassword());
            connectionString.Append(";");

            MySqlConnection connection = new MySqlConnection(connectionString.ToString());
            connection.Open();

            // ensure DB is in order for the new connection. Adds runtime, but ensures integrity
            if (validate) {
                _logger.Info("Trying to validate the datasource schema.");
                ValidateDatabase(connection);
                _logger.Info("Datasource schema validated.");
            }

            return connection;
        }

        /// <summary>
        /// Validates the database schema for the given connection
        /// </summary>
        /// <param name="connection">connection to validate</param>
        private static void ValidateDatabase(MySqlConnection connection) {
            foreach (Table table in _tables) {
                _logger.DebugFormat("Trying to validate table: {0}", table.Name);
                CheckTable(connection, table, 0);
            }
        }

        /// <summary>
        /// Validates a table
        /// </summary>
        /// <param name="connection">Connection to validate</param>
        /// <param name="table">Table to validate</param>
        /// <param name="retryAttempts">Number of retries to try and validate</param>
        private static void CheckTable(MySqlConnection connection, Table table, int retryAttempts) {
            if (retryAttempts > 2) { //ensure no insane infinite loop case
                throw new NoRetriesLeftException("Ran out of retries validating stabase schema");
            }

            try {
                // check to see if table exists
                StringBuilder showTableQuery = new StringBuilder();
                showTableQuery.Append("show create table ");
                showTableQuery.Append(table.Name);

                MySqlCommand existsCommand = new MySqlCommand(showTableQuery.ToString(), connection);
                MySqlDataReader reader = existsCommand.ExecuteReader();
                if (reader.HasRows) {
                    reader.Read();
                    string showTableResult = reader.GetString(1);
                    CheckColumns(showTableResult, table.Columns, table.Name);
                    CheckIndices(showTableResult, table.Indices, table.Name);
                }
                reader.Close();
            } catch (MySqlException e) {
                // checks to see if the table exists, if it doesn't try to create it.
                if ((int)MySqlErrorCode.NoSuchTable == e.Number) {
                    string createTableSql = table.CreateTableSQL();
                    _logger.DebugFormat("Creating table: {0} with query: \r {1}", table.Name, createTableSql);
                    MySqlCommand createTable = new MySqlCommand(createTableSql, connection);

                    try {
                        createTable.ExecuteNonQuery();
                    } catch (MySqlException mse) {
                        if ((int)MySqlErrorCode.TableExists == mse.Number) {
                            _logger.Info("Could not create table since it already exists, probably race condition from different thread, revalidating");
                            CheckTable(connection, table, retryAttempts++);
                            return;
                        } else {
                            _logger.Error("Error validating database schema", mse);
                            throw; // we want blowups on invalid schema
                        }
                    }
                } else {
                    _logger.Error("Error validating database schema", e);
                    throw; // we want blowups on invalid schema
                }
            }
        }

        /// <summary>
        /// Validates columns from a table
        /// </summary>
        /// <param name="sqlToScrape">SQL create table string to check for column data</param>
        /// <param name="columns">Projected schema columns</param>
        /// <param name="tableName">Name of table to check</param>
        private static void CheckColumns(string sqlToScrape, ISet<Column> columns, string tableName) {
            // create regex to scrape SQL result
            Regex columnRegex = new Regex(COLUMN_MATCHER_REGEX);
            MatchCollection matches = columnRegex.Matches(sqlToScrape);
            ISet<Column> schemaColumns = new HashSet<Column>();

            foreach (Match match in matches) {
                GroupCollection groups = match.Groups;
                schemaColumns.Add(new Column(groups[1].Value,
                                             groups[2].Value,
                                             groups[3].Value,
                                             groups[4].Value,
                                             groups[5].Value));
            }

            foreach (Column col in columns) {
                if (!schemaColumns.Contains(col)) {
                    _logger.ErrorFormat("Schema for table: {0} does not match the code base. Missing/mismatched column: {1}", tableName, col.Field);
                    throw new SchemaMismatchException(); // we want blowups on invalid schema
                }
            }
        }

        /// <summary>
        /// Validates indices from a table
        /// </summary>
        /// <param name="sqlToScrape">SQL create table string to check for column data</param>
        /// <param name="indices">Projected schema indices</param>
        /// <param name="tableName">Name of table to check</param>
        private static void CheckIndices(string sqlToScrape, IList<Index> indices, string tableName) {
            // create regex to scrape SQL result
            Regex indexRegex = new Regex(INDEX_MATCHER_REGEX);
            MatchCollection matches = indexRegex.Matches(sqlToScrape);
            IList<Index> schemaIndices = new List<Index>(matches.Count);

            foreach (Match match in matches) {
                GroupCollection groups = match.Groups;
                schemaIndices.Add(new Index(groups[1].Value,
                                            groups[2].Value,
                                            groups[3].Value));
            }

            Regex fkRegex = new Regex(FK_MATCHER_REGEX);
            matches = fkRegex.Matches(sqlToScrape);

            foreach (Match match in matches) {
                GroupCollection groups = match.Groups;
                schemaIndices.Add(new Index(groups[1].Value,
                                            groups[2].Value,
                                            groups[3].Value,
                                            groups[4].Value));
            }

            foreach (Index index in indices) {
                if (!schemaIndices.Contains(index)) {
                    _logger.ErrorFormat("Schema for table: {0} does not match the code base. Missing/mismatched index: {1}", tableName, index.Name == null ? "PRIMARY" : index.Name);
                    throw new SchemaMismatchException(); // we want blowups on invalid schema
                }
            }
        }

        /// <summary>
        /// Decrypts the MySQL connection password
        /// </summary>
        /// <returns>Decrypted MySQL password</returns>
        public static string GetMySQLPassword() {
            // Declare the string used to hold the decrypted text. 
            string plainText = "";
            try {
                byte[] encrypted = Properties.Settings.Default.mySQLServerPassword;
                byte[] key = Properties.Settings.Default.key;
                byte[] iv = Properties.Settings.Default.iv;

                // Create an AesManaged object with the specified key and IV.
                using (AesManaged aes = new AesManaged()) {
                    aes.Key = key;
                    aes.IV = iv;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // Create the streams used for decryption. 
                    using (MemoryStream msDecrypt = new MemoryStream(encrypted)) {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                                // Read the decrypted bytes from the decrypting stream and place them in a string.
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                }

            } catch (Exception e) {
                _logger.Error("Error decrypting a value, returning empty string", e);
            }
            return plainText;
        }

        /// <summary>
        /// Encrypts and stores the MySQL connection password
        /// </summary>
        /// <param name="password">Password to encrypt and store</param>
        public static void SetMySQLPassword(string password) {
            try {
                byte[] encrypted;
                using (AesManaged aes = new AesManaged()) {
                    // Create a encrytor to perform the stream transform.
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // Create the streams used for encryption. 
                    using (MemoryStream msEncrypt = new MemoryStream()) {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                                //Write all data to the stream.
                                swEncrypt.Write(password);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }

                    if (encrypted == null) {
                        throw new ArgumentNullException("The text did not get encrypted");
                    }

                    Properties.Settings.Default.mySQLServerPassword = encrypted;
                    Properties.Settings.Default.key = aes.Key;
                    Properties.Settings.Default.iv = aes.IV;
                    Properties.Settings.Default.Save();
                }
            } catch (Exception e) {
                _logger.Error("Error encrypting a value", e);
            }
        }

        // map columns
        private readonly static ISet<Column> _mapColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("friendly_name", "varchar(150)", false, "", false) },
            { new Column("name", "varchar(150)", false, "", false) },
            { new Column("active", "bit(1)", false, "b'1'", false) }
        };

        // map indices
        private readonly static IList<Index> _mapIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "name", "name") },
            { new Index(IndexType.UNIQUE, "friendly_name", "friendly_name") }
        };

        // player columns
        private readonly static ISet<Column> _playerColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("name", "varchar(255)", false, "", false) },
            { new Column("has_clan_tag", "bit(1)", false, "b'0'", false) }
        };

        // player indices
        private readonly static IList<Index> _playerIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "name", "name") }
        };

        // session columns
        private readonly static ISet<Column> _sessionColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("date", "datetime", false, "", false) },
            { new Column("host_name", "varchar(255)", false, "", false) },
            { new Column("max_players", "int(10) unsigned", false, "", false) },
            { new Column("version", "varchar(50)", false, "", false) },
            { new Column("min_ping", "int(10) unsigned", false, "", false) },
            { new Column("max_ping", "int(10) unsigned", false, "", false) }
        };

        // session indices
        private readonly static IList<Index> _sessionIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "date", "date") }
        };

        // schedule columns
        private readonly static ISet<Column> _scheduleColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("day_of_the_week", "enum('SUNDAY','MONDAY','TUESDAY','WEDNESDAY','THURSDAY','FRIDAY','SATURDAY')", false, "", false) },
            { new Column("time_of_day", "time", false, "", false) }
        };

        // schedule indices
        private readonly static IList<Index> _scheduleIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "unique_schedule_constraint", "day_of_the_week,time_of_day") }
        };

        // framework columns
        private readonly static ISet<Column> _frameworkColumns = new HashSet<Column> {
            { new Column("id", "int(10)", false, "", true) },
            { new Column("version", "varchar(10)", false, "", false) },
            { new Column("url", "varchar(255)", false, "", false) }
        };

        // framework indices
        private readonly static IList<Index> _frameworkIndices = new List<Index> {
            { new Index("id") }
        };

        // mission columns
        private readonly static ISet<Column> _missionColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("name", "varchar(255)", false, "", false) },
            { new Column("map_id", "int(10) unsigned", false, "", false) },
            { new Column("created_on", "datetime", false, "", false) },
            { new Column("updated_on", "datetime", false, "", false) },
            { new Column("description", "text", false, "", false) },
            { new Column("mode", "enum('Adversarial','COOP','Zeus','After Hours')", false, "'Adversarial'", false) },
            { new Column("target_player_count", "int(10) unsigned", false, "'0'", false) },
            { new Column("framework_id", "int(10)", true, "NULL", false) },
            { new Column("tested", "bit(1)", false, "b'0'", false) },
            { new Column("replayable", "bit(1)", false, "b'0'", false) }
        };

        // mission indices
        private readonly static IList<Index> _missionIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "name", "name") },
            { new Index(IndexType.NONUNIQUE, "map", "map_id") },
            { new Index(IndexType.NONUNIQUE, "framework_id", "framework_id") },
            { new Index("mission_fw_id_to_fw_id", "framework_id", "framework", "id") },
            { new Index("mission_key_map_id_to_map", "map_id", "map", "id") }
        };

        // mission to session columns
        private readonly static ISet<Column> _mtsColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("mission_id", "int(10) unsigned", false, "", false) },
            { new Column("session_id", "int(10) unsigned", false, "", false) },
            { new Column("length", "int(10) unsigned", false, "'0'", false) },
            { new Column("played", "bit(1)", false, "b'0'", false) }
        };

        // mission to session indices
        private readonly static IList<Index> _mtsIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "mission_id_session_id", "mission_id,session_id") },
            { new Index(IndexType.NONUNIQUE, "mts_key_session_id_to_session", "session_id") },
            { new Index(IndexType.NONUNIQUE, "mts_key_mission_id_to_mission", "mission_id") },
            { new Index("mts_key_mission_id_to_mission", "mission_id", "mission", "id") },
            { new Index("mts_key_session_id_to_session", "session_id", "session", "id") }
        };

        // mission to session columns
        private readonly static ISet<Column> _ptmtsColumns = new HashSet<Column> {
            { new Column("id", "int(10) unsigned", false, "", true) },
            { new Column("player_id", "int(10) unsigned", false, "", false) },
            { new Column("mission_to_session_id", "int(10) unsigned", false, "", false) },
            { new Column("length", "int(10) unsigned", false, "'0'", false) },
            { new Column("played", "bit(1)", false, "b'0'", false) }
        };

        // mission to session indices
        private readonly static IList<Index> _ptmtsIndices = new List<Index> {
            { new Index("id") },
            { new Index(IndexType.UNIQUE, "player_to_mission_to_session_idx", "mission_to_session_id,player_id") },
            { new Index(IndexType.NONUNIQUE, "player_id", "player_id") },
            { new Index(IndexType.NONUNIQUE, "mission_to_session_id", "mission_to_session_id") },
            { new Index("ptmts_to_mts", "mission_to_session_id", "mission_to_session", "id") },
            { new Index("ptmts_to_player", "player_id", "player", "id") }
        };

        // ORDER MATTERS!
        private readonly static IList<Table> _tables = new List<Table> {
            // adding map table
            { new Table(MAP, _mapColumns, _mapIndices) },

            // adding player table
            { new Table(PLAYER, _playerColumns, _playerIndices) },

            // adding session table
            { new Table(SESSION, _sessionColumns, _sessionIndices) },

            // adding schedule table
            { new Table(SCHEDULE, _scheduleColumns, _scheduleIndices) },

            // adding framework table
            { new Table(FRAMEWORK, _frameworkColumns, _frameworkIndices) },

            // adding mission table (relies on map and framework)
            { new Table(MISSION, _missionColumns, _missionIndices) },

            // addding mission to session table (relies on mission and session)
            { new Table(MISSION_TO_SESSION, _mtsColumns, _mtsIndices) },

            // adding player to mission to session table (relies on pts and mts)
            { new Table(PLAYER_TO_MISSION_TO_SESSION, _ptmtsColumns, _ptmtsIndices) }
        };
    }
}
