using log4net;

using MySql.Data;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using BWServerLogger.Exceptions;

namespace BWServerLogger.Util
{
    class DatabaseUtil
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(DatabaseUtil));

        public const string NAME_KEY = "@name";
        public const string PLAYER_ID_KEY = "@playerId";
        public const string SESSION_ID_KEY = "@sessionId";
        public const string MAX_PLAYERS_KEY = "@maxPlayers";
        public const string MAX_PING_KEY = "@maxPing";
        public const string MIN_PING_KEY = "@minPing";
        public const string PLAYER_TO_SESSION_ID_KEY = "@playerToSessionId";
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

        public const int DEFAULT_PLAYER_COUNT = 50;

        public const string DATE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string TIME_FORMAT = "HH:mm:ss";

        public const string GET_LAST_ID_QUERY = "select last_insert_id()";

        public static MySqlConnection OpenDataSource()
        {
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
            return connection;
        }

        public static Int32 GetLastInsertedId(ref MySqlConnection connection)
        {
            string getLastInsertedIdSelect = GET_LAST_ID_QUERY;

            MySqlCommand getLastInsertedId = new MySqlCommand(getLastInsertedIdSelect, connection);

            MySqlDataReader lastInsertedIdResult = getLastInsertedId.ExecuteReader();

            if (lastInsertedIdResult.HasRows)
            {
                lastInsertedIdResult.Read();
                Int32 id = lastInsertedIdResult.GetInt32(0);
                lastInsertedIdResult.Close();

                return id;
            }
            else
            {
                throw new NoLastInsertedIdException("Last inserted ID query failed, aborting");
            }
        }

        public static string GetMySQLPassword()
        {
            // Declare the string used to hold the decrypted text. 
            string plainText = "";
            try
            {
                byte[] encrypted = Properties.Settings.Default.mySQLServerPassword;
                byte[] key = Properties.Settings.Default.key;
                byte[] iv = Properties.Settings.Default.iv;

                // Create an AesManaged object with the specified key and IV.
                using (AesManaged aes = new AesManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    // Create the streams used for decryption. 
                    using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and place them in a string.
                                plainText = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                }
                
            }
            catch (Exception e)
            {
                logger.Error("Error decrypting a value, returning empty string", e);
            }
            return plainText;
        }

        public static void SetMySQLPassword(string password)
        {
            try
            {
                byte[] encrypted;
                using (AesManaged aes = new AesManaged())
                {
                    // Create a encrytor to perform the stream transform.
                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    // Create the streams used for encryption. 
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                //Write all data to the stream.
                                swEncrypt.Write(password);
                            }
                            encrypted = msEncrypt.ToArray();
                        }
                    }

                    if (encrypted == null)
                    {
                        throw new ArgumentNullException("The text did not get encrypted");
                    }

                    Properties.Settings.Default.mySQLServerPassword = encrypted;
                    Properties.Settings.Default.key = aes.Key;
                    Properties.Settings.Default.iv = aes.IV;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error encrypting a value", e);
            }
        }
    }
}
