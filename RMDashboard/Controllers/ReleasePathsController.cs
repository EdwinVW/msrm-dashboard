using RMDashboard.Repositories;
using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Collections.Generic;

namespace RMDashboard.Controllers
{
    /// <summary>
    /// WebAPI controller that retreives the releasepaths defined in RM.
    /// </summary>
    public class ReleasePathsController : ApiController
    {
        private IReleaseRepository _releaseRepository;

        public ReleasePathsController() : this(new ReleaseRepository())
        {
        }

        public ReleasePathsController(IReleaseRepository releaseRepository)
        {
            if (releaseRepository == null) throw new ArgumentNullException("releaseRepository");
            _releaseRepository = releaseRepository;
        }

        // GET: api/releasepath
        public object Get()
        {
            try
            {
                var releasePaths = _releaseRepository.GetReleasePaths();

                IEnumerable<dynamic> result = releasePaths.Select(rp => 
                {
                    dynamic releasePath = new ExpandoObject();
                    releasePath.id = rp.Id;
                    releasePath.name = rp.Name;
                    releasePath.description = rp.Description;
                    return releasePath;
                });

                return result.OrderBy(r => r.name);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
