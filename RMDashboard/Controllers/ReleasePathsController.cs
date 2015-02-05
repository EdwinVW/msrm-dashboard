using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;

namespace RMDashboard.Controllers
{
    public class ReleasePathsController : ApiController
    {
        // GET: api/releasepath
        public object Get()
        {
            try
            {
                var releasePaths = GetData();
                return releasePaths.Select(rp => new { id = rp.Id, name = rp.Name, description = rp.Description });
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private List<ReleasePath> GetData()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ReleaseManagement"].ConnectionString))
            {
                var sql = @"
                    select	Id, 
                            Name, 
                            Description
                    from    ReleasePath";
                return connection.Query<ReleasePath>(sql).ToList();                
            }
        }
    }
}
