using Dapper;
using System.Data;
using System.Data.SqlClient;
using ILogger = Serilog.ILogger;

namespace User_Infrastructure.Interface
{
    public class DapperRepository : IDapperRepository
    {
        private readonly ILogger logger;

        public DapperRepository(ILogger logger)
        {
            this.logger = logger;
        }

        public void Dispose()
        {
        }

        private IDbConnection UserDBConnection(string SQLConnectionstr)
        {
            IDbConnection db = new SqlConnection(SQLConnectionstr);
            if (db.State == ConnectionState.Closed)
            {
                db.Open();
            }

            return db;
        }

        public async Task<int> ExecuteAsync(string Query, DynamicParameters param, CommandType commandType, string SQLConnectionstr)
        {
            try
            {
                int affectedRows = 0;

                using (IDbConnection db = UserDBConnection(SQLConnectionstr))
                {
                    affectedRows = param == null ? await db.ExecuteAsync(Query, commandType: commandType) : await db.ExecuteAsync(Query, param, commandType: commandType);
                }
                return affectedRows;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " ~ " + ex.InnerException);
                throw;
            }
        }

        public async Task<string> ExecuteScalarAsync(string Query, DynamicParameters param, CommandType commandType, string SQLConnectionstr)
        {
            try
            {
                string data = string.Empty;

                using (IDbConnection db = UserDBConnection(SQLConnectionstr))
                {
                    data = param == null ? await db.ExecuteScalarAsync<string>(Query, commandType: commandType) : await db.ExecuteScalarAsync<string>(Query, param, commandType: commandType);
                }
                return data;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " ~ " + ex.InnerException);
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetDataListAsync<T>(string Query, DynamicParameters param, CommandType commandType, string SQLConnectionstr)
        {
            try
            {
                using IDbConnection db = UserDBConnection(SQLConnectionstr);
                IEnumerable<T> data = param == null ? await db.QueryAsync<T>(Query, commandType: commandType) : await db.QueryAsync<T>(Query, param, commandType: commandType);

                return data;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " ~ " + ex.InnerException);
                throw;
            }
        }

        public async Task<DataSet> GetDataSetAsync(string Query, DynamicParameters param, CommandType commandType, string SQLConnectionstr)
        {
            try
            {
                using IDbConnection db = UserDBConnection(SQLConnectionstr);
                IDataReader list = param == null ? await db.ExecuteReaderAsync(Query, commandType: commandType) : await db.ExecuteReaderAsync(Query, param, commandType: commandType);

                DataSet dataset = ConvertDataReaderToDataSet(list);
                return dataset;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " ~ " + ex.InnerException);
                throw;
            }
        }

        private DataSet ConvertDataReaderToDataSet(IDataReader data)
        {
            DataSet ds = new();
            int i = 0;
            while (!data.IsClosed)
            {
                _ = ds.Tables.Add("Table" + i.ToString());
                ds.EnforceConstraints = false;
                ds.Tables[i].Load(data);
                i++;
            }
            return ds;
        }
    }
}