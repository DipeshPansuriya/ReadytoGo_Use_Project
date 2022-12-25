using User_Database.Domain;
using ILogger = Serilog.ILogger;

namespace User_Infrastructure.Interface
{
    public class Response_Request : IResponse_Request
    {
        private readonly ILogger logger;
        private readonly IDapperRepository dapper;

        public Response_Request(ILogger logger, IDapperRepository dapper)
        {
            this.logger = logger;
            this.dapper = dapper;
        }

        public async Task<int> RequestSaveAsync(string Scheme, string Path, string QueryString, string Userid, string Request)
        {
            try
            {
                string Query = "Insert Into APIRequest(Scheme, Path, QueryString, Userid, Request, RequestDate) Values (N'" + Scheme + "', N'" + Path + "', N'" + QueryString + "', N'" + Userid + "', N'" + Request.Replace("'", "''") + "', GETDATE()); SELECT CAST(SCOPE_IDENTITY() as int)";

                string id = "0";
                id = await dapper.ExecuteScalarAsync(Query, null, System.Data.CommandType.Text, APISetting.LogDBConnection);
                return int.Parse(id);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return 0;
        }

        public async Task ResponseSaveAsync(string Userid, string Response, int RequestId)
        {
            try
            {
                bool res = Response.Contains("ResponseStatus\": true,") == true;
                if (res == false)
                {
                    res = Response.Contains("ResponseStatus\":true,") == true;
                }

                string Query = "Insert Into APIResponse(Response, RequestId, ResponseDate, ReponseStatus,  UserId) Values (N'" + Response.Replace("'", "''") + "', N'" + RequestId + "', GETDATE(), N'" + res.ToString() + "', N'" + Userid + "'); SELECT CAST(SCOPE_IDENTITY() as int)";
                _ = await dapper.ExecuteScalarAsync(Query, null, System.Data.CommandType.Text, APISetting.LogDBConnection);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }
    }
}