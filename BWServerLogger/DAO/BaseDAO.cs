using log4net;

using MySql.Data.MySqlClient;

using System;

using BWServerLogger.Exceptions;

namespace BWServerLogger.DAO {
    public abstract class BaseDAO : IDisposable {
        // protected logger, should be used on subclasses
        protected ILog _logger;

        // query to get the last inserted ID from MySQL
        private const string GET_LAST_ID_QUERY = "select last_insert_id()";

        // command to get the last inserted ID from MySQL
        private MySqlCommand _getLastInsertedId;

        public BaseDAO(MySqlConnection connection) {
            _logger = LogManager.GetLogger(GetType());
            SetupGetLastInsertedId(connection);
            SetupPreparedStatements(connection);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_getLastInsertedId != null) {
                    _getLastInsertedId.Dispose();
                }
            }
        }

        protected abstract void SetupPreparedStatements(MySqlConnection connection);

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

        private void SetupGetLastInsertedId(MySqlConnection connection) {
            _getLastInsertedId = new MySqlCommand(GET_LAST_ID_QUERY, connection);
            _getLastInsertedId.Prepare();
        }
    }
}
