using log4net;

using MySql.Data;
using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

using BWServerLogger.Exceptions;
using BWServerLogger.Model;
using BWServerLogger.Util;

namespace BWServerLogger.DAO
{
    public abstract class BaseDAO
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(BaseDAO));

        private const string GET_LAST_ID_QUERY = "select last_insert_id()";

        private MySqlCommand _getLastInsertedId;


        public BaseDAO(MySqlConnection connection)
        {
            VerifySchema(connection);
            SetupGetLastInsertedId(connection);
            SetupPreparedStatements(connection);
        }

        protected abstract ISet<Column> GetColumns();
        protected abstract String GetTable();
        protected abstract void SetupPreparedStatements(MySqlConnection connection);

        protected Int32 GetLastInsertedId()
        {
            MySqlDataReader lastInsertedIdResult = _getLastInsertedId.ExecuteReader();

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

        private void SetupGetLastInsertedId(MySqlConnection connection)
        {
            _getLastInsertedId = new MySqlCommand(GET_LAST_ID_QUERY, connection);
            _getLastInsertedId.Prepare();
        }

        private void VerifySchema(MySqlConnection connection)
        {
            try
            {
                // check to see if table exists
                StringBuilder existsQuery = new StringBuilder();
                existsQuery.Append("select 1 from ");
                existsQuery.Append(GetTable());
                existsQuery.Append(" limit 1");

                MySqlCommand existsCommand = new MySqlCommand(existsQuery.ToString(), connection);
                existsCommand.ExecuteReader();

                // table is ensured to exsist here, check schema

                StringBuilder getSchemaQuery = new StringBuilder();
                getSchemaQuery.Append("describe ");
                getSchemaQuery.Append(GetTable());

                MySqlCommand getSchema = new MySqlCommand(getSchemaQuery.ToString(), connection);
                MySqlDataReader reader = getSchema.ExecuteReader();

                if (reader.HasRows)
                {
                    ISet<Column> schemaColumns = new HashSet<Column>();
                    while(reader.Read())
                    {
                        Column col = new Column(reader.GetString(0),
                                                reader.GetString(1),
                                                reader.GetString(2),
                                                reader.GetString(3),
                                                reader.GetString(4),
                                                reader.GetString(5));
                        schemaColumns.Add(col);
                    }

                    if (!schemaColumns.SetEquals(GetColumns()))
                    {
                        _logger.ErrorFormat("Schema for table: {0} does not match the code base.", GetTable());
                        throw new SchemaMismatchException(); // we want blowups on invalid schema
                    }
                }
                else
                {
                    _logger.ErrorFormat("No schema found for table: {0}", GetTable());
                    throw new NoSchemaException("No schema found"); // we want blowups on invalid schema
                }

            }
            catch (MySqlException e)
            {
                // checks to see if the table exists, if it doesn't try to create it.
                if (MySqlErrorCode.NoSuchTable.Equals(e.ErrorCode))
                {
                    //create table
                }
                else
                {
                    _logger.ErrorFormat("Error validating database schema for table: {}", GetTable());
                    throw e; // we want blowups on invalid schema
                }
            }
        }
    }
}
