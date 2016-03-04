using log4net;

using MySql.Data.MySqlClient;

using System;

using BWServerLogger.Exceptions;

namespace BWServerLogger.DAO {
    /// <summary>
    /// Abstract base class that all database access objects will extend
    /// </summary>
    public abstract class BaseDAO : IDisposable {
        /// <summary>
        /// protected logger, should be used on subclasses
        /// </summary>
        protected ILog _logger;

        // query to get the last inserted ID from MySQL
        private const string GET_LAST_ID_QUERY = "select last_insert_id()";

        // command to get the last inserted ID from MySQL
        private MySqlCommand _getLastInsertedId;

        /// <summary>
        /// Constructor to set up prepared statements, also sets up logger for subclasses
        /// </summary>
        /// <param name="connection">Open MySQL connection, used to create prepared statements</param>
        public BaseDAO(MySqlConnection connection) {
            _logger = LogManager.GetLogger(GetType());
            SetupGetLastInsertedId(connection);
            SetupPreparedStatements(connection);
        }

        /// <summary>
        /// Disposal method, should free all managed objects
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposal method, should free all managed objects, sub classes must free their commands here
        /// </summary>
        /// <param name="disposing">should the method dispose managed objects</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_getLastInsertedId != null) {
                    _getLastInsertedId.Dispose();
                }
            }
        }

        /// <summary>
        /// Abstract method to setup prepared statements for implementing classes. MUST BE IMPLEMENTED, CALLED IN CONSTRUCTOR
        /// </summary>
        /// <param name="connection">Open MySQL connection used to prepare statements</param>
        protected abstract void SetupPreparedStatements(MySqlConnection connection);

        /// <summary>
        /// Helper method to get the last inserted id of the last executed update
        /// </summary>
        /// <returns>the last inserted id of the last executed update</returns>
        protected int GetLastInsertedId() {
            MySqlDataReader lastInsertedIdResult = _getLastInsertedId.ExecuteReader();

            if (lastInsertedIdResult.HasRows) {
                lastInsertedIdResult.Read();
                int id = lastInsertedIdResult.GetInt32(0);
                lastInsertedIdResult.Close();

                return id;
            } else {
                throw new NoLastInsertedIdException("Last inserted ID query failed, aborting");
            }
        }

        /// <summary>
        /// Method to setup the last inserted ID command
        /// </summary>
        /// <param name="connection">Open MySQL connection used to prepare statements</param>
        private void SetupGetLastInsertedId(MySqlConnection connection) {
            _getLastInsertedId = new MySqlCommand(GET_LAST_ID_QUERY, connection);
            _getLastInsertedId.Prepare();
        }
    }
}
