using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using Hermes.Serialization;

namespace Hermes.Monitoring.DataAcess
{
    public class PagedResult<T>
    {
        public int RowCount { get; set; }
        public int Amount { get; set; }
        public int PageNumber { get; set; }
        public T Result { get; set; }
    }

    public class HermesMessage
    {
        public Guid Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string ReplyTo { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Body { get; set; }
        public int RowVersion { get; set; }
    }

    public class TableSchema
    {
        public TableSchema()
        {
            TableNames = new List<string>();
        }

        public string Name { get; set; }
        public IList<string> TableNames { get; set; } 
    }
    
    public class BrokerDataAccess
    {
        private readonly ISerializeObjects serializer;
        private const int MaxPageNumber = 10;

        private readonly string connectionString;

        public BrokerDataAccess(ISerializeObjects serializer)
        {
            this.serializer = serializer;
            this.connectionString = ConfigurationManager.ConnectionStrings["SqlTransport"].ConnectionString;
        }

        public PagedResult<IList<HermesMessage>> GetHermesMessages(string tableName, int pageNumber)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand numberOfMessagesCommnad = sqlConnection.CreateCommand();
                numberOfMessagesCommnad.CommandText = "select COUNT(*) from @TableName";
                numberOfMessagesCommnad.Parameters.AddWithValue("@TableName", tableName);

                var numberOfMessage = Convert.ToInt32(numberOfMessagesCommnad.ExecuteScalar());


                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand = PagedSqlQuery(sqlCommand, 
                    "[Id], [CorrelationId], [ReplyTo], [Expires], [Headers], [Body], [RowVersion]",
                    tableName,
                    pageNumber);

                var reader = sqlCommand.ExecuteReader();

                var result = new PagedResult<IList<HermesMessage>>();
                result.RowCount = numberOfMessage;
                result.PageNumber = pageNumber;
                result.Amount = MaxPageNumber;
               
                result.Result = new List<HermesMessage>();
                
                while (reader.Read())
                {
                    var bodyBytes = (byte[])reader["Body"];
                    string header = reader.GetString(4);
                    
                    var messageHeader = serializer.DeserializeObject<Dictionary<string, string>>(header);
                    var body = Encoding.UTF8.GetString(bodyBytes);

                    var hermesMessage = new HermesMessage
                        {
                            Id = reader.GetGuid(0),
                            CorrelationId = reader.GetGuid(1),
                            ReplyTo = reader.GetString(2),
                            Body = body,
                            Headers = messageHeader
                        };
                    result.Result.Add(hermesMessage);
                }
                return result;
            }
        }

        private SqlCommand PagedSqlQuery(SqlCommand sqlCommand, 
            string selectColumnNames, 
            string tableName, 
            int pageNumber)
        {
            var fromRow = pageNumber*MaxPageNumber;
            var toRow = fromRow + MaxPageNumber;

            sqlCommand.Parameters.AddWithValue("@SelectColumnNames", selectColumnNames);
            sqlCommand.Parameters.AddWithValue("@FromRow", fromRow);
            sqlCommand.Parameters.AddWithValue("@ToRow", toRow);
            sqlCommand.Parameters.AddWithValue("@TableName", tableName);
            sqlCommand.CommandText = GetHermesQueueQuery();

            return sqlCommand;
        }

        private string GetHermesQueueQuery()
        {
            return "SELECT @SelectedColumnNames FROM " +
                   "( SELECT ROW_NUMBER() OVER ( ORDER BY [RowVersion] ) AS RowNum," +
                   " * FROM @TableName) " +
                   "AS RowConstrainedResult WHERE " +
                   "RowNum >= @FromRow AND RowNum < @ToRow " +
                   "ORDER BY RowNum";
        }

        public Dictionary<string, TableSchema> GetTableSchemas()
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "select TABLE_SCHEMA, TABLE_NAME from INFORMATION_SCHEMA.TABLES";

                var reader = sqlCommand.ExecuteReader();

                var tableSchemas = new Dictionary<string, TableSchema>();

                while (reader.Read())
                {
                    var tableSchemaName = reader.GetString(0);

                     if (tableSchemas.ContainsKey(tableSchemaName))
                     {
                         tableSchemas[tableSchemaName].TableNames.Add(reader.GetString(1));
                     }
                     else
                     {
                         var tableSchema = new TableSchema
                             {
                                 Name = tableSchemaName,
                                 TableNames = new List<string>
                                     {
                                         reader.GetString(1)
                                     }
                             };
                         tableSchemas.Add(tableSchemaName, tableSchema);
                     }
                }
                return tableSchemas;
            }

        }
        
        public List<T> SqlQuery<T>(string sqlQuery, params SqlParameter[] sqlParameters)
        {
            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = sqlQuery;
                sqlCommand.Parameters.AddRange(sqlParameters);

                var data = sqlCommand.ExecuteReader();
                var items = new List<T>();
                T obj = default(T);

                while (data.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach (PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(data[prop.Name], DBNull.Value))
                        {
                            prop.SetValue(obj, data[prop.Name], null);
                        }
                    }
                    items.Add(obj);
                }
                return items;
            }
        }

        public List<string> GetTableNames(string tableSchemaName)
        {
            return
                SqlQuery<string>("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=@tableSchemaNam",
                                 new SqlParameter("@tableSchemaName", tableSchemaName));
        }
    }
}