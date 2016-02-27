using log4net;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Text;

using BWServerLogger.Exceptions;
using BWServerLogger.Model;

namespace BWServerLogger.DAO {
    public abstract class BaseDAO : IDisposable {
        protected ILog _logger;

        private const string GET_LAST_ID_QUERY = "select last_insert_id()";

        private MySqlCommand _getLastInsertedId;

        public BaseDAO(MySqlConnection connection) {
            _logger = LogManager.GetLogger(GetType());
            VerifySchema(connection);
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

        protected abstract IDictionary<string, ISet<Column>> GetRequiredSchema();
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

        private void VerifySchema(MySqlConnection connection) {
            foreach (KeyValuePair<String, ISet<Column>> kvp in GetRequiredSchema()) {
                CheckTable(connection, kvp, 0);
                CheckColumns(connection, kvp);
            }
        }

        private void CheckTable(MySqlConnection connection, KeyValuePair<String, ISet<Column>> kvp, int retryAttempts) {
            if (retryAttempts > 2) //ensure no insane infinite loop case
            {
                throw new NoRetriesLeftException("Ran out of retries validating stabase schema");
            }

            try {
                // check to see if table exists
                StringBuilder existsQuery = new StringBuilder();
                existsQuery.Append("select 1 from ");
                existsQuery.Append(kvp.Key);
                existsQuery.Append(" limit 1");

                MySqlCommand existsCommand = new MySqlCommand(existsQuery.ToString(), connection);
                existsCommand.ExecuteReader().Close();
            } catch (MySqlException e) {
                // checks to see if the table exists, if it doesn't try to create it.
                if ((int)MySqlErrorCode.NoSuchTable == e.Number) {
                    StringBuilder createTableQuery = new StringBuilder();
                    createTableQuery.Append("create table `");
                    createTableQuery.Append(kvp.Key);
                    createTableQuery.Append("` (");

                    bool first = true;
                    foreach (Column col in kvp.Value) {
                        if (first) {
                            first = false;
                        } else {
                            createTableQuery.Append(", ");
                        }
                        createTableQuery.Append("`");
                        createTableQuery.Append(col.Field);
                        createTableQuery.Append("` ");
                        createTableQuery.Append(col.Type);
                        createTableQuery.Append(" ");
                        createTableQuery.Append(col.Null ? "" : "NOT NULL");
                        createTableQuery.Append(" ");
                        if (col.Default != null) {
                            createTableQuery.Append("DEFAULT ");
                            createTableQuery.Append(col.Default);
                            createTableQuery.Append(" ");
                        }
                        createTableQuery.Append(col.AutoIncrement ? "AUTO_INCREMENT" : "");
                    }

                    foreach (Column col in kvp.Value) {
                        if (col.Key != IndexType.NONE) {
                            createTableQuery.Append(", ");
                            switch (col.Key) {
                                case IndexType.PRIMARY:
                                createTableQuery.Append("PRIMARY ");
                                break;
                                case IndexType.UNIQUE:
                                createTableQuery.Append("UNIQUE ");
                                break;
                            }
                            createTableQuery.Append("KEY (`");
                            createTableQuery.Append(col.Field);
                            createTableQuery.Append("`)");
                        }
                    }
                    createTableQuery.Append(") ENGINE=InnoDB DEFAULT CHARSET=utf8");

                    MySqlCommand createTable = new MySqlCommand(createTableQuery.ToString(), connection);

                    try {
                        createTable.ExecuteNonQuery();
                    } catch (MySqlException mse) {
                        if ((int)MySqlErrorCode.TableExists == mse.Number) {
                            _logger.Info("Could not create table since it already exists, probably race condition from different thread, revalidating");
                            CheckTable(connection, kvp, retryAttempts++);
                            return;
                        } else {
                            _logger.ErrorFormat("Error validating database schema", mse);
                            throw; // we want blowups on invalid schema
                        }
                    }
                } else {
                    _logger.ErrorFormat("Error validating database schema", e);
                    throw; // we want blowups on invalid schema
                }
            }
        }

        private void CheckColumns(MySqlConnection connection, KeyValuePair<string, ISet<Column>> kvp) {
            // table is ensured to exsist here, check schema
            StringBuilder getSchemaQuery = new StringBuilder();
            getSchemaQuery.Append("describe ");
            getSchemaQuery.Append(kvp.Key);

            MySqlCommand getSchema = new MySqlCommand(getSchemaQuery.ToString(), connection);
            MySqlDataReader reader = getSchema.ExecuteReader();

            if (reader.HasRows) {
                ISet<Column> schemaColumns = new HashSet<Column>();
                while (reader.Read()) {
                    Column col = new Column(reader.GetString(0),
                                            reader.GetString(1),
                                            reader.GetString(2),
                                            reader.GetString(3),
                                            (reader.IsDBNull(4)) ? null : reader.GetString(4),
                                            reader.GetString(5));
                    schemaColumns.Add(col);
                }

                foreach (Column col in kvp.Value) {
                    if (!schemaColumns.Contains(col)) {
                        _logger.ErrorFormat("Schema for table: {0} does not match the code base. Missing/mismatched column: {1}", kvp.Key, col.Field);
                        throw new SchemaMismatchException(); // we want blowups on invalid schema
                    }
                }
            } else {
                _logger.ErrorFormat("No schema found for table: {0}", kvp.Key);
                throw new NoSchemaException("No schema found"); // we want blowups on invalid schema
            }
            reader.Close();
        }
    }
}
